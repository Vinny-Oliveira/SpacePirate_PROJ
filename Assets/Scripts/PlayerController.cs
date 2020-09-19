using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float fltSpeed = 5;
    public TileLocation currentTile;

    public void MoveToTile(float x, float z) {
        Vector3 target = new Vector3(x, transform.position.y, z);
        //Vector3.MoveTowards(transform.position, target, fltSpeed * Time.deltaTime);
        transform.position = target;
    }

    public void MoveToTile(Vector3 target) {
        MoveToTile(target.x, target.z);
    }

    public void MoveToTile(TileLocation tile) {
        currentTile = tile;
        MoveToTile(tile.GetLocation());
    }

}
