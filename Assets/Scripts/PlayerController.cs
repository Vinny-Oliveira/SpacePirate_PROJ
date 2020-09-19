using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float fltSpeed = 5;
    public int intRange = 2;

    public TileLocation currentTile;
    public GridManager currentGrid;

    private void Start() {
        if (currentGrid == null) {
            Debug.Log("ERROR: Assign a Grid!");
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }

        if (currentTile == null) { 
            if (currentGrid.listTempTiles.Count < 1) {
                Debug.Log("ERROR: Create a Tile Map!");
                UnityEditor.EditorApplication.isPlaying = false;
                return;
            }

            currentTile = currentGrid.listTempTiles[0].GetComponent<TileLocation>();
        }

        MoveToTile(currentTile);
    }

    /// <summary>
    /// Move player to tile with coordinates x and z
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void MoveToTile(float x, float z) {
        Vector3 target = new Vector3(x, transform.position.y, z);
        transform.position = target;
    }

    /// <summary>
    /// Move player to tile with coordinate target
    /// </summary>
    /// <param name="target"></param>
    public void MoveToTile(Vector3 target) {
        MoveToTile(target.x, target.z);
    }

    /// <summary>
    /// Move the player to given tile
    /// </summary>
    /// <param name="tile"></param>
    public void MoveToTile(TileLocation tile) {
        currentTile = tile;
        MoveToTile(tile.GetLocation());
    }



}
