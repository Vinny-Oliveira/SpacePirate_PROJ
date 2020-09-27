using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EDirection { 
    NORTHEAST = 0,
    NORTHWEST = 1,
    SOUTHEAST = 2,
    SOUTHWEST = 3
}

public class CubeMovement : Character {

    public List<EDirection> listPath= new List<EDirection>();

    public GameObject center;
    public GameObject northEast;
    public GameObject northWest;
    public GameObject southEast;
    public GameObject southWest;

    public int step = 9;
    public float speed = 0.01f;

    Dictionary<EDirection, Tuple<GameObject, Vector3>> dicDirections;

    private void Start() {
        SetStartingTile();
        BuildDirectionDictionary();
    }

    /// <summary>
    /// Add all directions to a dictionary
    /// </summary>
    void BuildDirectionDictionary() {
        dicDirections = new Dictionary<EDirection, Tuple<GameObject, Vector3>> {
            { EDirection.NORTHEAST, new Tuple<GameObject, Vector3>(northEast, Vector3.right) },
            { EDirection.NORTHWEST, new Tuple<GameObject, Vector3>(northWest, Vector3.forward) },
            { EDirection.SOUTHEAST, new Tuple<GameObject, Vector3>(southEast, Vector3.back) },
            { EDirection.SOUTHWEST, new Tuple<GameObject, Vector3>(southWest, Vector3.left) }
        };

    }

    /// <summary>
    /// Roll the cube in the given cardinal direction (NE, NW, SE, or SW)
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    IEnumerator Roll_Cube(EDirection direction) {
        center.transform.parent = null;

        // Rotate the cube around the given direction object
        for (int i = 0; i < (90/step); i++) {
            GameObject goDirection = dicDirections[direction].Item1;
            Vector3 axis = dicDirections[direction].Item2;

            transform.RotateAround(goDirection.transform.position, axis, step);
            yield return new WaitForSeconds(speed);
        }

        // Reset the center object
        center.transform.parent = transform;
        center.transform.position = transform.position;
    }

    /// <summary>
    /// Move on the path set to the Cube
    /// </summary>
    [ContextMenu("ROLL")]
    public override void MoveOnPath() {
        StartCoroutine(MoveOnEachDirection());
    }

    /// <summary>
    /// Go through the list of directions in the path and perform the movement
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveOnEachDirection() { 
        foreach (var direction in listPath) {
            yield return StartCoroutine(Roll_Cube(direction));
        }
    }
}
