using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

    protected List<Tile> listFieldOfView = new List<Tile>();

    /* Effects of the EMP */
    public bool IsDisabled { get; set; }
    protected int intWaitTurns;

    #region FIELD_OF_VIEW

    /// <summary>
    /// Getter of the list of tiles on the field of view
    /// </summary>
    /// <returns></returns>
    public List<Tile> GetFieldOfView() {
        return listFieldOfView;
    }

    /// <summary>
    /// Highlight the tiles in the Character's field of view
    /// </summary>
    public void HighlightFieldOfView() { 
        foreach (var tile in listFieldOfView) {
            tile.tileHighlighter.ChangeColorToCubeView();
            tile.tileHighlighter.TurnHighlighterOn();
        }
    }

    /// <summary>
    /// Clear the filed of view tile list and turn their highlighters off
    /// </summary>
    public void DisableFieldOfView() { 
        foreach (var tile in listFieldOfView) {
            tile.tileHighlighter.TurnHighlighterOff();
        }
        listFieldOfView.Clear();
    }

    /// <summary>
    /// Look for a tile on the grid with the given coordinates and add it to the field of view
    /// </summary>
    /// <param name="newTileCoord"></param>
    protected void AddTileWithCoordinates(Vector3 newTileCoord) { 
        Tile viewedTile = currentGrid.listGridTiles.Find(tile => tile.coordinates == newTileCoord);

        if (viewedTile) { 
            listFieldOfView.Add(viewedTile);
        }
    }

    #endregion

    #region EMP_EFFECTS

    /// <summary>
    /// Disable the cube and its field of view
    /// </summary>
    /// <param name="turns"></param>
    public void DisableEnemy(int turns) {
        IsDisabled = true;
        intWaitTurns = turns;
        DisableFieldOfView();
    }

    /// <summary>
    /// Enable the enemy and turn the field of view on
    /// </summary>
    public virtual void EnableEnemy() { 
        IsDisabled = false;
    }

    /// <summary>
    /// Check if the cube can be re-enabled
    /// </summary>
    /// <returns></returns>
    public bool CanEnable() {
        return (intWaitTurns < 1);
    }

    /// <summary>
    /// Reduce the wait turns by 1
    /// </summary>
    public void ReduceOneWaitTurn() {
        intWaitTurns--;
    }

    #endregion
}
