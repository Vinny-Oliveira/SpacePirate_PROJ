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

    /* Path where the Cube rolls */
    public List<EDirection> listPath= new List<EDirection>();

    /* Direction GameObjects */
    public GameObject center;
    public GameObject northEast;
    public GameObject northWest;
    public GameObject southEast;
    public GameObject southWest;

    /* Rotation parameters */
    public int step = 9;

    /* Map each direction enum to a direction game object, an axis of rotation, and a set of coordinates */
    Dictionary<EDirection, Tuple<GameObject, Vector3, Vector3>> dicDirections;

    private void Start() {
        IsMoving = false;
        SetStartingTile();
        BuildDirectionDictionary();
    }

    /// <summary>
    /// Map each direction enum to a direction game object, an axis of rotation, and a set of coordinates
    /// </summary>
    void BuildDirectionDictionary() {
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
                yield return StartCoroutine(Roll_Cube(direction));
                currentTile = nextTile;
                yield return StartCoroutine(WaitOnTile());
            }

            // Check if the thief was caught
            if (turnManager.HandleNewTile(ref currentTile)) {
                yield break;
            }

        }

        IsMoving = false;
        turnManager.DecreaseMovementCount();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position - 2 * transform.right, transform.localPosition + 2*transform.right);
    }

}
