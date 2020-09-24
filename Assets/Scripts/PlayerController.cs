using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour {

    public float fltSpeed = 5;
    public int intRange = 2;

    bool isSelected;
    List<Tile> listTargetTiles = new List<Tile>();
    Stack<Tile> stkPathTiles = new Stack<Tile>();

    public Tile currentTile;
    public GridManager currentGrid;
    public Camera mainCamera;

    private void Start() {
        isSelected = false;
        SetStartingTile();
    }

    private void Update() {
        MakeTilePath();
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


    /// <summary>
    /// Event to highlight the tiles in range of the player
    /// </summary>
    private void OnMouseDown() {
        //isSelected = !isSelected;
        isSelected = true;
        HighlightTargetTiles();

        //// Highlight the tiles within the range
        //if (isSelected) {
        //    HighlightTargetTiles();
        //} else {
        //    TurnTargetTilesOff();
        //}
    }

    /// <summary>
    /// Highlight the tiles in range of the player's movement
    /// </summary>
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

    /// <summary>
    /// Turn the highlighted tiles back to their original colors
    /// </summary>
    void TurnTargetTilesOff() { 
        foreach (var tile in listTargetTiles) {
            tile.ResetMaterial();
        }

        listTargetTiles.Clear();
    }

    /// <summary>
    /// Build a stack of tiles with the path the player will follow
    /// </summary>
    void MakeTilePath() {
        // The player needs to have been selected
        if (isSelected) {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100.0f)) {
                Tile pathTile = hit.transform.GetComponent<Tile>();
                // Tile needs to be within the target list
                if (pathTile && listTargetTiles.Contains(pathTile)) {
                    
                    // Remove tiles from the path if you hover back
                    if (stkPathTiles.Contains(pathTile)) {
                        while (!pathTile.Equals(stkPathTiles.Peek())) {
                            Tile poppedTile = stkPathTiles.Pop();
                            poppedTile.HighlightTile();
                        }
                    
                    // Add tile to the path if it is still within range
                    } else if ((stkPathTiles.Count < intRange)) { 
                        stkPathTiles.Push(pathTile);
                        pathTile.PathMatTile();
                    }
                    
                }
            }

            // When the mouse is let go, refresh all tiles
            if (Input.GetMouseButtonUp(0)) {
                isSelected = false;
                stkPathTiles.Clear();
                TurnTargetTilesOff();
                return;
            }

        }
    }
}
