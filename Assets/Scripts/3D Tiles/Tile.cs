using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ETileType {
    DEFAULT = 0,
    WIRED = 1,
    WALL = 2,
    DOOR = 3
}

public class Tile : MonoBehaviour, IEquatable<Tile> {

    public GridManager gridManager;
    public Vector3 coordinates;
    public ETileType tileType;
    public List<Tile> listNeighbors;

    public TileHighlighter tileHighlighter;

    #region LOCATION_AND_NEIGHBORS

    /// <summary>
    /// Setter of coordinates and tileType
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="type"></param>
    public void SetLocation(int x, int z) {
        coordinates = new Vector3(x, 0, z);
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

    /// <summary>
    /// Checks if the Thief can walk on this tile
    /// </summary>
    /// <returns></returns>
    public bool IsWalkable() {
        return (tileType != ETileType.WALL && tileType != ETileType.DOOR);
    }

    #endregion

    #region EQUALITY_OVERLOAD
    public override bool Equals(object other) {
        return Equals(other as Tile);
    }

    public bool Equals(Tile tile) {
        if (!tile) { return false; } // tile is null

        return (coordinates == tile.coordinates && gridManager.Equals(tile.gridManager));
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

    #endregion

}


