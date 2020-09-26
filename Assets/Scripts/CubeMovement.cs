using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMovement : MonoBehaviour {

    Vector3 offset;

    public GameObject center;
    public GameObject NorthEast;
    public GameObject SouthWest;
    public GameObject NorthWest;
    public GameObject SouthEast;

    public int step = 9;
    public float speed = 0.01f;
    bool input = true;

    [ContextMenu("MoveCube")]
    public void MoveNorthEast() {
        StartCoroutine(MoveNE());
    }

    IEnumerator MoveNE() {
        //center.transform.parent = null;
        for (int i = 0; i < (90/step); i++) {
            transform.RotateAround(NorthEast.transform.position, Vector3.right, step);
            yield return new WaitForSeconds(speed);
        }
        input = true;
        //center.transform.parent = transform;
        center.transform.position = transform.position;
    }
}
