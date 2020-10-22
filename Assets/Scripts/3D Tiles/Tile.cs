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

    [Header("Manage Location")]
    public GridManager gridManager;
    public Vector3 coordinates;

    [Header("Control Path")]
    public ETileType tileType;
    public List<Tile> listNeighbors;
    public TileHighlighter tileHighlighter;

    [Header("If tile is a DOOR, add the Door")]
    public Door door;

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

    #endregion

    #region PATH_OF_TILES

    /// <summary>
    /// Add tiles to the path when you click on them
    /// </summary>
    private void OnMouseDown() {
        if (TurnManager.instance.CanClick) {
            AddToPath();
        }
    }

    /// <summary>
    /// Remove tiles from the path when you click on them
    /// </summary>
    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(1) && TurnManager.instance.CanClick) { // Right click
            RemoveFromPath();
        }
    }

    /// <summary>
    /// Add a clicked tile that is a target tile to the Thief's path
    /// </summary>
    public void AddToPath() {
        ThiefController thief = TurnManager.instance.thief;

        if (thief.IsTargetTile(this)) {
            thief.AddTileToPath(this);
            DisplayPathAndTargets();
        }
    }

    /// <summary>
    /// Remove a tile from the end of the path
    /// </summary>
    void RemoveFromPath() {
        ThiefController thief = TurnManager.instance.thief;

        if (thief.IsTileLastOfPath(this)) {
            thief.RemoveLastTileFromPath();

            if (thief.LastPathTile) { 
                thief.LastPathTile.DisplayPathAndTargets();
            } else {
                thief.currentTile.DisplayPathAndTargets();
            }
        }
    }

    /// <summary>
    /// Display the path of tiles and the next target tiles of the Thief
    /// </summary>
    public void DisplayPathAndTargets() {
        ThiefController thief = TurnManager.instance.thief;
        
        thief.TurnTargetTilesOff();
        TurnManager.instance.HighlightCubesFieldsOfView();
        thief.HighlightPathTiles();
        thief.DisplayMoveCounter();

        if (thief.CanAddToPath()) {
            HighlightNeighbors();
        }

    }

    /// <summary>
    /// Highlight neighbor tiles as targets to the thief and add them to the target list
    /// </summary>
    public void HighlightNeighbors() { 
        ThiefController thief = TurnManager.instance.thief;
        thief.TurnTargetTilesOff();

        foreach (var tile in listNeighbors) {
            if (tile.IsWalkable()) { // Non-walkable tiles are not added
                
                if (!thief.IsTileOnPath(tile)) { // Do not highlight tiles that are already on the path
                    tile.tileHighlighter.ChangeColorToThiefRange();
                    tile.tileHighlighter.TurnHighlighterOn();
                }

                thief.AddTileToTargets(tile);
            }
        }
    }

    #endregion

    #region DOOR_AND_WALLS

    /// <summary>
    /// Checks if the Thief can walk on this tile
    /// </summary>
    /// <returns></returns>
    public bool IsWalkable() {
        return (tileType != ETileType.WALL && tileType != ETileType.DOOR);
    }

    /// <summary>
    /// Open the door and change the tileType to DEFAULT
    /// </summary>
    public void OpenDoor() {
        door.OpenDoor();
        tileType = ETileType.DEFAULT;
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


