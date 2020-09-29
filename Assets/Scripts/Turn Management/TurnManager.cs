using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {

    public ThiefController thief;
    public Treasure treasure;

    public Tile exitTile;

    public static TurnManager instance;

    private void Awake() {
        instance = this;
    }

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
    public bool IsCubeTouchingThief(ref Tile cubeTile) {
        return AreTilesTheSame(ref cubeTile, ref thief.currentTile);
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
        return (thief.GetHasTreasure() && AreTilesTheSame(ref exitTile, ref thief.currentTile));
    }

    /// <summary>
    /// Make the player grab the treasure
    /// </summary>
    public void MakeThiefGrabTreasure() {
        treasure.transform.position = thief.transform.position;
        treasure.transform.parent = thief.treasureHolder.transform;
        Debug.Log("Treasure Caught!");
    }
}
