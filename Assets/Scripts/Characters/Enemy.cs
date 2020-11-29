using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Character {

    /* Field of View */
    protected List<Tile> listFieldOfView = new List<Tile>();

    /* EMP Effects */
    public bool IsDisabled { get; set; }
    protected int intWaitTurns;
    [Header("EMP Effects")]
    public GameObject visionCones;

    #region MOVEMENT

    /// <summary>
    /// Check if the enemy is not disabled
    /// </summary>
    /// <param name="moveCoroutine"></param>
    public void PlayMovePattern(IEnumerator moveCoroutine) {
        if (!IsDisabled) { 
            StartCoroutine(moveCoroutine);
        }
    }

    #endregion

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
            tile.visionQuad.ChangeColorToEnemyVision();
            tile.visionQuad.TurnHighlighterOn();
        }
    }

    /// <summary>
    /// Clear the filed of view tile list and turn their highlighters off
    /// </summary>
    public void DisableFieldOfView() { 
        foreach (var tile in listFieldOfView) {
            tile.visionQuad.TurnHighlighterOff();
        }
        listFieldOfView.Clear();
    }

    /// <summary>
    /// Look for a tile on the grid with the given coordinates and add it to the field of view
    /// </summary>
    /// <param name="newTileCoord"></param>
    protected void AddTileWithCoordinates(Vector3 newTileCoord) { 
        Tile viewedTile = currentTile.gridManager.listGridTiles.Find(tile => tile.coordinates == newTileCoord);

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
        if (visionCones) { 
            visionCones.SetActive(false); 
        }
        DisableFieldOfView();
    }

    /// <summary>
    /// Enable the enemy and turn the field of view on
    /// </summary>
    public virtual void EnableEnemy() { 
        IsDisabled = false;
        if (visionCones) { 
            visionCones.SetActive(true); 
        }
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
