using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour/*, System.IEquatable<GridManager>*/ {

    //public Transform cameraHolder;

    public int[,] grid = new int[0,0]; // Grid with positions

    public List<GameObject> listPrefabTiles; // List with all the tile prefabs
    public List<Tile> listGridTiles = new List<Tile>(); // List of tiles spawned in the scene

    public Tile[,] tileLocationMap;
    private int currentMapSizeX;
    private int currentMapSizeZ;

    // Grid size
    public int intMapSizeX = 10;
    public int intMapSizeZ = 10;

    #region TILE_MAP_CREATION

    /// <summary>
    /// Getter of listTempTiles
    /// </summary>
    /// <returns></returns>
    public List<Tile> GetTileList() {
        return listGridTiles;
    }

    /// <summary>
    /// Clear the current tiles of the scene and clear the list of spawned tiles
    /// </summary>
    [ContextMenu("Clear Map")]
    public void ClearMapOfTiles() {
        if (listGridTiles != null) { 
            for (int i = listGridTiles.Count - 1; i > -1; i--) {
                DestroyImmediate(listGridTiles[i].gameObject); // Destroy function used in Editor mode
            }

            listGridTiles.Clear();
        }
    }

    /// <summary>
    /// Create the map of tiles
    /// </summary>
    [ContextMenu("Create Tiles")]
    public void CreateTiles() {
        ClearMapOfTiles();

        grid = new int[intMapSizeX, intMapSizeZ];

        currentMapSizeX = intMapSizeX;
        currentMapSizeZ = intMapSizeZ;

        tileLocationMap = new Tile[intMapSizeX, intMapSizeZ];

        // Tiles location maps initialization of the 2d Array
        // The information will be added inside when creating the tile object
        for (int x = 0; x < intMapSizeX; x++) { 
            for (int z = 0; z < intMapSizeZ; z++) {

                if ( (x + z) % 2 == 0 ) {
                    grid[x, z] = 0;
                } else {
                    grid[x, z] = 1;
                }

                SpawnTile(x, z);
            }
        }

        LinkTileNeighbors();
    }

    /// <summary>
    /// Spawn a tile game object in the assigned position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void SpawnTile(int x, int z) {

        // Spawn and reposition the tile
        GameObject tileGO = Instantiate(listPrefabTiles[ grid[x, z] ], transform);
        tileGO.transform.position = new Vector3(x, 0, z);
        tileGO.name = "Tile (" + x + "," + z + ")";

        // Set the location variables of the tile
        Tile singleTile = tileGO.GetComponent<Tile>();
        singleTile.SetLocation(x, z);
        singleTile.gridManager = this;

        // Add tile to lists
        listGridTiles.Add(singleTile);
        AddTileToMap(singleTile, x, z);
    }

    /// <summary>
    /// Add tile location to the map of tile locations
    /// </summary>
    /// <param name="singleTile"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void AddTileToMap(Tile singleTile, int x, int z) {
        tileLocationMap[x, z] = singleTile;
    }

    #endregion

    #region LINK_NEIGHBORS

    /// <summary>
    /// Link neighbor tiles to each tile of the map
    /// </summary>
    [ContextMenu("Link Neighbors")]
    public void LinkTileNeighbors() {
        listGridTiles.RemoveAll(x => x == null);

        if (tileLocationMap != null) {

            for (int x = 0; x < tileLocationMap.GetLength(0); x++)  {
                for (int z = 0; z < tileLocationMap.GetLength(1); z++) {
                    if (tileLocationMap[x, z]) { 
                        tileLocationMap[x, z].listNeighbors.Clear();
                        LinkVerAndHorNeighbors(x, z);
                        LinkDiagonalNeighbors(x, z);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Remove null entries from neighbor lists
    /// </summary>
    [ContextMenu("Remove Empty Neighbors")]
    public void RemoveEmptyNeighbors() {
        listGridTiles.RemoveAll(x => x == null);

        foreach (var tile in listGridTiles) {
            tile.listNeighbors.RemoveAll(x => x == null);
        }
    }

    /// <summary>
    /// Reset the values of tile coordinates based on their locations
    /// </summary>
    [ContextMenu("Reset Coordinates")]
    public void ResetCoordinates() { 
        foreach (var tile in listGridTiles) {
            int x = (int)tile.gameObject.transform.position.x;
            int z = (int)tile.gameObject.transform.position.z;

            tile.SetLocation(x, z);
            tile.gameObject.name = "Tile (" + x + "," + z + ")";
        }
    }

    /// <summary>
    /// Link vertical and horizontal neighbors
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    void LinkVerAndHorNeighbors(int x, int z) {
        if (x > 0 && tileLocationMap[x - 1, z]) { // Left neighbor
            tileLocationMap[x, z].listNeighbors.Add(tileLocationMap[x - 1, z]);
        }

        if (x < currentMapSizeX - 1 && tileLocationMap[x + 1, z]) { // Right neighbor
            tileLocationMap[x, z].listNeighbors.Add(tileLocationMap[x + 1, z]);
        }

        if (z > 0 && tileLocationMap[x, z - 1]) { // Bottom neighbor
            tileLocationMap[x, z].listNeighbors.Add(tileLocationMap[x, z - 1]);
        }

        if (z < currentMapSizeZ - 1 && tileLocationMap[x, z + 1]) { // Top neighbor
            tileLocationMap[x, z].listNeighbors.Add(tileLocationMap[x, z + 1]);
        }
    }

    /// <summary>
    /// Link diagonalNeighbors
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    void LinkDiagonalNeighbors(int x, int z) { 
        if ( (z < currentMapSizeZ - 1) && (x > 0) ) { // Top Left
            tileLocationMap[x, z].listNeighbors.Add(tileLocationMap[x - 1, z + 1]);
        }
                    
        if ( (z < currentMapSizeZ - 1) && (x < currentMapSizeX - 1) ) {  // Top Right
            tileLocationMap[x, z].listNeighbors.Add(tileLocationMap[x + 1, z + 1]);
        }
                    
        if ( (z > 0) && (x > 0) ) { // Bottom Left
            tileLocationMap[x, z].listNeighbors.Add(tileLocationMap[x - 1, z - 1]);
        }
                    
        if ( (z > 0) && (x < currentMapSizeX - 1) ) { // Bottom Right
            tileLocationMap[x, z].listNeighbors.Add(tileLocationMap[x + 1, z - 1]);
        }
    }

    #endregion

    //#region EQUALITY_OVERLOAD
    //public override bool Equals(object other) {
    //    return Equals(other as GridManager);
    //}

    //public bool Equals(GridManager grid) {
    //    if (!grid) { return false; }

    //    //return cameraHolder.position == grid.cameraHolder.position;
    //    return listGridTiles == grid.listGridTiles;
    //}

    //public override int GetHashCode() {
    //    return base.GetHashCode();
    //}

    //#endregion

}
