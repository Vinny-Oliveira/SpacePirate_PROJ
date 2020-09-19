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

        GameObject singleTile = Instantiate(listPrefabTiles[ grid[x, z] ], this.transform); // Spawn the tile
        singleTile.transform.position = new Vector3(x, 0, z); // Reposition the tile

        TileLocation singleTileLocation = singleTile.GetComponent<TileLocation>();
        singleTileLocation.SetLocation(x, z, (TileType)(grid[x, z])); // Set the location variables of the tile
        singleTileLocation.gridManager = this;

        listTempTiles.Add(singleTile);

        //TileInfo newSpawnedTile = new TileInfo();
        //newSpawnedTile.coordinates = new Vector3(x, 0, z);
        //newSpawnedTile.tileType = ConvertToType(grid[x, z]);
        //savedGridMap.listTilesInfo.Add(newSpawnedTile);

        AddTileToMap(singleTile.GetComponent<TileLocation>(), x, z);
    }

    ///// <summary>
    ///// Load a saved map of tiles
    ///// </summary>
    //[ContextMenu("Load Tiles")]
    //public void LoadTiles() 
    //{
    //    ClearMapOfTiles();

    //    // Load info only if info exists in the SO
    //    if (savedGridMap.listTilesInfo.Count > 0)
    //    {
    //        Debug.Log("Loading Map...");

    //        currentMapSizeX = savedGridMap.intMapSizeX;
    //        currentMapSizeZ = savedGridMap.intMapSizeZ;
    //        tileLocationMap = new TileLocation[currentMapSizeX, currentMapSizeZ];

    //        foreach (TileInfo singleTile in savedGridMap.listTilesInfo)
    //        {
    //            GameObject tile = Instantiate(listPrefabTiles[ConvertToInt(singleTile.tileType)]);
    //            tile.transform.position = singleTile.coordinates;
    //            tile.GetComponent<TileLocation>().tileInfo = singleTile;

    //            listTempTiles.Add(tile);

    //            AddTileToMap(tile.GetComponent<TileLocation>(), (int) singleTile.coordinates.x, (int) singleTile.coordinates.z);
    //        }
    //    }
    //}

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
            int total = 0;
            //print(tileLocationMap[0,0]);

            for (int x = 0; x < tileLocationMap.GetLength(0); x++)  {
                for (int z = 0; z < tileLocationMap.GetLength(1); z++) {

                    if (x > 0) { // Not on the edge
                        tileLocationMap[x, z].listNeighbors.Add(tileLocationMap[x - 1, z]);
                    } 

                    if (x < currentMapSizeX - 1) { // Not on the edge
                        tileLocationMap[x, z].listNeighbors.Add(tileLocationMap[x + 1, z]);
                    }

                    if (z > 0) { // Not on the edge
                        tileLocationMap[x, z].listNeighbors.Add(tileLocationMap[x, z - 1]);
                    }

                    if (z < currentMapSizeZ - 1) { // Not on the edge
                        tileLocationMap[x, z].listNeighbors.Add(tileLocationMap[x, z + 1]);
                    }

                    total++;
                }
            }
            Debug.Log("Total: " + total);
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
