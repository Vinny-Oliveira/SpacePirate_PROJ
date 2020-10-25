﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour {

    [Header("Characters and Items")]
    public Thief thief;
    public Treasure treasure;
    public List<CubeBot> listCubes;
    public List<Keycard> listKeycards = new List<Keycard>();
    public List<SecurityCamera> listSecCams = new List<SecurityCamera>();
    public EMP_Device emp;

    [Header("Turn Control")]
    public Tile exitTile;
    public UnityEngine.UI.Button btnEndTurn;

    [Header("UI Images and Panels")]
    public GameObject keycard_Image;
    public GameObject treasure_Image;
    public GameObject thiefWinPanel;
    public GameObject thiefLosePanel;
    public GameObject needKeycardPanel;
    public GameObject needTreasurePanel;

    public bool CanClick { get; set; }
    int intMoveCount;

    public static TurnManager instance;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        CanClick = true;
        SetupCharacters();
    }

    #region STARTUP_FUNCTIONS

    /// <summary>
    /// Trigger the startup functions of all Characters
    /// </summary>
    void SetupCharacters() { 
        thief.SetupThief();

        foreach (var cube in listCubes) {
            cube.SetupCubeStart();
        }

        foreach (var secCam in listSecCams) {
            secCam.SetFieldOfView();
        }
    }

    #endregion

    #region CHECKERS_FOR_OBJECTS_ON_SAME_TILES

    /// <summary>
    /// Check if two tiles are the same
    /// </summary>
    /// <param name="tile1"></param>
    /// <param name="tile2"></param>
    /// <returns></returns>
    bool AreTilesTheSame(ref Tile tile1, ref Tile tile2) { 
        if (tile1.Equals(tile2)) {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if the cube and the thief are on the same tile
    /// </summary>
    /// <returns></returns>
    public bool IsThiefTouchingCube(ref Tile cubeTile) {
        return AreTilesTheSame(ref cubeTile, ref thief.currentTile);
    }
    
    public bool IsThiefTouchingCube() {
        foreach (var cube in listCubes) { 
            if (AreTilesTheSame(ref cube.currentTile, ref thief.currentTile)) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Check if the Thief is touching a keycard
    /// </summary>
    /// <returns></returns>
    public Keycard IsThefTouchingKeycard() { 
        foreach (var keycard in listKeycards) { 
            if (AreTilesTheSame(ref thief.currentTile, ref keycard.placeTile)) {
                return keycard;
            }
        }

        return null;
    }

    /// <summary>
    /// Check if the player caught the treasure
    /// </summary>
    /// <returns></returns>
    public bool IsThiefTouchingTreasure() {
        return AreTilesTheSame(ref treasure.placeTile, ref thief.currentTile);
    }
    
    /// <summary>
    /// Check if the player is touching the EMP device
    /// </summary>
    /// <returns></returns>
    public bool IsThiefTouchingEMP() {
        return AreTilesTheSame(ref emp.placeTile, ref thief.currentTile);
    }

    /// <summary>
    /// Check if the thief has escaped with the treasure
    /// </summary>
    /// <returns></returns>
    public bool CanThiefEscape() {
        if (AreTilesTheSame(ref exitTile, ref thief.currentTile)) { // Thief on Exit tile
            if (thief.HasTreasure) {
                return true;
            }
            ThiefNeedsTreasure();
        }

        return false;
    }

    #endregion

    #region TURN_CONTROL

    /// <summary>
    /// Check if the characters can take their next steps
    /// </summary>
    /// <returns></returns>
    public bool CanCharactersStep() {
        bool canStep = thief.CanStep;
        foreach (var cube in listCubes) {
            canStep = canStep && cube.CanStep;
        }

        return canStep;
    }

    /// <summary>
    /// Event for when the End Turn button is pressed
    /// </summary>
    public void OnEndTurnButtonPress() {
        PlayEveryAction();
    }

    /// <summary>
    /// Play all the actions of the thief and the cubes
    /// </summary>
    void PlayEveryAction() {
        // Disable clicking while things move
        CanClick = false;
        intMoveCount = listCubes.Count + 1; // Cubes plus 1 thief
        btnEndTurn.interactable = false;
        thief.TryToActivateEMP();

        // Play actions
        thief.TurnTargetTilesOff();
        thief.MoveOnPath();
        foreach (var cube in listCubes) {
            cube.MoveOnPath();
        }
    }

    /// <summary>
    /// Decrese the count of moving objects and enable move when the count is zero
    /// </summary>
    public void DecreaseMovementCount() {
        intMoveCount--;
        if (intMoveCount < 1) {
            EnableNewTurn();
        }
    }

    /// <summary>
    /// Enable a new turn to be played
    /// </summary>
    void EnableNewTurn() { 
        if (EnableEnemies()) { return; }
        EnableThief();
    }

    /// <summary>
    /// Enable the enemies and check if they see the Thief.
    /// </summary>
    /// <returns></returns>
    bool EnableEnemies() { 
        // Re-enable the disabled cubes
        foreach (var cube in listCubes.Where(x => x.IsDisabled && x.CanEnable())) {
            cube.EnableEnemy();
            if (IsEnemySeeingThief(cube.GetFieldOfView())) {
                return HandleThiefCaught();
            }
        }

        // Move security cameras
        foreach (var secCam in listSecCams) { 
            // Enable cameras that can be enables, or reduce their wait turns
            if (secCam.IsDisabled) {
                secCam.ReduceOneWaitTurn();

                if (secCam.CanEnable()) { 
                    secCam.EnableEnemy();
                }
            } 
                
            // Go to next camera position
            if (!secCam.IsDisabled) {
                secCam.NextPosition();
                secCam.SetFieldOfView();
                if (IsEnemySeeingThief(secCam.GetFieldOfView())) {
                    return HandleThiefCaught();
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Enable clicking and moving Thief again.
    /// </summary>
    void EnableThief() {
        CanClick = true;
        btnEndTurn.interactable = true;
        thief.ChargeEMP();
        thief.StartNewPath();
    }

    #endregion

    #region HANDLERS_FOR_OBJECTS_SHARING_SAME_TILE

    /// <summary>
    /// Thief is caught by cubes
    /// </summary>
    /// <returns></returns>
    public bool IsThiefCaught() { 
        if (IsThiefTouchingCube() || IsThiefSeenByCube() || IsThiefSeenByCameras()) {
            return HandleThiefCaught();
        }
        return false;
    }

    /// <summary>
    /// Check if the cube is touching the thief when it rolls to a new tile
    /// </summary>
    /// <param name="newTile"></param>
    /// <param name="fieldOfView"></param>
    /// <returns></returns>
    public bool IsThiefCaught(ref Tile newTile, ref List<Tile> fieldOfView) {
        if (IsThiefTouchingCube(ref newTile) || IsEnemySeeingThief(fieldOfView)) {
            return HandleThiefCaught();
        }
        return false;
    }

    /// <summary>
    /// Clear Thief's path and turn lose panel on
    /// </summary>
    /// <returns></returns>
    bool HandleThiefCaught() {
        thief.ClearPath();
        Debug.Log("THIEF CAUGHT!");
        thiefLosePanel.SetActive(true);
        return true;
    }

    /// <summary>
    /// Check if the Thief can grab the treasure
    /// </summary>
    public void CheckForTreasure() { 
        if (!thief.HasTreasure && IsThiefTouchingTreasure()) {
            thief.HasTreasure = true;
            PickUpTreasure();
        }
    }

    /// <summary>
    /// Beat the level if Thief has escaped
    /// </summary>
    /// <returns></returns>
    public bool HasThiefBeatenLevel() { 
        if (CanThiefEscape()) {
            thief.ClearPath();
            Debug.Log("THIEF ESCAPED");
            thiefWinPanel.SetActive(true);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Make the player grab the treasure
    /// </summary>
    public void PickUpTreasure() {
        treasure.StealCoins();
        treasure_Image.SetActive(true);
    }

    #endregion

    #region THIEF_ON_ENEMIES_FIELD_OF_VIEW

    /// <summary>
    /// Check if a given Enemy's field of view is seeing the Thief
    /// </summary>
    /// <param name="fieldOfView"></param>
    /// <returns></returns>
    public bool IsEnemySeeingThief(List<Tile> fieldOfView) { 
        foreach (var seenTile in fieldOfView) { 
            if (seenTile.Equals(thief.currentTile)) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Check if a list of enemies see the Thief
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="listEnemies"></param>
    /// <returns></returns>
    bool IsThiefSeenByEnemies<T>(List<T> listEnemies) where T : Enemy { 
        foreach (var secCam in listEnemies.Where(x => !x.IsDisabled)) { 
            if (IsEnemySeeingThief(secCam.GetFieldOfView())) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Check if the Thief is seen by enabled the cubes
    /// </summary>
    /// <returns></returns>
    public bool IsThiefSeenByCube() {
        return IsThiefSeenByEnemies(listCubes);
    }

    /// <summary>
    /// Check if the Thief is seen by enabled security cameras
    /// </summary>
    /// <returns></returns>
    public bool IsThiefSeenByCameras() {
        return IsThiefSeenByEnemies(listSecCams);
    }

    #endregion

    #region ENEMIES_FIELD_OF_VIEW

    /// <summary>
    /// Highlight the tiles within all the enabled cubes' fields of view
    /// </summary>
    public void HighlightCubesFieldsOfView() {
        HighlightFieldsOfView<CubeBot>(listCubes);
    }

    
    /// <summary>
    /// Highlight the tiles within all the enabled cubes' fields of view
    /// </summary>
    public void HighlightCamerasFieldsOfView() {
        HighlightFieldsOfView<SecurityCamera>(listSecCams);
    }

    /// <summary>
    /// Highlight the field of view of Enemy types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="listEnemies"></param>
    void HighlightFieldsOfView<T>(List<T> listEnemies) where T : Enemy { 
        foreach (var enemy in listEnemies.Where(x => !x.IsDisabled)) {
            enemy.HighlightFieldOfView();
        }
    }

    #endregion

    #region PLAYER_NEEDS_PANELS

    /// <summary>
    /// Turn a panel on for a period of time
    /// </summary>
    /// <param name="panel"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator TurnOnAndOffPanel(GameObject panel, float time = 1.5f) {
        panel.SetActive(true);
        yield return new WaitForSeconds(time);
        panel.SetActive(false);
    }

    /// <summary>
    /// Turn the need keycard panel on
    /// </summary>
    public void ThiefNeedsKeycard() {
        StartCoroutine(TurnOnAndOffPanel(needKeycardPanel));
    }
    
    /// <summary>
    /// Turn the need treasure panel on
    /// </summary>
    public void ThiefNeedsTreasure() {
        StartCoroutine(TurnOnAndOffPanel(needTreasurePanel));
    }

    #endregion

}