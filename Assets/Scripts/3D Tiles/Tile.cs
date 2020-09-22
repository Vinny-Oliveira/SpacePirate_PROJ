using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType {
    EVEN = 0,
    ODD = 1
}

public class Tile : MonoBehaviour, IEquatable<Tile> {

    public GridManager gridManager;
    public Vector3 coordinates;
    public TileType tileType;
    public List<Tile> listNeighbors;

    bool canMoveHere = false;
    public Material defaultMaterial;

    /// <summary>
    /// Store the tile's default material
    /// </summary>
    public void SetDefaultMaterial() {
        defaultMaterial = GetComponent<MeshRenderer>().sharedMaterial;
    }

    /// <summary>
    /// Turn the tile's material back to the default one
    /// </summary>
    public void ResetMaterial() {
        GetComponent<MeshRenderer>().sharedMaterial = defaultMaterial;
    }

    /// <summary>
    /// Place the highlight material on the tile
    /// </summary>
    public void HighlightTile() {
        GetComponent<MeshRenderer>().sharedMaterial = gridManager.highlightMat;
    }

    /// <summary>
    /// Setter of coordinates and tileType
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="type"></param>
    public void SetLocation(int x, int z, TileType type) {
        coordinates = new Vector3(x, 0, z);
        tileType = type;
    }

    /// <summary>
    /// Getter of coordinates
    /// </summary>
    /// <returns></returns>
    public Vector3 GetLocation() {
        return coordinates;
    }

    /// <summary>
    /// Event for when the tile is clicked
    /// </summary>
    private void OnMouseDown() {
        MovePlayerHere();
    }

    /// <summary>
    /// Move the player to this tile
    /// </summary>
    void MovePlayerHere() { 
        if (gridManager != null && gridManager.player != null && canMoveHere) {
            gridManager.player.MoveToTile(this);
        }
    }

#region EQUALITY_OVERLOAD
    public override bool Equals(object other) {
        return this.Equals(other as Tile);
    }

    public bool Equals(Tile tile) {
        if (tile == null) return false;

        return this.coordinates == tile.coordinates;
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

#endregion

}
