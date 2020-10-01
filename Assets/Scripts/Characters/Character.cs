﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    /* Grid Control */
    public Tile currentTile;
    public GridManager currentGrid;
    protected bool IsMoving { get; set; }
    public static float stepTime = 0.5f;

    /// <summary>
    /// Move the player to given tile
    /// </summary>
    /// <param name="nextTile"></param>
    protected virtual void MoveToTile(ref Tile nextTile) {
        currentTile = nextTile;
        //Vector3 target = new Vector3(nextTile.GetLocation().x, transform.position.y, nextTile.GetLocation().z);
        //transform.position = target;
    }

    /// <summary>
    /// Set the value of the tile the player starts on and move them there
    /// </summary>
    protected void SetStartingTile() { 
        if (currentGrid == null) {
            Debug.Log("ERROR: Assign a Grid!");
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }

        if (currentTile == null) { 
            if (currentGrid.GetTileList().Count < 1) {
                Debug.Log("ERROR: Create a Tile Map!");
                UnityEditor.EditorApplication.isPlaying = false;
                return;
            }

            currentTile = currentGrid.GetTileList()[0].GetComponent<Tile>();
        }

        MoveToTile(ref currentTile);
    }

    public virtual void MoveOnPath() { 
    
    }
}
