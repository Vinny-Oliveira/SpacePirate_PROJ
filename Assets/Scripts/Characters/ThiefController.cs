using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class ThiefController : Character {

    [Header("Movement Range")]
    [SerializeField]
    int intRange = 2;

    /* Path Control */
    Tile targetTile;
    List<Tile> listTargetTiles = new List<Tile>();
    List<Tile> listPathTiles = new List<Tile>();

    public Tile LastPathTile { 
        get {
            if (listPathTiles.Count > 0) {
                return listPathTiles.Last();
            } else {
                return null;
            }
        }
    }

    /* Item Control */
    public bool HasTreasure { get; set; } = false;
    List<Keycard> listKeycards = new List<Keycard>();

    [Header("Camera & UI")]
    public Camera mainCamera;
    public TMPro.TextMeshProUGUI tmpMoveCount;
    public Color maxRangeColor;

    //private void Update() {
    //    ControlMouseOverTiles();
    //}

    #region STARTUP_FUNCTIONS

    /// <summary>
    /// Startup for the Thief
    /// </summary>
    public void SetupThief() {
        IsMoving = false;
        CanStep = true;
        HasTreasure = false;
        SetStartingTile();
        RepositionCamera();
        DisplayMoveCounter();
        StartNewPath();
    }

    #endregion

    #region MOVE_PLAYER

    /// <summary>
    /// Move the player to given tile
    /// </summary>
    /// <param name="nextTile"></param>
    protected override void MoveToTile(ref Tile nextTile) {
        // Calculate position
        Vector3 target = new Vector3(nextTile.transform.position.x, transform.position.y, nextTile.transform.position.z);
        Vector3 lookRotation = target - transform.position;
        targetTile = nextTile;

        // Move to tile
        transform.DOMove(target, stepTime).SetEase(Ease.OutQuart).OnComplete(UpdateTile);
        transform.DORotateQuaternion(Quaternion.LookRotation(lookRotation), 0.3f);
    }

    /// <summary>
    /// Update the current tile of the Thief
    /// </summary>
    void UpdateTile() { 
        if (currentGrid != targetTile.gridManager) {
            currentGrid = targetTile.gridManager;
            RepositionCamera();
        }

        currentTile = targetTile;

        StartCoroutine(WaitOnTile());
    }

    /// <summary>
    /// Have the Thief wait on the tile for a while before continuing the path, and check what is on the tile
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator WaitOnTile() {
        yield return StartCoroutine(base.WaitOnTile());
        CanStep = true;
        yield return new WaitUntil(() => TurnManager.instance.CanCharactersStep());

        TurnManager turnManager = TurnManager.instance;

        // Caught by a cube or ended the level
        if (turnManager.IsThiefCaught() || turnManager.HasThiefBeatenLevel()) {
            IsMoving = false;
            yield break;
        }

        // Check if touching treasure, keycard, or door
        PickUpKeycard();
        turnManager.CheckForTreasure();
        OpenNeighborDoors();
        MoveOnPath();
    }

    /// <summary>
    /// Move the player through the path of tiles and destroy the list of path tiles as they go
    /// </summary>
    public override void MoveOnPath() {
        TurnManager turnManager = TurnManager.instance;
        
        // Path is over
        if (listPathTiles.Count < 1) {
            IsMoving = false;
            turnManager.DecreaseMovementCount();
            DisplayMoveCounter();
            return;
        }

        // Continue the path
        IsMoving = true;
        CanStep = false;
        Tile nextTile = listPathTiles[0];
        MoveToTile(ref nextTile);

        // Deactivate chaders and update counter
        listPathTiles.RemoveAt(0);
        if (!listPathTiles.Contains(nextTile)) { 
            nextTile.tileHighlighter.TurnHighlighterOff();
        }
        DisplayMoveCounter();
    }

    /// <summary>
    /// Display how many moves the Thief still has
    /// </summary>
    public void DisplayMoveCounter() {
        if (listPathTiles.Count < intRange) {
            tmpMoveCount.color = Color.white;
        } else {
            tmpMoveCount.color = maxRangeColor;
        }

        tmpMoveCount.text = listPathTiles.Count.ToString() + "/" + intRange;
    }

    #endregion

    //#region PLAYER_MOUSE_INPUT

    /////// <summary>
    /////// Event to highlight the tiles in range of the player if they are not moving
    /////// </summary>
    ////private void OnMouseDown() {
    ////    if (!IsMoving && TurnManager.instance.CanClick) { 
    ////        isSelected = true;
    ////        ClearPath();
    ////        HighlightTargetTiles();
    ////    }
    ////}

    /////// <summary>
    /////// Build a list of tiles with the path the player will follow
    /////// </summary>
    ////void ControlMouseOverTiles() {
    ////    // The player needs to have been selected
    ////    if (isSelected && TurnManager.instance.CanClick) {
    ////        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
    ////        RaycastHit hit;

    ////        if (Physics.Raycast(ray, out hit, 100.0f)) {
    ////            AddTilesToPathList(ref hit);
    ////        }

    ////        // When the mouse is let go, refresh all tiles
    ////        if (Input.GetMouseButtonUp(0)) {
    ////            isSelected = false;
    ////            TurnTargetTilesOff();

    ////            TurnManager.instance.HighlightCubesFieldsOfView();
    ////            HighlightPathTiles();
    ////            return;
    ////        }

    ////    }
    ////}

    //#endregion

    #region TARGET_TILES

    /// <summary>
    /// Turn the highlighted tiles back to their original colors
    /// </summary>
    public void TurnTargetTilesOff() { 
        foreach (var tile in listTargetTiles) {
            if (!listPathTiles.Contains(tile)) { 
                tile.tileHighlighter.TurnHighlighterOff();
            }
        }

        listTargetTiles.Clear();
    }

    /// <summary>
    /// Check if a tile is contained in the target list
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool IsTargetTile(Tile tile) {
        return listTargetTiles.Contains(tile);
    }

    /// <summary>
    /// Add a tile to the list of targets
    /// </summary>
    /// <param name="tile"></param>
    public void AddTileToTargets(Tile tile) {
        listTargetTiles.Add(tile);
    }

    #endregion

    #region PATH_OF_TILES

    /// <summary>
    /// Highlight the tiles of the new path
    /// </summary>
    public void StartNewPath() {
        ClearPath();
        currentTile.HighlightNeighbors();
    }

    /// <summary>
    /// Check if tile can be added to the path
    /// </summary>
    /// <returns></returns>
    public bool CanAddToPath() {
        return (listPathTiles.Count < intRange);
    }

    ///// <summary>
    ///// Highlight the next possible targets for the path, as long as they are walkable, not the current tile, and not already on the path
    ///// </summary>
    ///// <param name="lastPathTile"></param>
    //void HighlightTargetTiles() {
    //    TurnTargetTilesOff();

    //    if (listPathTiles.Count < intRange) {
    //        Tile lastTile = currentTile;

    //        // Use the last tile of the path to check for the next targets
    //        if (listPathTiles.Count > 0) {
    //            lastTile = listPathTiles.Last();
    //        }

    //        listTargetTiles.AddRange(lastTile.listNeighbors);
    //        listTargetTiles.RemoveAll(x => !x.IsWalkable() || x == currentTile || listPathTiles.Contains(x));

    //        // Highlight each tile
    //        foreach (var tile in listTargetTiles) {
    //            tile.tileHighlighter.ChangeColorToThiefRange();
    //            tile.tileHighlighter.TurnHighlighterOn();
    //        }
    //    }

    //}

    /// <summary>
    /// Highlight the tiles of the Thief's path
    /// </summary>
    public void HighlightPathTiles() { 
        foreach (var tile in listPathTiles) {
            tile.tileHighlighter.ChangeColorToThiefPath();
            tile.tileHighlighter.TurnHighlighterOn();
        }
    }

    /// <summary>
    /// Add a tile to the path
    /// </summary>
    /// <param name="tile"></param>
    public void AddTileToPath(Tile tile) {
        listPathTiles.Add(tile);
    }

    ///// <summary>
    ///// As the user drags the mouse over the target tiles, add them to the path
    ///// </summary>
    ///// <param name="hit"></param>
    //void AddTilesToPathList(ref RaycastHit hit) {
    //    Tile pathTile = hit.transform.GetComponent<Tile>();

    //    // Tile needs to be within the target list
    //    if (pathTile) {
                    
    //        // Remove tiles from the path if you hover back
    //        if (listPathTiles.Contains(pathTile)) {
    //            while (!pathTile.Equals(listPathTiles[listPathTiles.Count - 1])) {
    //                listPathTiles.Last().tileHighlighter.TurnHighlighterOff();//.ChangeColorToThiefRange();
    //                listPathTiles.RemoveAt(listPathTiles.Count - 1);
    //            }
    //            HighlightTargetTiles();

    //        // Add tile to the path if it is still within range making sure it is a neighbor tile, and highlight new targets
    //        } else if (listTargetTiles.Contains(pathTile) && listPathTiles.Count < intRange) { 
                
    //            if ( (listPathTiles.Count < 1 && currentTile.HasNeighbor(pathTile)) || (listPathTiles.Count > 0 && listPathTiles.Last().HasNeighbor(pathTile)) ) { 
    //                listPathTiles.Add(pathTile);
    //                HighlightTargetTiles(); // Highlight new targets
    //                pathTile.tileHighlighter.ChangeColorToThiefPath();
    //                pathTile.tileHighlighter.TurnHighlighterOn();
    //            }
            
    //        // Remove all tiles from the path if the player hovers back to the start
    //        } else if (pathTile.Equals(currentTile)) { 
    //            foreach (var tile in listPathTiles) {
    //                tile.tileHighlighter.TurnHighlighterOff();
    //            }
    //            ClearPath();
    //            HighlightTargetTiles();
    //        }

    //        // Update move counter
    //        DisplayMoveCounter();
    //    }
    //}

    /// <summary>
    /// Clear the path of tiles
    /// </summary>
    public void ClearPath() {
        foreach (var tile in listPathTiles) {
            tile.tileHighlighter.TurnHighlighterOff();
        }
        listPathTiles.Clear();
        DisplayMoveCounter();
    }

    /// <summary>
    /// Check if a tile is on the path
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool IsTileOnPath(Tile tile) {
        return listPathTiles.Contains(tile);
    }

    /// <summary>
    /// Check if a given tile is the last of the path
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool IsTileLastOfPath(Tile tile) {
        return listPathTiles.Last().Equals(tile);
    }

    /// <summary>
    /// Remove the last tile of the path
    /// </summary>
    /// <param name="tile"></param>
    public void RemoveLastTileFromPath() {
        listPathTiles.RemoveAt(listPathTiles.Count - 1);
    }

    #endregion

    #region CAMERA_CONTROL

    /// <summary>
    /// Position the camera accordingly depending on the Thief's grid
    /// </summary>
    void RepositionCamera() {
        mainCamera.transform.DOMove(currentGrid.cameraHolder.position, 0.9f);
    }

    #endregion

    #region KEYCARDS_AND_DOORS

    /// <summary>
    /// Add a keycard to the list of keycards
    /// </summary>
    /// <param name="keycard"></param>
    public void PickUpKeycard() {
        TurnManager turnManager = TurnManager.instance;
        Keycard keycard = turnManager.IsThefTouchingKeycard();

        if (keycard) { 
            listKeycards.Add(keycard);
            keycard.gameObject.SetActive(false);
            turnManager.keycard_Image.SetActive(true);
        }
    }

    /// <summary>
    /// CHeck if the Thief if close to doors that can be opened and open them
    /// </summary>
    void OpenNeighborDoors() {
        List<Tile> doorTiles = currentTile.listNeighbors.FindAll(x => x.tileType == ETileType.DOOR);

        foreach (var doorTile in doorTiles) {
            ECardType doorType = doorTile.door.cardType;

            if (listKeycards.Find(x => x.cardType == doorType)) {
                doorTile.OpenDoor();
            } else {
                TurnManager.instance.ThiefNeedsKeycard();
            }
        }
    }

    #endregion

}
