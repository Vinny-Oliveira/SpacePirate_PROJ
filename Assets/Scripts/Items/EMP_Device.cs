using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EMP_Device : Item {

    public float fltRange;
    public int intTurnsAffected;
    public ParticleSystem particle;
    public Toggle toggleEMP;
    public Color colorToggleOn;
    public GameObject empBody;

    Thief thief;
    List<Tile> listRangeTiles = new List<Tile>();

    /// <summary>
    /// Activate the EMP if the toggle is on
    /// </summary>
    public void TryToActivateEMP() { 
        if (toggleEMP.isOn) {
            Activate_EMP();
        }
    }

    /// <summary>
    /// Called when the EMP is picked up. Setup all initial values
    /// </summary>
    public void OnDevicePickedUp(Thief newThief) {
        thief = newThief;
        toggleEMP.interactable = true;
        toggleEMP.gameObject.SetActive(true);
        empBody.SetActive(false);
        PlayAnimationPanel();
    }

    /// <summary>
    /// Disable cubes within range of the EMP charge
    /// </summary>
    public void Activate_EMP() {
        // Play particles
        if (particle) { 
            particle.Play();
        }

        List<Enemy> listEnemies = new List<Enemy>();
        listEnemies.AddRange(TurnManager.instance.listCubes);
        listEnemies.AddRange(TurnManager.instance.listSecCams);

        // Disable enemies
        foreach (var enemy in listEnemies) { 
            if (listRangeTiles.Contains(enemy.currentTile)) {
                enemy.DisableEnemy(intTurnsAffected);
            }
        }

        // Disable the EMP and toss it
        toggleEMP.interactable = false;
        toggleEMP.isOn = false;
        toggleEMP.gameObject.SetActive(false);
        TurnManager.instance.emp = null;
    }

    /// <summary>
    /// Find tiles in range and change the color of the toggle if it is on
    /// </summary>
    public void OnToggleValueChanged() {
        HandleTilesWithinRange();
    }

    /// <summary>
    /// Depending on the state of the toggle, highlight the tiles in range or turn them off
    /// </summary>
    void HandleTilesWithinRange() { 
        if (toggleEMP.isOn) {
            FindTilesWithinRange();
        } else {
            ClearEmpTiles();
            TurnManager.instance.thief.ToggleEmpOff();
        }
    }

    /// <summary>
    /// Clear all the tiles within range of the EMP
    /// </summary>
    void ClearEmpTiles() { 
        foreach (var tile in listRangeTiles) {
            tile.empQuad.TurnHighlighterOff();
        }
        listRangeTiles.Clear();
    }

    /// <summary>
    /// Find which tiles are within the range of the EMP
    /// </summary>
    void FindTilesWithinRange() {
        TurnManager turnManager = TurnManager.instance;
        ClearEmpTiles();
        turnManager.thief.ToggleEmpOn();

        // Find tiles in each grid and highlight them
        foreach (var tile in turnManager.gridManager.listGridTiles) {
            // The last tile of the Thief's path is the origin of the EMP blast
            Tile originTile = (turnManager.thief.LastPathTile) ? (turnManager.thief.LastPathTile) : (turnManager.thief.currentTile);
            originTile.DisplayPathAndTargets();
            if (Vector3.Magnitude(originTile.transform.position - tile.transform.position) < fltRange + 0.5f) {
                listRangeTiles.Add(tile);
                tile.empQuad.ChangeColorToEmp();
                tile.empQuad.TurnHighlighterOn();
            }
        }
    }

}
