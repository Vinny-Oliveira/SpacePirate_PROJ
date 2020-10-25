using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

    protected List<Tile> listFieldOfView = new List<Tile>();

    /* Effects of the EMP */
    public bool IsDisabled { get; set; }
    protected int intWaitTurns;

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
}
