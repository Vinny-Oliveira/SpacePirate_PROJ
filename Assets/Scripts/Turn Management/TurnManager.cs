using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {

    public PlayerController thief;
    public CubeMovement cube;
    public Treasure treasure;

    public Tile exitTile;

    /// <summary>
    /// Check if two tiles are the same
    /// </summary>
    /// <param name="tile1"></param>
    /// <param name="tile2"></param>
    /// <returns></returns>
    bool AreTilesTheSame(ref Tile tile1, ref Tile tile2) { 
        if (tile1.Equals(tile2)) {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if the cube and the thief are on the same tile
    /// </summary>
    /// <returns></returns>
    public bool IsCubeTouchingThief() {
        return AreTilesTheSame(ref cube.currentTile, ref thief.currentTile);
    }

    /// <summary>
    /// Check if the player caught the treasure
    /// </summary>
    /// <returns></returns>
    public bool IsThiefTouchingTreasure() {
        return AreTilesTheSame(ref treasure.placeTile, ref thief.currentTile);
    }

    /// <summary>
    /// Check if the thief has escaped with the treasure
    /// </summary>
    /// <returns></returns>
    public bool HasThiefEscaped() {
        return (thief.hasTreasure && AreTilesTheSame(ref exitTile, ref thief.currentTile));
    }
}
