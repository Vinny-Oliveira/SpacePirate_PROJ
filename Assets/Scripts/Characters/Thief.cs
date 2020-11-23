using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

/// <summary>
/// Delegate used for multiplayer events
/// </summary>
public delegate void Notify();

/// <summary>
/// Status of the Thief during each move along their path
/// </summary>
public enum EThiefStatus { 
    WAIT = 0,
    MOVE = 1,
    EMP = 2,
    OPEN = 3
}

/// <summary>
/// Class for the Thief main character
/// </summary>
public class Thief : Character {

    /* Maximum moves per turn */
    int intMaxMoves;

    /* Path Control */
    Tile targetTile;
    List<Tile> listTargetTiles = new List<Tile>();
    List<Tile> listPathTiles = new List<Tile>();
    EThiefStatus thiefStatus;
    List<EThiefStatus> listThiefStatus = new List<EThiefStatus>();
    
    [Header("Path Control")]
    public GameObject ghostPrefab;
    public List<GameObject> listGhosts = new List<GameObject>();
    public List<ActionTracker> listActions = new List<ActionTracker>(); // CONSTRAINT: This list MUST have as many ActionTracker panels as the number of maximum moves for the Thief

    /// <summary>
    /// Last tile in the list of path tiles
    /// </summary>
    public Tile LastPathTile { 
        get {
            if (listPathTiles.Count > 0) {
                return listPathTiles.Last();
            } else {
                return currentTile;
            }
        }
    }

    ///* Door Control */
    List<Door> listTempDoors = new List<Door>();
    List<Tile> listOpenDoorTiles = new List<Tile>();

    /* Item Control */
    public bool HasTreasure { get; set; } = false;
    List<Keycard> listKeycards = new List<Keycard>();
    EMP_Device emp;

    [Header("Camera & UI")]
    public Camera mainCamera;
    public TMPro.TextMeshProUGUI tmpMoveCount;
    public Color maxRangeColor;

    [Header("Thief Animations")]
    public Animator animator;
    const string WALK_ANIM_NAME = "IsWalking";

    [Header("Audio Clips and Source")]
    public AudioClip clipPlayerWins;
    public AudioClip clipPlayerLoses;

    [Header("Particle Effects")]
    public ThiefPaticles thiefPaticles;

    #region STARTUP_FUNCTIONS

    /// <summary>
    /// Startup for the Thief
    /// </summary>
    public void SetupThief(int maxMoves) {
        SetMaxMoves(maxMoves);
        thiefPaticles.PlayEnterParticle();
        IsMoving = false;
        CanStep = true;
        HasTreasure = false;
        thiefStatus = EThiefStatus.WAIT;
        SetStartingTile();
        RepositionCamera();
        DisplayMoveCounter();
        StartNewPath();
    }

    /// <summary>
    /// Set the maximum number of moves
    /// </summary>
    /// <param name="maxMoves"></param>
    public void SetMaxMoves(int maxMoves) {
        intMaxMoves = maxMoves;
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
        currentTile = targetTile;
        StartCoroutine(WaitOnTile());
    }

    /// <summary>
    /// Have the Thief wait on the tile for a while before continuing the path, and check what is on the tile
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator WaitOnTile() {
        TurnManager turnManager = TurnManager.instance;

        //yield return new WaitUntil(() => turnManager.CanCharactersStep());
        yield return StartCoroutine(base.WaitOnTile());

        // Caught by a cube or ended the level
        if (turnManager.IsThiefCaught() || turnManager.HasThiefBeatenLevel()) {
            IsMoving = false;
            yield break;
        }

        // Check if touching treasure, keycard, or door
        PickUpKeycard();
        PickUpEMP();
        turnManager.CheckForTreasure();
        //MoveOnPath();

        CanStep = true;
        
        // Path is over
        if (listThiefStatus.Count < 1) {
            StopWalkAnimation();
            IsMoving = false;
            DisplayMoveCounter();
        }
    }

    /// <summary>
    /// Move the player through the path of tiles and destroy the list of path tiles as they go
    /// </summary>
    public override void MoveOnPath() {
        IsMoving = true;
        CanStep = false;
        HandleCurrentStatus();
        MoveStart();
    }

    /// <summary>
    /// Handle the Thief's behavior on the path depending on their status
    /// </summary>
    void HandleCurrentStatus() {
        if (listThiefStatus.Count < 1) {
            return;
        }

        thiefStatus = listThiefStatus[0];
        RemoveFirstActiveStatus(); // Remove 1st item of the list

        switch (thiefStatus) {
            case EThiefStatus.WAIT: // Just wait on the tile
                StopWalkAnimation();
                RemoveGhostFromPath(currentTile);
                StartCoroutine(WaitOnTile());
                break;

            case EThiefStatus.MOVE: // Move on the path
                PlayWalkAnimation();
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
                StopWalkAnimation();
                emp.TryToActivateEMP();
                StartCoroutine(WaitOnTile());
                break;

            case EThiefStatus.OPEN: // Open a door and wait on the tile
                StopWalkAnimation();
                listOpenDoorTiles.Last().door.OpenDoor();
                listOpenDoorTiles.RemoveAt(listOpenDoorTiles.Count - 1);
                StartCoroutine(WaitOnTile());
                break;

            default:
                break;
        }

        DisplayMoveCounter();
    }

    /// <summary>
    /// Fill the rest of the status list with IDLE
    /// </summary>
    public void CompleteStatusList() { 
        while (listThiefStatus.Count < intMaxMoves) {
            AddNewStatus(EThiefStatus.WAIT);
        }
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
        LastPathTile.DisplayPathAndTargets();
    }

    #endregion

    #region PATH_OF_TILES

    /// <summary>
    /// Highlight the tiles of the new path
    /// </summary>
    public void StartNewPath() {
        ClearPath();
        ResetDoorsOnPath();
        EnableDoorToggles(currentTile);
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

        // Add to path. If consecutive tiles reteat, use a WAIT status
        if ( (listPathTiles.Count > 0 && tile == listPathTiles.Last()) || (listPathTiles.Count < 1 && tile == currentTile) ) {
            AddNewStatus(EThiefStatus.WAIT);
        } else { 
            AddNewStatus(EThiefStatus.MOVE);
            listPathTiles.Add(tile);
            tile.moveQuad.TurnHighlighterOff();
        }

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
        foreach (var action in listActions) {
            action.TurnActionOff();
        }

        if (emp) {
            emp.toggleEMP.isOn = false;
        }

        RemoveGhostFromPath(currentTile);
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
        return (listThiefStatus.Count > 0 && 
                (listThiefStatus.Last() == EThiefStatus.MOVE || listThiefStatus.Last() == EThiefStatus.WAIT) && 
                tile.Equals(LastPathTile));
    }

    /// <summary>
    /// Remove the last tile of the path
    /// </summary>
    public void RemoveLastTileFromPath() {
        RemoveGhostFromPath(LastPathTile);
        if (listThiefStatus.Last() == EThiefStatus.MOVE) { 
            listPathTiles.RemoveAt(listPathTiles.Count - 1);
        }
        RemoveLastActiveStatus();

        // Enable the option to open doors if next to one
        EnableDoorToggles(LastPathTile);

        if (listThiefStatus.Count < 1) {
            return;
        }

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
            EnableDoorToggles(currentTile);
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
        //mainCamera.transform.DOMove(currentGrid.cameraHolder.position, 0.9f).OnUpdate(counterFollow.UpdateCounterPosition);
    }

    #endregion

    #region KEYCARDS_AND_DOORS

    /// <summary>
    /// Add a keycard to the list of keycards
    /// </summary>
    /// <param name="keycard"></param>
    public void PickUpKeycard() {
        Keycard keycard = TurnManager.instance.IsThefTouchingKeycard();

        if (keycard) {
            PickUpKeycard(ref keycard);
        }
    }

    public void PickUpKeycard(ref Keycard keycard) {
        listKeycards.Add(keycard);
        keycard.On_ItemPickedUp();
        keycard.placeTile = null;
    }

    /// <summary>
    /// Set the open door button active if the given tile has DOOR tiles as neighbors and the Thief has a keycard to open them
    /// </summary>
    /// <param name="tile"></param>
    void EnableDoorToggles(Tile tile) {
        if (listThiefStatus.Count < intMaxMoves) { 

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
    }

    /// <summary>
    /// Disable the door toggles of the door temporarily stored
    /// </summary>
    public void DisableDoorToggles() {
        foreach (var door in listTempDoors) {
            door.DisableToggle();
        }

        listTempDoors.Clear();
    }

    /// <summary>
    /// Mark a door as open and allow the Thief to move beyond it
    /// </summary>
    public void OpenDoorMidPath(Tile doorTile) {
        AddNewStatus(EThiefStatus.OPEN);
        listOpenDoorTiles.Add(doorTile);
        DisplayCurrentTargets();
    }

    /// <summary>
    /// Mark a door as not open and do not allow the Thief to move beyond it
    /// </summary>
    public void CloseDoorMidPath(Tile doorTile) {
        if (listThiefStatus.Count > 0) {
            RemoveLastActiveStatus();
        }
        listOpenDoorTiles.Remove(doorTile);
        DisplayCurrentTargets();
    }

    /// <summary>
    /// Close doors that were open during the path if the path is reset
    /// </summary>
    void ResetDoorsOnPath() {
        // Copy of open door list so that no items are removed from it during the loop
        List<Tile> tempTileDoors = new List<Tile>();
        tempTileDoors.AddRange(listOpenDoorTiles);

        foreach (var doorTile in tempTileDoors) {
            doorTile.door.CloseDoor();
        }

        listOpenDoorTiles.Clear();
    }

    #endregion

    #region EMP_DEVICE

    /// <summary>
    /// Pick up the EMP device
    /// </summary>
    public void PickUpEMP() { 
        if (emp == null && TurnManager.instance.IsThiefTouchingEMP()) {
            emp = TurnManager.instance.emp;
            emp.transform.parent = transform; // EMP becomes a child of the thief
            emp.On_ItemPickedUp();
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
        AddNewStatus(EThiefStatus.EMP);
        DisplayMoveCounter();
    }
    
    /// <summary>
    /// Add the EMP to the status list when the EMP is toggled on
    /// </summary>
    public void ToggleEmpOff() {
        if (listThiefStatus.Count > 0 && listThiefStatus.Last() == EThiefStatus.EMP) {
            RemoveLastActiveStatus();
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

        if (ghost) { 
            ghost.SetActive(false);
            listGhosts.Add(ghost);
        }
    }

    #endregion

    #region STATUS_AND_ACTION_LISTS

    /// <summary>
    /// Add a new status to the status list and turn on its respective action tracker
    /// </summary>
    /// <param name="thiefStatus"></param>
    void AddNewStatus(EThiefStatus thiefStatus) {
        listThiefStatus.Add(thiefStatus);
        listActions[listThiefStatus.Count - 1].SetNewAction(thiefStatus);
    }

    /// <summary>
    /// Remove the first status of the Status List and turn the first active action off
    /// </summary>
    void RemoveFirstActiveStatus() {
        listActions[listActions.Count - listThiefStatus.Count].TurnActionOff();
        listThiefStatus.RemoveAt(0);
    }

    /// <summary>
    /// Remove the last status of the Status List and turn the last active action off
    /// </summary>
    void RemoveLastActiveStatus() {
        listActions[listThiefStatus.Count - 1].TurnActionOff();
        listThiefStatus.RemoveAt(listThiefStatus.Count - 1);
    }

    #endregion

    #region EVENTS_FOR_MULTIPLAYER

    /* Movement Events */
    public event Notify MoveCompleted;

    public void MoveStart() {
        OnMoveCompleted();
    }

    protected virtual void OnMoveCompleted() {
        MoveCompleted?.Invoke();
    }

    /* Death Events */
    public event Notify ThiefDead;

    public void DeathStart() {
        OnThiefDead();
    }

    protected virtual void OnThiefDead() {
        ThiefDead?.Invoke();
    }

    #endregion

    #region WIN_LOSS_SFX

    /// <summary>
    /// Play the sound effect for when the player wins the level
    /// </summary>
    public void PlayWinSfx() {
        GameUtilities.PlayAudioClip(ref clipPlayerWins, ref audioSource);
    }
    
    /// <summary>
    /// Play the sound effect for when the player loses the level
    /// </summary>
    public void PlayLossSfx() {
        GameUtilities.PlayAudioClip(ref clipPlayerLoses, ref audioSource);
    }

    #endregion

    #region ANIMATION_FUNCTIONS

    /// <summary>
    /// Start playing the walk animation
    /// </summary>
    public void PlayWalkAnimation() {
        animator.SetBool(WALK_ANIM_NAME, true);
    }
    
    /// <summary>
    /// Stop playing the walk animation
    /// </summary>
    public void StopWalkAnimation() {
        animator.SetBool(WALK_ANIM_NAME, false);
    }

    #endregion

}
