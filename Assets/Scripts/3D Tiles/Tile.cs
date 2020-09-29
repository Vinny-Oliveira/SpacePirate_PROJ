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

    public Material defaultMaterial;

    #region LOCATION_AND_NEIGHBORS

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
    /// Returns whether or not the tile has the input in its list of neighbors
    /// </summary>
    /// <param name="neighbor"></param>
    /// <returns></returns>
    public bool HasNeighbor(Tile neighbor) {
        return listNeighbors.Contains(neighbor);
    }

    #endregion

    #region MATERIAL_MANAGEMENT

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
    /// Place the path material on the tile
    /// </summary>
    public void PathMatTile() {
        GetComponent<MeshRenderer>().sharedMaterial = gridManager.pathMat;
    }

    #endregion

    #region EQUALITY_OVERLOAD
    public override bool Equals(object other) {
        return this.Equals(other as Tile);
    }

    public bool Equals(Tile tile) {
        if (tile == null) return false;

        return coordinates == tile.coordinates;
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

#endregion

}
