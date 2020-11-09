using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Directions the cube can roll to
/// </summary>
public enum EDirection { 
    NE = 0, // NorthEast
    NW = 1, // NorthWest
    SE = 2, // SouthEast
    SW = 3  // SouthWest
}

public class CubeBot : Enemy {

    [Header("Path where the Cube rolls")]
    [SerializeField]
    int intRollsPerTurn = 1;
    public List<EDirection> listPath = new List<EDirection>();
    Queue<EDirection> quePath = new Queue<EDirection>();

    [Header("Direction GameObjects")]
    public GameObject center;
    public GameObject northEast;
    public GameObject northWest;
    public GameObject southEast;
    public GameObject southWest;

    [Header("Rotation and Field of View")]
    const int step = 9;
    public List<Vector2> listViewCoords = new List<Vector2>();

    /* Map each direction enum to a direction game object, an axis of rotation, and a set of coordinates */
    Dictionary<EDirection, Tuple<GameObject, Vector3, Vector3>> dicDirections;

    /* Initial setup */
    float initPosY;
    Quaternion initCenterRotation;

    #region INITIAL_SETUP

    /// <summary>
    /// Setup all initial values for the cube
    /// </summary>
    public void SetupCubeStart(int rolls) {
        intRollsPerTurn = rolls;
        IsMoving = false;
        CanStep = true;
        IsDisabled = false;
        SetStartingTile();
        StoreStartingPosition();
        BuildDirectionDictionary();
        EnqueueThePath();
        SetFieldOfView();
    }

    /// <summary>
    /// Turn the list of path to a Queue
    /// </summary>
    void EnqueueThePath() { 
        foreach (var direction in listPath) {
            quePath.Enqueue(direction);
        }
    }

    /// <summary>
    /// Store the initial values of height (position.y) and rotation
    /// </summary>
    void StoreStartingPosition() {
        initPosY = transform.position.y;
        initCenterRotation = center.transform.rotation;
    }

    /// <summary>
    /// Reset the cube's rotation and Y position to the initial ones
    /// </summary>
    void ResetPositionToStart() {
        transform.position = new Vector3(transform.position.x, initPosY, transform.position.z);
        center.transform.rotation = initCenterRotation;
    }

    /// <summary>
    /// Map each direction enum to a direction game object, an axis of rotation, and a set of coordinates
    /// </summary>
    public void BuildDirectionDictionary() {
        dicDirections = new Dictionary<EDirection, Tuple<GameObject, Vector3, Vector3>> {
            { EDirection.NE, new Tuple<GameObject, Vector3, Vector3>(northEast, Vector3.right, Vector3.forward) },
            { EDirection.NW, new Tuple<GameObject, Vector3, Vector3>(northWest, Vector3.forward, Vector3.left) },
            { EDirection.SE, new Tuple<GameObject, Vector3, Vector3>(southEast, Vector3.back, Vector3.right) },
            { EDirection.SW, new Tuple<GameObject, Vector3, Vector3>(southWest, Vector3.left, Vector3.back) }
        };

    }

    #endregion

    #region MOVEMENT

    /// <summary>
    /// Roll the cube in the given cardinal direction (NE, NW, SE, or SW)
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    IEnumerator Roll_Cube(EDirection direction) {
        center.transform.parent = null;
        float waitTime = stepTime / (step + stepTime * (step + 1f));

        // Rotate the cube around the given direction object
        for (int i = 0; i < (90 / step); i++) {
            GameObject goDirection = dicDirections[direction].Item1;
            Vector3 axis = dicDirections[direction].Item2;

            transform.RotateAround(goDirection.transform.position, axis, step);
            yield return new WaitForSeconds(waitTime);
        }

        // Reset the center object
        center.transform.localScale = Vector3.one;
        center.transform.parent = transform;
        center.transform.position = transform.position;
    }

    /// <summary>
    /// Move on the path set to the Cube
    /// </summary>
    public override void MoveOnPath() {
        // Check if the cube is not disabled
        if (IsDisabled) {
            ReduceOneWaitTurn();
            TurnManager.instance.DecreaseMovementCount();
        } else { 
            StartCoroutine(MoveOnEachDirection());
        }
    }

    /// <summary>
    /// Go through the list of directions in the path and perform the movement
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveOnEachDirection() {
        IsMoving = true;
        TurnManager turnManager = TurnManager.instance;

        for (int i = 0; i < intRollsPerTurn; i++) {
            // If the Thief activates the EMP mid-path, disable the cube
            if (IsDisabled) {
                DisableFieldOfView();
                ReduceOneWaitTurn();
                TurnManager.instance.DecreaseMovementCount();
                yield break;
            }

            CanStep = false;
            EDirection direction = quePath.Dequeue();
            quePath.Enqueue(direction);
            Vector3 nextCoordinates = currentTile.coordinates + dicDirections[direction].Item3;
            Tile nextTile = currentTile.listNeighbors.Find(x => x.coordinates == nextCoordinates);

            // Only roll to a tile that is within the grid
            if (nextTile != null) {
                DisableFieldOfView();
                yield return StartCoroutine(Roll_Cube(direction));

                // Position cube on the tile and turn field of view on
                MoveToTile(ref nextTile);
                SetFieldOfView();
            }

            // Wait
            CanStep = true;
            yield return new WaitUntil(() => TurnManager.instance.CanCharactersStep());
            yield return StartCoroutine(WaitOnTile());
            
            // Check if the thief was caught
            if (turnManager.IsThiefCaught(ref currentTile, ref listFieldOfView)) {
                yield break;
            }

        }
        
        IsMoving = false;
        ResetPositionToStart();
        turnManager.DecreaseMovementCount();
    }

    #endregion

    #region FIELD_OF_VIEW

    /// <summary>
    /// Check the alignments of the local right and local forward directions
    /// </summary>
    /// <param name="newForward"></param>
    /// <param name="newRight"></param>
    /// <returns></returns>
    bool RecalculateLocalDirections(ref Vector3 newForward, ref Vector3 newRight) {
        float newX = Mathf.Abs((float)Math.Round(newForward.x, 3));
        float newY = Mathf.Abs((float)Math.Round(newForward.y, 3));
        float newZ = Mathf.Abs((float)Math.Round(newForward.z, 3));

        // Eyes on the global x-axis
        if (Mathf.Approximately(newX, Vector3.right.x) && Mathf.Approximately(newY, Vector3.right.y) 
                && Mathf.Approximately(newZ, Vector3.right.z)) {
            newForward = Vector3.forward;
            newRight = Vector3.right;
            return true;
        
        // Eyes on the global z-axis
        } else if (Mathf.Approximately(newX, Vector3.forward.x) && Mathf.Approximately(newY, Vector3.forward.y) 
                && Mathf.Approximately(newZ, Vector3.forward.z)) {
            newForward = Vector3.right;
            newRight = Vector3.forward;
            return true;
        }

        // Eyes on the global y-axis
        return false;
    }

    /// <summary>
    /// Check which tiles are in the Cube's field of view
    /// </summary>
    public void SetFieldOfView() {
        DisableFieldOfView();
        
        Vector3 newForward = transform.right;
        Vector3 newRight = transform.right;

        // Cube looking up
        if (!RecalculateLocalDirections(ref newForward, ref newRight)) { 
            return;
        }
        
        // Calculate coordinate of the new tile and check if it exists
        foreach (var newCoord in listViewCoords) {
            Vector3 newTileCoord = currentTile.coordinates + newCoord.x * newRight + newCoord.y * newForward;
            AddTileWithCoordinates(newTileCoord);
        }
        
        HighlightFieldOfView();
    }

    /// <summary>
    /// Enable the enemy and turn the field of view on
    /// </summary>
    public override void EnableEnemy() {
        base.EnableEnemy();
        SetFieldOfView();
    }

    #endregion

    //private void OnDrawGizmos() {
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position - 2 * transform.right, transform.position + 2*transform.right);
    //}

}
