﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {

    public ThiefController thief;
    public Treasure treasure;
    public List<CubeMovement> listCubes;

    public Tile exitTile;
    public UnityEngine.UI.Button btnEndTurn;

    public bool CanMove { get; set; } = true;
    int intMoveCount;

    public static TurnManager instance;

    private void Awake() {
        instance = this;
        CanMove = true;
    }

    private void Start() {
        listCubes[0].BuildDirectionDictionary();

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
        thief.MoveOnPath();
        thief.TurnTargetTilesOff();
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
        if (IsThiefTouchingCube()) {
            thief.ClearPath();
            Debug.Log("THIEF CAUGHT!");
            return true;
        }

        // Treasure caught
        if (IsThiefTouchingTreasure()) {
            thief.HasTreasure = true;
            MakeThiefGrabTreasure();
        }

        // Escaped with treasure
        if (HasThiefEscaped()) {
            thief.ClearPath();
            Debug.Log("THIEF ESCAPED");
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if the cube is touching the thief when it rolls to a new tile
    /// </summary>
    /// <param name="newTile"></param>
    /// <returns></returns>
    public bool HandleNewTile(ref Tile newTile) {
        if (IsThiefTouchingCube(ref newTile)) {
            thief.ClearPath();
            Debug.Log("THIEF CAUGHT!");
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

    /// <summary>
    /// Highlight the tiles within all the cubes' fields of view
    /// </summary>
    public void HighlightCubesFieldsOfView() { 
        foreach (var cube in listCubes) {
            cube.HighlightFieldOfView();
        }
    }
}
