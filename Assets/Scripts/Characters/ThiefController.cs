using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class ThiefController : Character {

    /* Movement */
    public int intRange = 2;

    /* Path Control */
    bool isSelected;
    List<Tile> listTargetTiles = new List<Tile>();
    [SerializeField]
    List<Tile> listPathTiles = new List<Tile>();

    /* Item Control */
    public bool HasTreasure { get; set; } = false;
    public GameObject treasureHolder;

    /* Camera */
    public Camera mainCamera;

    private void Start() {
        isSelected = false;
        IsMoving = false;
        HasTreasure = false;
        SetStartingTile();
    }

    private void Update() {
        ControlMouseOverTiles();
    }

    #region MOVE_PLAYER

    /// <summary>
    /// Move the player to given tile
    /// </summary>
    /// <param name="nextTile"></param>
    protected override void MoveToTile(ref Tile nextTile) {
        // Calculate position
        Vector3 target = new Vector3(nextTile.transform.position.x, transform.position.y, nextTile.transform.position.z);
        Vector3 lookRotation = target - transform.position;

        // Move to tile
        transform.DOMove(target, stepTime).SetEase(Ease.OutCubic).OnComplete(UpdateTile(ref nextTile)).OnComplete(StartWaitOnTile);
        transform.DORotateQuaternion(Quaternion.LookRotation(lookRotation), 0.3f);
    }

    /// <summary>
    /// Callback for a DoTween function to update the current Tile of the thief
    /// </summary>
    /// <param name="nextTile"></param>
    /// <returns></returns>
    TweenCallback UpdateTile(ref Tile nextTile) {
        currentTile = nextTile;
        return null;
    }

    /// <summary>
    /// Have the Thief wait on the tile for a with before continuing the path
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator WaitOnTile() {
        yield return StartCoroutine(base.WaitOnTile());
        MoveOnPath();
    }

    /// <summary>
    /// Start the WaitOnTile coroutine
    /// </summary>
    void StartWaitOnTile() {
        StartCoroutine(WaitOnTile());
    }

    /// <summary>
    /// Move the player through the path of tiles and destroy the list of path tiles as they go
    /// </summary>
    public override void MoveOnPath() {
        TurnManager turnManager = TurnManager.instance;

        // Touched a cube or ended the level
        if (turnManager.HandleNewTile()) {
            IsMoving = false;
            return;
        }

        // Path is over
        if (listPathTiles.Count < 1) {
            IsMoving = false;
            turnManager.DecreaseMovementCount();
            return;
        }

        IsMoving = true;
        Tile nextTile = listPathTiles[0];
        MoveToTile(ref nextTile);
        listPathTiles.RemoveAt(0);
    }

    #endregion

    #region PLAYER_MOUSE_INPUT

    /// <summary>
    /// Event to highlight the tiles in range of the player if they are not moving
    /// </summary>
    private void OnMouseDown() {
        if (!IsMoving && TurnManager.instance.CanMove) { 
            isSelected = true;
            ClearPath();
            HighlightTargetTiles();
        }
    }

    /// <summary>
    /// Build a list of tiles with the path the player will follow
    /// </summary>
    void ControlMouseOverTiles() {
        // The player needs to have been selected
        if (isSelected && TurnManager.instance.CanMove) {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100.0f)) {
                AddTilesToPathList(ref hit);
            }

            // When the mouse is let go, refresh all tiles
            if (Input.GetMouseButtonUp(0)) {
                isSelected = false;
                if (listPathTiles.Count < 1) {
                    TurnTargetTilesOff();
                }
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
            tile.tileHighlighter.ChangeColorToThiefRange();
            tile.tileHighlighter.TurnHighlighterOn();
        }
    }

    /// <summary>
    /// Turn the highlighted tiles back to their original colors
    /// </summary>
    public void TurnTargetTilesOff() { 
        foreach (var tile in listTargetTiles) {
            tile.tileHighlighter.TurnHighlighterOff();
        }

        listTargetTiles.Clear();
    }

    /// <summary>
    /// As the user drags the mouse over the target tiles, ass them to the path
    /// </summary>
    /// <param name="hit"></param>
    void AddTilesToPathList(ref RaycastHit hit) {
        Tile pathTile = hit.transform.GetComponent<Tile>();

        // Tile needs to be within the target list
        if (pathTile) {
                    
            // Remove tiles from the path if you hover back
            if (listPathTiles.Contains(pathTile)) {
                while (!pathTile.Equals(listPathTiles[listPathTiles.Count - 1])) {
                    listPathTiles.Last().tileHighlighter.ChangeColorToThiefRange();
                    listPathTiles.RemoveAt(listPathTiles.Count - 1);
                }
                    
            // Add tile to the path if it is still within range making sure it is a neighbor tile
            } else if (listTargetTiles.Contains(pathTile) && listPathTiles.Count < intRange) { 
                
                if ( (listPathTiles.Count < 1 && currentTile.HasNeighbor(pathTile)) || (listPathTiles.Count > 0 && listPathTiles.Last().HasNeighbor(pathTile)) ) { 
                    listPathTiles.Add(pathTile);
                    pathTile.tileHighlighter.ChangeColorToThiefPath();
                }
            
            // Remove all tiles from the path if the player hovers back to the start
            } else if (pathTile.Equals(currentTile)) { 
                foreach (var tile in listPathTiles) {
                    tile.tileHighlighter.ChangeColorToThiefRange();
                }
                listPathTiles.Clear();
            }
                    
        }
    }

    /// <summary>
    /// Clear the path of tiles
    /// </summary>
    public void ClearPath() {
        listPathTiles.Clear();
    }

    #endregion

}
