using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {

    //public static GridManager instance;

    public int[,] grid = new int[0,0]; // Grid with positions

    public PlayerController player;

    public List<GameObject> listPrefabTiles; // List with all the tile prefabs
    public List<GameObject> listTempTiles; // List of tiles spawned in the scene

    public TileLocation[,] tileLocationMap;
    private int currentMapSizeX;
    private int currentMapSizeZ;

    // Grid size
    public int intMapSizeX = 10;
    public int intMapSizeZ = 10;

    //[Space(10)]
    //[Header("Saved Content")]
    //public SavedGridSO savedGridMap;

    //private void Awake() {
    //    instance = this;
    //}

    /// <summary>
    /// Clear the current tiles of the scene and clear the list of spawned tiles
    /// </summary>
    [ContextMenu("Clear Map")]
    public void ClearMapOfTiles() {
        if (listTempTiles != null) { 
            for (int i = listTempTiles.Count - 1; i > -1; i--) {
                DestroyImmediate(listTempTiles[i]); // Destroy function used in Editor mode
            }

            listTempTiles.Clear();
        }
    }

    /// <summary>
    /// Create the map of tiles
    /// </summary>
    [ContextMenu("Create Tiles")]
    public void CreateTiles() {
        ClearMapOfTiles();

        //savedGridMap.listTilesInfo.Clear();
        //savedGridMap.intMapSizeX = intMapSizeX;
        //savedGridMap.intMapSizeZ = intMapSizeZ;

        grid = new int[intMapSizeX, intMapSizeZ];

        currentMapSizeX = intMapSizeX;
        currentMapSizeZ = intMapSizeZ;

        tileLocationMap = new TileLocation[intMapSizeX, intMapSizeZ];

        // Tiles location maps initialization of the 2d Array
        // The information will be added inside when creating the tile object
        for (int x = 0; x < intMapSizeX; x++) { 
            for (int z = 0; z < intMapSizeZ; z++) {
                
                //if (IsEdge(x, z)) {
                //    grid[x, z] = 0;
                //} else {
                //    grid[x, z] = Random.Range(1, listPrefabTiles.Count);
                //}

                if ( (x + z) % 2 == 0 ) {
                    grid[x, z] = 0;
                } else {
                    grid[x, z] = 1;
                }

                SpawnTile(x, z);
            }
        }

        LinkMyNeighbors();
    }

    /// <summary>
    /// Spawn a tile game object in the assigned position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void SpawnTile(int x, int z) {

        // Spawn and reposition the tile
        GameObject singleTile = Instantiate(listPrefabTiles[ grid[x, z] ], this.transform);
        singleTile.transform.position = new Vector3(x, 0, z);

        // Set the location variables of the tile
        TileLocation singleTileLocation = singleTile.GetComponent<TileLocation>();
        singleTileLocation.SetLocation(x, z, (TileType)(grid[x, z]));
        singleTileLocation.gridManager = this;
        singleTile.name = "Tile (" + x + "," + z + ")";

        // Add tile to lists
        listTempTiles.Add(singleTile);
        AddTileToMap(singleTile.GetComponent<TileLocation>(), x, z);
    }

    /// <summary>
    /// Add tile location to the map of tile locations
    /// </summary>
    /// <param name="singleTile"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void AddTileToMap(TileLocation singleTile, int x, int z) {
        tileLocationMap[x, z] = singleTile;
    }

    /// <summary>
    /// Link neighbor tiles to each tile of the map
    /// </summary>
    [ContextMenu("Link My Neighbors")]
    public void LinkMyNeighbors() { 
        if (tileLocationMap != null) {

            for (int x = 0; x < tileLocationMap.GetLength(0); x++)  {
                for (int z = 0; z < tileLocationMap.GetLength(1); z++) {

                    if (x > 0) { // Left neighbor
                        tileLocationMap[x, z].listNeighbors.Add(tileLocationMap[x - 1, z]);
                    } 

                    if (x < currentMapSizeX - 1) { // Right neighbor
                        tileLocationMap[x, z].listNeighbors.Add(tileLocationMap[x + 1, z]);
                    }

                    if (z > 0) { // Bottom neighbor
                        tileLocationMap[x, z].listNeighbors.Add(tileLocationMap[x, z - 1]);
                    }

                    if (z < currentMapSizeZ - 1) { // Top neighbor
                        tileLocationMap[x, z].listNeighbors.Add(tileLocationMap[x, z + 1]);
                    }

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
            }
        }
    }


    ///// <summary>
    ///// Check if the position where the tile is spawned is an edge
    ///// </summary>
    ///// <param name="x"></param>
    ///// <param name="z"></param>
    ///// <returns></returns>
    //public bool IsEdge(int x, int z) { 
    //    if ((x == 0) || (x == intMapSizeX - 1) || (z == 0) || (z == intMapSizeZ - 1)) {
    //        return true;
    //    }

    //    return false;
    //}

    ///// <summary>
    ///// Print the location of a Vector3
    ///// </summary>
    ///// <param name="tileLocation"></param>
    //public void TellMyLocation(Vector3 tileLocation) {
    //    print(tileLocation);
    //}

    ///// <summary>
    ///// Utility to convert an int to an enum
    ///// </summary>
    ///// <param name="value"></param>
    ///// <returns></returns>
    //public TileType ConvertToType(int value) {
    //    TileType returnThisTile = new TileType();
    //    switch (value) {

    //        case 0:
    //            returnThisTile = TileType.Edge;
    //            break;
    //        case 1:
    //            returnThisTile = TileType.Walkable;
    //            break;
    //        case 2:
    //            returnThisTile = TileType.Obstacle;
    //            break;
    //        default:
    //            break;
    //    }

    //    return returnThisTile;
    //}

    ///// <summary>
    ///// Utility to convert an enum to an int
    ///// </summary>
    ///// <param name="type"></param>
    ///// <returns></returns>
    //public int ConvertToInt(TileType type) {
    //    int returnThisTile = 0;
    //    switch (type) {

    //        case TileType.Edge:
    //            returnThisTile = 0;
    //            break;
    //        case TileType.Walkable:
    //            returnThisTile = 1;
    //            break;
    //        case TileType.Obstacle:
    //            returnThisTile = 2;
    //            break;
    //        default:
    //            break;
    //    }

    //    return returnThisTile;
    //}
}
