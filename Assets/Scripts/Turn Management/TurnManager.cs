using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {

    [Header("Characters and Treasure")]
    public ThiefController thief;
    public Treasure treasure;
    public List<CubeMovement> listCubes;

    [Header("Turn Control")]
    public Tile exitTile;
    public UnityEngine.UI.Button btnEndTurn;

    [Header("UI Panels")]
    public GameObject thiefWinPanel;
    public GameObject thiefLosePanel;
    public GameObject securityWinPanel;
    public GameObject securityLosePanel;

    public bool CanMove { get; set; } = true;
    int intMoveCount;

    public static TurnManager instance;

    private void Awake() {
        instance = this;
        CanMove = true;
    }

    private void Start() {
        foreach (var cube in listCubes) {
            cube.SetupCubeStart();
        }
    }

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
    /// Check if the player caught the treasure
    /// </summary>
    /// <returns></returns>
    public bool IsThiefTouchingTreasure() {
        return AreTilesTheSame(ref treasure.placeTile, ref thief.currentTile);
    }

    /// <summary>
    /// Check if the thief has escaped with the treasure
    /// </summary>
    /// <returns></returns>
    public bool HasThiefEscaped() {
        return (thief.HasTreasure && AreTilesTheSame(ref exitTile, ref thief.currentTile));
    }

    #endregion

    #region TURN_CONTROL

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
        // Disable movement
        CanMove = false;
        intMoveCount = listCubes.Count + 1; // Cubes plus 1 thief
        btnEndTurn.interactable = false;

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
            CanMove = true;
            btnEndTurn.interactable = true;
        }
    }

    #endregion

    #region HANDLERS_FOR_OBJECTS_SHARING_SAME_TILE

    /// <summary>
    /// Check if the thief if touching a special tile when they reach a new tile
    /// </summary>
    /// <returns></returns>
    public bool HandleNewTile() {
        // Thief touched a cube
        if (IsThiefTouchingCube() || IsThiefSeenByCube()) {
            thief.ClearPath();
            Debug.Log("THIEF CAUGHT!");
            thiefLosePanel.SetActive(true);
            return true;
        }

        // Treasure caught
        if (!thief.HasTreasure && IsThiefTouchingTreasure()) {
            thief.HasTreasure = true;
            MakeThiefGrabTreasure();
        }

        // Escaped with treasure
        if (HasThiefEscaped()) {
            thief.ClearPath();
            Debug.Log("THIEF ESCAPED");
            thiefWinPanel.SetActive(true);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if the cube is touching the thief when it rolls to a new tile
    /// </summary>
    /// <param name="newTile"></param>
    /// <returns></returns>
    public bool HandleNewTile(ref Tile newTile, ref List<Tile> cubeFieldOfView) {
        if (IsThiefTouchingCube(ref newTile) || IsCubeSeeingThief(ref cubeFieldOfView)) {
            thief.ClearPath();
            Debug.Log("THIEF CAUGHT!");
            thiefLosePanel.SetActive(true);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Make the player grab the treasure
    /// </summary>
    public void MakeThiefGrabTreasure() {
        treasure.gameObject.transform.parent = thief.treasureHolder.transform;
        treasure.gameObject.transform.position = thief.treasureHolder.transform.position;
        Debug.Log("Treasure Found!");
    }

    #endregion

    #region THIEF_ON_CUBES_FIELD_OF_VIEW

    /// <summary>
    /// Check if the Thief seen by any of the cubes
    /// </summary>
    /// <returns></returns>
    public bool IsThiefSeenByCube() { 
        foreach (var cube in listCubes) {
            foreach (var viewedTile in cube.listFieldOfView) { 
                if (viewedTile.Equals(thief.currentTile)) {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Check if a given Cube is seeing the Thief
    /// </summary>
    /// <param name="cubeFieldOfView"></param>
    /// <returns></returns>
    public bool IsCubeSeeingThief(ref List<Tile> cubeFieldOfView) { 
        foreach (var seenTile in cubeFieldOfView) { 
            if (seenTile.Equals(thief.currentTile)) {
                return true;
            }
        }

        return false;
    }

    #endregion

    /// <summary>
    /// Highlight the tiles within all the cubes' fields of view
    /// </summary>
    public void HighlightCubesFieldsOfView() { 
        foreach (var cube in listCubes) {
            cube.HighlightFieldOfView();
        }
    }
}
