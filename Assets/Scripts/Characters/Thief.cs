using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

/// <summary>
/// Status of the Thief during each move along their path
/// </summary>
public enum EThiefStatus { 
    IDLE = 0,
    MOVE = 1,
    EMP = 2,
    OPEN_DOOR = 3
}

/// <summary>
/// Class for the Thief main character
/// </summary>
public class Thief : Character {

    /* Maximum moves per turn */
    int intMaxMoves = 2;

    [Header("Path Control")]
    Tile targetTile;
    List<Tile> listTargetTiles = new List<Tile>();
    List<Tile> listPathTiles = new List<Tile>();
    EThiefStatus thiefStatus;
    List<EThiefStatus> listThiefStatus = new List<EThiefStatus>();
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

    ///* Door Control */
    List<Door> listTempDoors = new List<Door>();
    //List<Tuple<int, Door>> listDoorsToOpen = new List<Tuple<int, Door>>();

    /* Item Control */
    public bool HasTreasure { get; set; } = false;
    List<Keycard> listKeycards = new List<Keycard>();
    EMP_Device emp;

    [Header("Camera & UI")]
    public Camera mainCamera;
    //public UnityEngine.UI.Button btnOpenDoor;
    public TMPro.TextMeshProUGUI tmpMoveCount;
    public Color maxRangeColor;
    public CounterFollow counterFollow;

    #region STARTUP_FUNCTIONS

    /// <summary>
    /// Startup for the Thief
    /// </summary>
    public void SetupThief(int maxMoves) {
        intMaxMoves = maxMoves;
        IsMoving = false;
        CanStep = true;
        HasTreasure = false;
        thiefStatus = EThiefStatus.IDLE;
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
        CanStep = true;
        yield return new WaitUntil(() => TurnManager.instance.CanCharactersStep());
        yield return StartCoroutine(base.WaitOnTile());

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
        //OpenNeighborDoors();
        MoveOnPath();
    }

    /// <summary>
    /// Move the player through the path of tiles and destroy the list of path tiles as they go
    /// </summary>
    public override void MoveOnPath() {
        TurnManager turnManager = TurnManager.instance;
        
        // Path is over
        if (listThiefStatus.Count < 1) {
            IsMoving = false;
            turnManager.DecreaseMovementCount();
            DisplayMoveCounter();
            return;
        }

        // Continue the path
        IsMoving = true;
        CanStep = false;
        HandleCurrentStatus();
    }

    /// <summary>
    /// Handle the Thief's behavior on the path depending on their status
    /// </summary>
    void HandleCurrentStatus() {
        thiefStatus = listThiefStatus[0];
        listThiefStatus.RemoveAt(0);

        switch (thiefStatus) {
            case EThiefStatus.IDLE: // Just wait on the tile
                StartCoroutine(WaitOnTile());
                break;

            case EThiefStatus.MOVE: // Move on the path
                Tile nextTile = listPathTiles[0];
                MoveToTile(ref nextTile);
                RemoveGhostFromPath(nextTile);

                // Deactivate shaders and update counter
                listPathTiles.RemoveAt(0);
                if (!listPathTiles.Contains(nextTile)) { 
                    nextTile.moveQuad.TurnHighlighterOff();
                }
                TurnPathTilesOff();
                break;

            case EThiefStatus.EMP: // Activate the EMP and wait on the tile
                emp.TryToActivateEMP();
                StartCoroutine(WaitOnTile());
                break;

            case EThiefStatus.OPEN_DOOR: // Open the doors around and wait on the tile
                //OpenNeighborDoors();
                StartCoroutine(WaitOnTile());
                break;

            default:
                break;
        }

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

    /// <summary>
    /// Display the current targets depending on the last tile of the path
    /// </summary>
    public void DisplayCurrentTargets() { 
        if (LastPathTile) { 
            LastPathTile.DisplayPathAndTargets();
        } else {
            currentTile.DisplayPathAndTargets();
        }
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
        return (listThiefStatus.Count < intMaxMoves);
    }

    /// <summary>
    /// Highlight the tiles of the Thief's path
    /// </summary>
    public void TurnPathTilesOff() { 
        foreach (var tile in listPathTiles) {
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
        listThiefStatus.Add(EThiefStatus.MOVE);
        listPathTiles.Add(tile);
        tile.moveQuad.TurnHighlighterOff();

        // Disable EMP if the path is full or if the EMP is already active
        if (emp != null && (!CanAddToPath() || listThiefStatus.Contains(EThiefStatus.EMP))) {
            emp.toggleEMP.gameObject.SetActive(false);
        }

        // Enable the option to open doors if next to one
        EnableDoorToggles(tile);
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
        listThiefStatus.Clear();
        if (emp) {
            emp.toggleEMP.isOn = false;
        }
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
        return (listThiefStatus.Count > 0 && listThiefStatus.Last() == EThiefStatus.MOVE && tile.Equals(LastPathTile));
    }

    /// <summary>
    /// Remove the last tile of the path
    /// </summary>
    public void RemoveLastTileFromPath() {
        RemoveGhostFromPath(listPathTiles.Last());
        listPathTiles.RemoveAt(listPathTiles.Count - 1);
        listThiefStatus.RemoveAt(listThiefStatus.Count - 1);

        if (listThiefStatus.Count < 1) {
            return;
        }

        // Enable the option to open doors if next to one
        EnableDoorToggles((LastPathTile) ? (LastPathTile) : (currentTile));

        // Re-enable the EMP if possible
        if (emp != null && (listThiefStatus.Last() == EThiefStatus.EMP || !listThiefStatus.Contains(EThiefStatus.EMP))) {
            emp.toggleEMP.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Clear the cuurent path and start a new one from the current tile
    /// </summary>
    public void ResetPath() {
        if (TurnManager.instance.CanClick) { 
            TurnTargetTilesOff();
            ClearPath();
            //btnOpenDoor.gameObject.SetActive(false);
            if (emp) {
                emp.toggleEMP.gameObject.SetActive(true);
            }
            StartNewPath();
        }
    }

    /// <summary>
    /// Display how many moves the Thief still has
    /// </summary>
    public void DisplayMoveCounter() {
        if (listPathTiles.Count < intMaxMoves) {
            tmpMoveCount.color = Color.white;
        } else {
            tmpMoveCount.color = maxRangeColor;
        }

        tmpMoveCount.text = (intMaxMoves - listThiefStatus.Count).ToString() + "/" + intMaxMoves;
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
    /// Check if the Thief if close to doors that can be opened and open them
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

    /// <summary>
    /// Set the open door button active if the given tile has DOOR tiles as neighbors and the Thief has a keycard to open them
    /// </summary>
    /// <param name="tile"></param>
    void EnableDoorToggles(Tile tile) {
        // Disable door toggles from previous tile
        DisableDoorToggles();

        // Enable toggles of closed doors if the Thief has their keycards
        List<Tile> doorTiles = tile.listNeighbors.FindAll(x => x.tileType == ETileType.DOOR && !x.door.IsOpen);

        foreach (var doorTile in doorTiles) { 
            if (listKeycards.Exists(x => x.cardType == doorTile.door.cardType)) {
                listTempDoors.Add(doorTile.door);
                doorTile.door.EnableToggle();
            }
        }
    }

    /// <summary>
    /// Disable the door toggles of the door temporarily stored
    /// </summary>
    void DisableDoorToggles() {
        foreach (var door in listTempDoors) {
            door.DisableToggle();
        }

        listTempDoors.Clear();
    }

    ///// <summary>
    ///// Event for when the Open Doors button is pressed
    ///// </summary>
    //public void OnOpenDoorsButtonPressed() {
    //    listThiefStatus.Add(EThiefStatus.OPEN_DOOR);
    //    OpenDoorsMidPath();
    //    DisplayMoveCounter();
    //}

    ///// <summary>
    ///// Mark the doors as open and be able to move on a path beyond them
    ///// </summary>
    //void OpenDoorsMidPath() {
    //    // Store the index of the status list and the doors
    //    foreach (var door in listCloseDoors) {
    //        door.IsOpen = true;
    //        listDoorsToOpen.Add(new Tuple<int, Door>(listThiefStatus.Count - 1, door));
    //    }

    //    DisplayCurrentTargets();
    //}

    #endregion

    #region EMP_DEVICE

    /// <summary>
    /// Pick up the EMP device
    /// </summary>
    public void PickUpEMP() { 
        if (TurnManager.instance.IsThiefTouchingEMP()) {
            emp = TurnManager.instance.emp;
            emp.transform.parent = transform; // EMP becomes a child of the thief
            emp.OnDevicePickedUp(this);
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
    /// Add the EMP to the status list when the EMP is toggled on
    /// </summary>
    public void ToggleEmpOn() {
        listThiefStatus.Add(EThiefStatus.EMP);
        DisplayMoveCounter();
    }
    
    /// <summary>
    /// Add the EMP to the status list when the EMP is toggled on
    /// </summary>
    public void ToggleEmpOff() {
        if (listThiefStatus.Count > 0 && listThiefStatus.Last() == EThiefStatus.EMP) {
            listThiefStatus.RemoveAt(listThiefStatus.Count - 1);
            DisplayMoveCounter();
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
