using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Directions the cube can roll to
/// </summary>
public enum EDirection { 
    NORTHEAST = 0,
    NORTHWEST = 1,
    SOUTHEAST = 2,
    SOUTHWEST = 3
}

public class CubeMovement : Character {

    [Header("Path where the Cube rolls")]
    public List<EDirection> listPath= new List<EDirection>();

    [Header("Direction GameObjects")]
    public GameObject center;
    public GameObject northEast;
    public GameObject northWest;
    public GameObject southEast;
    public GameObject southWest;

    [Header("Rotation and Field of View")]
    public int step = 9;
    public List<Vector2> listViewCoords = new List<Vector2>();
    public List<Tile> listFieldOfView = new List<Tile>(); // Field of view

    /* Map each direction enum to a direction game object, an axis of rotation, and a set of coordinates */
    Dictionary<EDirection, Tuple<GameObject, Vector3, Vector3>> dicDirections;

    public void SetupCubeStart() {
        IsMoving = false;
        SetStartingTile();
        BuildDirectionDictionary();
        SetFieldOfView();
    }

    /// <summary>
    /// Map each direction enum to a direction game object, an axis of rotation, and a set of coordinates
    /// </summary>
    public void BuildDirectionDictionary() {
        dicDirections = new Dictionary<EDirection, Tuple<GameObject, Vector3, Vector3>> {
            { EDirection.NORTHEAST, new Tuple<GameObject, Vector3, Vector3>(northEast, Vector3.right, Vector3.forward) },
            { EDirection.NORTHWEST, new Tuple<GameObject, Vector3, Vector3>(northWest, Vector3.forward, Vector3.left) },
            { EDirection.SOUTHEAST, new Tuple<GameObject, Vector3, Vector3>(southEast, Vector3.back, Vector3.right) },
            { EDirection.SOUTHWEST, new Tuple<GameObject, Vector3, Vector3>(southWest, Vector3.left, Vector3.back) }
        };

    }

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
        StartCoroutine(MoveOnEachDirection());
    }

    /// <summary>
    /// Go through the list of directions in the path and perform the movement
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveOnEachDirection() {
        IsMoving = true;
        TurnManager turnManager = TurnManager.instance;

        foreach (var direction in listPath) {
            Vector3 nextCoordinates = currentTile.coordinates + dicDirections[direction].Item3;
            Tile nextTile = currentTile.listNeighbors.Find(x => x.coordinates == nextCoordinates);

            // Only roll to a tile that is within the grid
            if (nextTile != null) {
                DisableFieldOfView();
                yield return StartCoroutine(Roll_Cube(direction));
                //currentTile = nextTile;
                MoveToTile(ref nextTile);
                SetFieldOfView();
                yield return StartCoroutine(WaitOnTile());
            }

            // Check if the thief was caught
            if (turnManager.HandleNewTile(ref currentTile, ref listFieldOfView)) {
                yield break;
            }

        }

        IsMoving = false;
        turnManager.DecreaseMovementCount();
    }

    #region FIELD_OF_VIEW

    /// <summary>
    /// Check which tiles are in the Cube's field of view
    /// </summary>
    public void SetFieldOfView() {
        DisableFieldOfView();
        
        Vector3 newForward = transform.right.normalized;
        Vector3 newRight = transform.right.normalized;

        //if (Mathf.Abs(Vector3.Dot(newForward, Vector3.right)) > 0.95f) {
        //    newForward = Vector3.right;
        //} else if (Mathf.Abs(Vector3.Dot(newForward, Vector3.forward)) > 0.95f) {
        //    newForward = Vector3.forward;
        //} else {
        //    return;
        //}

        float newX = Mathf.Abs((float)System.Math.Round(newForward.x, 3));
        float newY = Mathf.Abs((float)System.Math.Round(newForward.y, 3));
        float newZ = Mathf.Abs((float)System.Math.Round(newForward.z, 3)); // Mathf.Abs(newForward.z);

        // Eyes on the global x-axis
        if (Mathf.Approximately(newX, Vector3.right.x) && Mathf.Approximately(newY, Vector3.right.y) 
                && Mathf.Approximately(newZ, Vector3.right.z)) {
            //if (transform.right.normalized == Vector3.right || transform.right.normalized == -Vector3.right) {
            newForward = Vector3.forward;
            newRight = Vector3.right;

            // Eyes on the global z-axis
        } else if (Mathf.Approximately(newX, Vector3.forward.x) && Mathf.Approximately(newY, Vector3.forward.y) 
                && Mathf.Approximately(newZ, Vector3.forward.z)) {
        //} else if (transform.right.normalized == Vector3.forward || transform.right.normalized == -Vector3.forward) {
            newForward = Vector3.right;
            newRight = Vector3.forward;
        
        // Eyes on the global y-axis
        } else {
            return;
        }
        
        // Calculate coordinate of the new tile and check if it exists
        foreach (var newCoord in listViewCoords) {
            Vector3 newTileCoord = currentTile.coordinates + newCoord.x * newRight + newCoord.y * newForward;
            //Vector3 newTileCoord = currentTile.coordinates + newCoord.x * transform.right + newCoord.y * newForward;
            //if (newTileCoord.y < 0.01f) { newTileCoord.y = 0f; }
            Tile viewedTile = currentGrid.listGridTiles.Find(tile => tile.coordinates == newTileCoord);

            if (viewedTile != null) { 
                listFieldOfView.Add(viewedTile);
            }
        }
        
        HighlightFieldOfView();
    }

    /// <summary>
    /// Highlight the tiles in the Cube's field of view
    /// </summary>
    public void HighlightFieldOfView() { 
        foreach (var tile in listFieldOfView) {
            tile.tileHighlighter.ChangeColorToCubeView();
            tile.tileHighlighter.TurnHighlighterOn();
        }
    }

    /// <summary>
    /// Clear the filed of view tile list and turn their highlighters off
    /// </summary>
    public void DisableFieldOfView() { 
        foreach (var tile in listFieldOfView) {
            tile.tileHighlighter.TurnHighlighterOff();
        }
        listFieldOfView.Clear();
    }

    #endregion

    //private void OnDrawGizmos() {
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawLine(transform.position - 2 * transform.right, transform.position + 2*transform.right);
    //}

}
