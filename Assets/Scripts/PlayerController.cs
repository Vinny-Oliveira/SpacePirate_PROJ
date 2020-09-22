using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour {

    public float fltSpeed = 5;
    public int intRange = 2;

    bool isSelected;
    public List<Tile> listTargetTiles = new List<Tile>();

    public Tile currentTile;
    public GridManager currentGrid;

    private void Start() {
        isSelected = false;
        SetStartingTile();
    }

    /// <summary>
    /// Move player to tile with coordinates x and z
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public void MoveToTile(float x, float z) {
        Vector3 target = new Vector3(x, transform.position.y, z);
        transform.position = target;
    }

    /// <summary>
    /// Move player to tile with coordinate target
    /// </summary>
    /// <param name="target"></param>
    public void MoveToTile(Vector3 target) {
        MoveToTile(target.x, target.z);
    }

    /// <summary>
    /// Move the player to given tile
    /// </summary>
    /// <param name="tile"></param>
    public void MoveToTile(Tile tile) {
        currentTile = tile;
        MoveToTile(tile.GetLocation());
    }

    /// <summary>
    /// Set the value of the tile the player starts on and move them there
    /// </summary>
    void SetStartingTile() { 
        if (currentGrid == null) {
            Debug.Log("ERROR: Assign a Grid!");
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }

        if (currentTile == null) { 
            if (currentGrid.GetTileList().Count < 1) {
                Debug.Log("ERROR: Create a Tile Map!");
                UnityEditor.EditorApplication.isPlaying = false;
                return;
            }

            currentTile = currentGrid.GetTileList()[0].GetComponent<Tile>();
        }

        MoveToTile(currentTile);
    }

    private void OnMouseDown() {
        isSelected = !isSelected;

        // Highlight the tiles within the range
        if (isSelected) {
            HighlightTargetTiles();
        } else {
            TurnTargetTilesOff();
        }
    }

    void HighlightTargetTiles() { 
        listTargetTiles = currentTile.listNeighbors;
        List<Tile> listTempNeighbors = listTargetTiles;

        // Check tiles in range
        for (int i = 1; i < intRange; i++) {
            List<Tile> newNeighbors = new List<Tile>();
            
            // Add outer layer of neighbors
            foreach (var tile in listTempNeighbors) {
                List<Tile> tempNeighbors = tile.listNeighbors;
                newNeighbors = newNeighbors.Union(tempNeighbors).ToList();
            }

            newNeighbors.Remove(currentTile);
            listTargetTiles = listTargetTiles.Union(newNeighbors).ToList();
            listTempNeighbors = newNeighbors;
        }

        // Highlight each tile
        foreach (var tile in listTargetTiles) {
            tile.HighlightTile();
        }
    }

    void TurnTargetTilesOff() { 
        foreach (var tile in listTargetTiles) {
            tile.ResetMaterial();
        }
    }

}
