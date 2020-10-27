using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EMP_Device : Item {

    public float fltRange;
    public int intChargeTurns;
    public int intTurnsAffected;
    public ParticleSystem particle;
    public Toggle toggleEMP;
    public Color colorToggleOn;
    public GameObject empBody;

    int intWaitTurns;
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
    public void OnDevicePickedUp() {
        empBody.SetActive(false);
        intWaitTurns = 0;
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

        // Set turns to wait
        intWaitTurns = intChargeTurns;
        toggleEMP.interactable = false;
        toggleEMP.isOn = false;
        OnToggleValueChanged();
    }

    /// <summary>
    /// Charge the EMP for 1 turn
    /// </summary>
    public void ChargeOneTurn() {
        if (intWaitTurns > 0) { 
            intWaitTurns--;
        } else {
            toggleEMP.interactable = true;
        }
    }

    /// <summary>
    /// FInd tile in range and change the color of the toggle if it is on
    /// </summary>
    public void OnToggleValueChanged() {
        // Change color
        ColorBlock colorBlock = toggleEMP.colors;
        colorBlock.normalColor = (toggleEMP.isOn) ? (colorToggleOn) : (Color.white);
        colorBlock.selectedColor = colorBlock.normalColor;
        toggleEMP.colors = colorBlock;
        
        // Find tiles in range
        if (toggleEMP.isOn) {
            FindTilesWithinRange();
        } else {
            listRangeTiles.Clear();
        }
    }

    /// <summary>
    /// Find which tiles are within the range of the EMP
    /// </summary>
    void FindTilesWithinRange() {
        TurnManager turnManager = TurnManager.instance;
        listRangeTiles.Clear();

        foreach (var grid in turnManager.listGrids) {
            foreach (var tile in grid.listGridTiles) {
                if (Vector3.Magnitude(turnManager.thief.currentTile.transform.position - tile.transform.position) < fltRange + 0.5f) {
                    listRangeTiles.Add(tile);
                }
            }
        }
    }

}
