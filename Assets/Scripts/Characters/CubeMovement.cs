using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eDirection { 
    NORTHEAST = 0,
    NORTHWEST = 1,
    SOUTHEAST = 2,
    SOUTHWEST = 3
}

public class CubeMovement : MonoBehaviour {

    public GridManager grid;
    public Tile currentTile;

    public GameObject center;
    public GameObject northEast;
    public GameObject northWest;
    public GameObject southEast;
    public GameObject southWest;

    public int step = 9;
    public float speed = 0.01f;

    /// <summary>
    /// Roll towards Northeast
    /// </summary>
    public void MoveNorthEast() {
        StartCoroutine(Roll_Cube(northEast));
    }
    
    /// <summary>
    /// Roll towards Northwest
    /// </summary>
    public void MoveNorthWest() {
        StartCoroutine(Roll_Cube(northWest));
    }
    
    /// <summary>
    /// Roll towards Southeast
    /// </summary>
    public void MoveSouthEast() {
        StartCoroutine(Roll_Cube(southEast));
    }
    
    /// <summary>
    /// Roll towards Northeast
    /// </summary>
    public void MoveSouthWest() {
        StartCoroutine(Roll_Cube(southWest));
    }

    /// <summary>
    /// Roll the cube in the given cardinal direction (NE, NW, SE, or SW)
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    IEnumerator Roll_Cube(GameObject direction) {
        center.transform.parent = null;

        // Rotate the cube around the given direction object
        for (int i = 0; i < (90/step); i++) {
            transform.RotateAround(direction.transform.position, Vector3.right, step);
            yield return new WaitForSeconds(speed);
        }

        // Reset the center object
        center.transform.parent = transform;
        center.transform.position = transform.position;
    }
}
