using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class PlayerController : MonoBehaviour {

    /* Movement */
    public float fltSpeed = 5;
    public int intRange = 2;

    /* Path Control */
    bool isSelected;
    bool isMoving;
    List<Tile> listTargetTiles = new List<Tile>();
    List<Tile> listPathTiles = new List<Tile>();

    /* Game Objects */
    public Tile currentTile;
    public GridManager currentGrid;
    public Camera mainCamera;

    private void Start() {
        isSelected = false;
        isMoving = false;
        SetStartingTile();
    }

    private void Update() {
        ControlMouseOverTiles();
    }

    /// <summary>
    /// Move the player to given tile
    /// </summary>
    /// <param name="tile"></param>
    public void MoveToTile(Tile tile) {
        currentTile = tile;
        Vector3 target = new Vector3(tile.GetLocation().x, transform.position.y, tile.GetLocation().z);
        transform.DOMove(target, 1f).OnComplete(MoveOnPath);
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
    /// Event to highlight the tiles in range of the player if they are not moving
    /// </summary>
    private void OnMouseDown() {
        if (!isMoving) { 
            isSelected = true;
            HighlightTargetTiles();
        }
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
    /// Build a list of tiles with the path the player will follow
    /// </summary>
    void ControlMouseOverTiles() {
        // The player needs to have been selected
        if (isSelected) {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100.0f)) {
                AddTilesToPathList(ref hit);
            }

            // When the mouse is let go, refresh all tiles
            if (Input.GetMouseButtonUp(0)) {
                isSelected = false;
                TurnTargetTilesOff();
                MoveOnPath();
                return;
            }

        }
    }

    void AddTilesToPathList(ref RaycastHit hit) {
        Tile pathTile = hit.transform.GetComponent<Tile>();

        // Tile needs to be within the target list
        if (pathTile && listTargetTiles.Contains(pathTile)) {
                    
            // Remove tiles from the path if you hover back
            if (listPathTiles.Contains(pathTile)) {
                while (!pathTile.Equals(listPathTiles[listPathTiles.Count - 1])) {
                    listPathTiles.Last().HighlightTile();
                    listPathTiles.RemoveAt(listPathTiles.Count - 1);
                }
                    
            // Add tile to the path if it is still within range making sure it is a neighbor tile
            } else if (listPathTiles.Count < intRange) { 
                
                if ( (listPathTiles.Count < 1 && currentTile.HasNeighbor(pathTile)) || (listPathTiles.Count > 0 && listPathTiles.Last().HasNeighbor(pathTile)) ) { 
                    listPathTiles.Add(pathTile);
                    pathTile.PathMatTile();
                }
                
            }
                    
        }
    }

    /// <summary>
    /// Move the player through the path of tiles and destroy the list of path tiles as they go
    /// </summary>
    void MoveOnPath() {
        if (listPathTiles.Count < 1) {
            isMoving = false;
            return;
        }

        isMoving = true;
        MoveToTile(listPathTiles[0]);
        listPathTiles.RemoveAt(0);
    }
}
