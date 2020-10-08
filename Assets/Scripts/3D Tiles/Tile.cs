using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ETileType {
    EVEN = 0,
    ODD = 1
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
    public void SetLocation(int x, int z, ETileType type) {
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

    #region EQUALITY_OVERLOAD
    public override bool Equals(object other) {
        return Equals(other as Tile);
    }

    public bool Equals(Tile tile) {
        if (!tile) { return false; } // tile is null

        return coordinates == tile.coordinates;
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

    //public static bool operator ==(Tile lhs, Tile rhs) {
    //    return lhs.Equals(rhs);
    //}
    
    //public static bool operator !=(Tile lhs, Tile rhs) {
    //    return !lhs.Equals(rhs);
    //}

#endregion

}


