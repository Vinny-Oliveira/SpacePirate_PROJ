using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class PlayerController : Character {

    /* Movement */
    public float fltSpeed = 5;
    public int intRange = 2;
    public float stepTime = 0.5f;

    /* Path Control */
    bool isSelected;
    bool isMoving;
    List<Tile> listTargetTiles = new List<Tile>();
    List<Tile> listPathTiles = new List<Tile>();

    /* Item Control */
    public bool hasTreasure = false;

    /* Camera */
    public Camera mainCamera;

    private void Start() {
        isSelected = false;
        isMoving = false;
        hasTreasure = false;
        SetStartingTile();
    }

    private void Update() {
        ControlMouseOverTiles();
    }

    #region MOVE_PLAYER

    /// <summary>
    /// Move the player to given tile
    /// </summary>
    /// <param name="tile"></param>
    protected override void MoveToTile(Tile tile) {
        currentTile = tile;
        Vector3 target = new Vector3(tile.GetLocation().x, transform.position.y, tile.GetLocation().z);
        Vector3 lookRotation = target - transform.position;
        
        transform.DOMove(target, stepTime).OnComplete(MoveOnPath);
        transform.DORotateQuaternion(Quaternion.LookRotation(lookRotation), 0.3f);
    }

    /// <summary>
    /// Move the player through the path of tiles and destroy the list of path tiles as they go
    /// </summary>
    public override void MoveOnPath() {
        if (listPathTiles.Count < 1) {
            isMoving = false;
            return;
        }

        isMoving = true;
        MoveToTile(listPathTiles[0]);
        listPathTiles.RemoveAt(0);
    }

    #endregion

    #region PLAYER_MOUSE_INPUT

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

    #endregion

    #region PATH_OF_TILES

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

    void AddTilesToPathList(ref RaycastHit hit) {
        Tile pathTile = hit.transform.GetComponent<Tile>();

        // Tile needs to be within the target list
        if (pathTile) {
                    
            // Remove tiles from the path if you hover back
            if (listPathTiles.Contains(pathTile)) {
                while (!pathTile.Equals(listPathTiles[listPathTiles.Count - 1])) {
                    listPathTiles.Last().HighlightTile();
                    listPathTiles.RemoveAt(listPathTiles.Count - 1);
                }
                    
            // Add tile to the path if it is still within range making sure it is a neighbor tile
            } else if (listTargetTiles.Contains(pathTile) && listPathTiles.Count < intRange) { 
                
                if ( (listPathTiles.Count < 1 && currentTile.HasNeighbor(pathTile)) || (listPathTiles.Count > 0 && listPathTiles.Last().HasNeighbor(pathTile)) ) { 
                    listPathTiles.Add(pathTile);
                    pathTile.PathMatTile();
                }
            
            // Remove all tiles from the path if the player hovers back to the start
            } else if (pathTile.Equals(currentTile)) { 
                foreach (var tile in listPathTiles) {
                    tile.HighlightTile();
                }
                listPathTiles.Clear();
            }
                    
        }
    }

    #endregion

}
