using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    List<GameObject> listGhosts = new List<GameObject>();

    [Header("Highlight Quads")]
    public TileHighlighter moveQuad;
    public TileHighlighter visionQuad;
    public TileHighlighter empQuad;

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
        //} else if (Input.GetMouseButtonDown(2)) { // Middle click
        //    TurnManager.instance.thief.ResetPath();
        }
    }

    /// <summary>
    /// Add a clicked tile that is a target tile to the Thief's path
    /// </summary>
    public void AddToPath() {
        Thief thief = TurnManager.instance.thief;

        if (thief.IsTargetTile(this)) {
            thief.AddTileToPath(this);
            DisplayPathAndTargets();
        }
    }

    /// <summary>
    /// Remove a tile from the end of the path
    /// </summary>
    void RemoveFromPath() {
        Thief thief = TurnManager.instance.thief;

        if (thief.IsTileLastOfPath(this)) {
            thief.RemoveLastTileFromPath();
            thief.DisplayCurrentTargets();
        }
    }

    /// <summary>
    /// Display the path of tiles and the next target tiles of the Thief
    /// </summary>
    public void DisplayPathAndTargets() {
        Thief thief = TurnManager.instance.thief;
        
        thief.TurnTargetTilesOff();
        thief.TurnPathTilesOff();
        thief.DisplayMoveCounter();

        if (thief.CanAddToPath()) {
            HighlightNeighbors();
        }

    }

    /// <summary>
    /// Highlight neighbor tiles as targets to the thief and add them to the target list
    /// </summary>
    public void HighlightNeighbors() { 
        Thief thief = TurnManager.instance.thief;
        thief.TurnTargetTilesOff();

        // The current tile or the tile in the end of the path can be clicked again
        moveQuad.ChangeColorToThiefMove();
        moveQuad.TurnHighlighterOn();
        thief.AddTileToTargets(this);

        // Highlight neighbors
        foreach (var tile in listNeighbors) {
            if (tile.IsWalkable()) { // Non-walkable tiles are not added
                
                // Do not highlight tiles that are already on the path
                tile.moveQuad.ChangeColorToThiefMove();
                tile.moveQuad.TurnHighlighterOn();
                thief.AddTileToTargets(tile);
            }
        }
    }

    #endregion

    #region GHOST_THIEVES

    /// <summary>
    /// Add a ghost thief to the list of ghosts
    /// </summary>
    /// <param name="ghost"></param>
    public void AddGhostToTile(ref GameObject ghost) {
        listGhosts.Add(ghost);
    }

    /// <summary>
    /// Remove the last ghost from the tiles
    /// </summary>
    /// <returns>Return the removed ghost</returns>
    public GameObject RemoveLastGhost() {
        GameObject lastGhost = null;
        if (listGhosts.Count > 0) {
            lastGhost = listGhosts.Last();
            listGhosts.RemoveAt(listGhosts.Count - 1);
        }
        return lastGhost;
    }

    /// <summary>
    /// Remove all ghosts from the tile
    /// </summary>
    /// <returns>Return a list with all removed ghosts</returns>
    public List<GameObject> RemoveEveryGhost() {
        List<GameObject> removedGhosts = new List<GameObject>();
        while (listGhosts.Count > 0) {
            removedGhosts.Add(RemoveLastGhost());
        }
        return removedGhosts;
    }

    #endregion

    #region DOOR_AND_WALLS

    /// <summary>
    /// Checks if the Thief can walk on this tile
    /// </summary>
    /// <returns></returns>
    public bool IsWalkable() {
        return (tileType != ETileType.WALL && (tileType != ETileType.DOOR || (tileType == ETileType.DOOR && door.IsOpen)));
    }

    /// <summary>
    /// Open the door and change the tileType to DEFAULT
    /// </summary>
    public void OpenDoor() {
        door.OpenDoor();
    }

    #endregion

    #region EQUALITY_OVERLOAD
    public override bool Equals(object other) {
        return Equals(other as Tile);
    }

    public bool Equals(Tile tile) {
        if (!tile) { return false; } // tile is null

        //return (coordinates == tile.coordinates && gridManager.Equals(tile.gridManager));
        return (gameObject.transform.position == tile.gameObject.transform.position);
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

    #endregion

    #region EDITOR_USE

    /// <summary>
    /// EDITOR USE
    /// Add the Quad gameObjects to the tile highlighters
    /// </summary>
    [ContextMenu("Add Quads")]
    public void AddQuads() {
        List<TileHighlighter> moveHighlighter = GetComponentsInChildren<TileHighlighter>().Where(x => x.gameObject.name == "MoveQuad").ToList();
        List<TileHighlighter> visionHighlighter = GetComponentsInChildren<TileHighlighter>().Where(x => x.gameObject.name == "VisionQuad").ToList();
        List<TileHighlighter> empHighlighter = GetComponentsInChildren<TileHighlighter>().Where(x => x.gameObject.name == "EMPQuad").ToList();
        moveQuad = moveHighlighter[0];
        visionQuad = visionHighlighter[0];
        empQuad = empHighlighter[0];
    }

    #endregion
}


