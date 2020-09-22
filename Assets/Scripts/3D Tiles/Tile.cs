using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType {
    EVEN = 0,
    ODD = 1
}
public class Tile : MonoBehaviour {//, IEquatable<Tile> {

    public GridManager gridManager;
    public Vector3 coordinates;
    public TileType tileType;
    public List<Tile> listNeighbors;

    bool canMoveHere = false;
    public Material defaultMaterial;

    public void SetDefaultMaterial() {
        defaultMaterial = GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void ResetMaterial() {
        GetComponent<MeshRenderer>().sharedMaterial = defaultMaterial;
    }

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
        if (gridManager != null && gridManager.player != null && canMoveHere) {
            gridManager.player.MoveToTile(this);
        }
    }

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

    //public static bool operator ==(Tile tile1, Tile tile2) {
    //    if (Tile.ReferenceEquals(tile1, null) || Tile.ReferenceEquals(tile2, null)) {
    //        return false;
    //    }

    //    return tile1.Equals(tile2);
    //}
    
    //public static bool operator !=(Tile tile1, Tile tile2) {
    //    return !(tile1 == tile2);
    //}

}
