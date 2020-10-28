using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class Thief : Character {

    [Header("Movement Range")]
    [SerializeField]
    int intRange = 2;

    [Header("Path Control")]
    Tile targetTile;
    List<Tile> listTargetTiles = new List<Tile>();
    List<Tile> listPathTiles = new List<Tile>();
    public GameObject ghostPrefab;
    public List<GameObject> listGhosts = new List<GameObject>();

    /// <summary>
    /// Last tile in the list of path tiles
    /// </summary>
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
    EMP_Device emp;

    [Header("Camera & UI")]
    public Camera mainCamera;
    public TMPro.TextMeshProUGUI tmpMoveCount;
    public Color maxRangeColor;
    public CounterFollow counterFollow;

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
        transform.DOMove(target, stepTime).SetEase(Ease.OutQuart).OnUpdate(counterFollow.UpdateCounterPosition).OnComplete(UpdateTile);
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
        PickUpEMP();
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
        RemoveGhostFromPath(nextTile);

        // Deactivate shaders and update counter
        listPathTiles.RemoveAt(0);
        if (!listPathTiles.Contains(nextTile)) { 
            nextTile.moveQuad.TurnHighlighterOff();
        }
        HighlightPathTiles();
        DisplayMoveCounter();
    }

    #endregion

    #region TARGET_TILES

    /// <summary>
    /// Turn the highlighted tiles back to their original colors
    /// </summary>
    public void TurnTargetTilesOff() { 
        foreach (var tile in listTargetTiles) {
            if (!listPathTiles.Contains(tile)) { 
                tile.moveQuad.TurnHighlighterOff();
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

    /// <summary>
    /// Highlight the tiles of the Thief's path
    /// </summary>
    public void HighlightPathTiles() { 
        foreach (var tile in listPathTiles) {
            //tile.moveQuad.ChangeColorToThiefPath();
            //tile.moveQuad.TurnHighlighterOn();
            tile.moveQuad.TurnHighlighterOff();
        }
    }

    /// <summary>
    /// Add a tile to the path
    /// </summary>
    /// <param name="tile"></param>
    public void AddTileToPath(Tile tile) {
        // Place ghost on the path
        if (listPathTiles.Count > 0) { 
            AddGhostToPath(tile, listPathTiles.Last());
        } else {
            AddGhostToPath(tile, currentTile);
        }

        // Add to path
        listPathTiles.Add(tile);
        tile.moveQuad.TurnHighlighterOff();
    }

    /// <summary>
    /// Clear the path of tiles
    /// </summary>
    public void ClearPath() {
        foreach (var tile in listPathTiles) {
            tile.moveQuad.TurnHighlighterOff();
            RemoveGhostFromPath(tile);
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
        return tile.Equals(LastPathTile);
    }

    /// <summary>
    /// Remove the last tile of the path
    /// </summary>
    /// <param name="tile"></param>
    public void RemoveLastTileFromPath() {
        RemoveGhostFromPath(listPathTiles.Last());
        listPathTiles.RemoveAt(listPathTiles.Count - 1);
    }

    /// <summary>
    /// Clear the cuurent path and start a new one from the current tile
    /// </summary>
    public void ResetPath() {
        if (TurnManager.instance.CanClick) { 
            TurnTargetTilesOff();
            ClearPath();
            StartNewPath();
        }
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

    #region CAMERA_CONTROL

    /// <summary>
    /// Position the camera accordingly depending on the Thief's grid
    /// </summary>
    void RepositionCamera() {
        mainCamera.transform.DOMove(currentGrid.cameraHolder.position, 0.9f).OnUpdate(counterFollow.UpdateCounterPosition);
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

    #region EMP_DEVICE

    /// <summary>
    /// Pick up the EMP device
    /// </summary>
    public void PickUpEMP() { 
        if (TurnManager.instance.IsThiefTouchingEMP()) {
            emp = TurnManager.instance.emp;
            emp.toggleEMP.gameObject.SetActive(true);
            emp.transform.parent = transform; // EMP becomes a child of the thief
            emp.OnDevicePickedUp();
        }
    }

    /// <summary>
    /// Activate the EMP if the toggle is on
    /// </summary>
    /// <param name="isOn"></param>
    public void TryToActivateEMP() { 
        if (emp && emp.toggleEMP.isOn) {
            emp.Activate_EMP();
        }
    }

    /// <summary>
    /// Charge the EMP for 1 turn
    /// </summary>
    public void ChargeEMP() { 
        if (emp) { 
            emp.ChargeOneTurn(); 
        }
    }

    #endregion

    #region GHOST_POOLING

    /// <summary>
    /// Get a Ghost game object from the object pool
    /// </summary>
    /// <returns></returns>
    GameObject GhostPooling() { 
        if (listGhosts.Count > 0) {
            return listGhosts[0];
        }

        GameObject ghost = Instantiate(ghostPrefab);
        ghost.SetActive(false);
        return ghost;
    }

    /// <summary>
    /// Get a ghost from the ghost pool and place it on a path tile
    /// </summary>
    /// <param name="newTile"></param>
    void AddGhostToPath(Tile newTile, Tile prevTile) {
        GameObject ghost = GhostPooling();
        listGhosts.RemoveAt(0);
        ghost.transform.position = new Vector3(newTile.transform.position.x, transform.position.y, newTile.transform.position.z);

        // Align ghost in the proper direction
        Vector3 direction = newTile.transform.position - prevTile.transform.position;
        ghost.transform.rotation = Quaternion.LookRotation(direction);

        ghost.SetActive(true);
        newTile.AddGhostToTile(ref ghost);
    }

    /// <summary>
    /// Remove the ghost thief from a tile that is removed from the path
    /// </summary>
    /// <param name="tile"></param>
    void RemoveGhostFromPath(Tile tile) {
        GameObject ghost = tile.RemoveLastGhost();
        ghost.SetActive(false);
        listGhosts.Add(ghost);
    }

    #endregion

}
