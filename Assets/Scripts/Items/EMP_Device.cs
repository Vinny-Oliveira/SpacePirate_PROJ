using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EMP_Device : Item {

    [Header("EMP Functionality")]
    public float fltRange;
    public int intTurnsAffected;
    public ParticleSystem particle;
    public Toggle toggleEMP;
    public AudioClip clipActivateEmp;
    public EMP_Counter empCounter;

    [Header("Toggle Visuals")]
    public Color colorNoninteractable = new Color(200f / 255f, 200f / 255f, 200f / 255f, 128f / 255f);
    public Sprite spriteActive;
    public Sprite spriteNotActive;
    public Image imgToggle;

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
    public override void On_ItemPickedUp() {
        Change_Interactability(true);
        toggleEMP.gameObject.SetActive(true);
        base.On_ItemPickedUp();
    }

    /// <summary>
    /// Disable cubes within range of the EMP charge
    /// </summary>
    public void Activate_EMP() {
        // Play particles
        if (particle) { 
            particle.Play();
        }

        // Play the sound
        GameUtilities.PlayAudioClip(ref clipActivateEmp, ref audioSource);

        List<Enemy> listEnemies = new List<Enemy>();
        listEnemies.AddRange(TurnManager.instance.listCubes);
        listEnemies.AddRange(TurnManager.instance.listSecCams);
        listEnemies.AddRange(TurnManager.instance.listLaserBeams);

        // Disable enemies
        foreach (var enemy in listEnemies) { 
            if (listRangeTiles.Contains(enemy.currentTile)) {
                enemy.DisableEnemy(intTurnsAffected);
            }
        }

        // Disable the EMP and toss it
        Change_Interactability(false);
        toggleEMP.isOn = false;
        toggleEMP.gameObject.SetActive(false);
        TurnManager.instance.thief.DropEmp();
        empCounter.EnableCounter(intTurnsAffected);
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
            imgToggle.sprite = spriteActive;
            FindTilesWithinRange();
        } else {
            imgToggle.sprite = spriteNotActive;
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

    /// <summary>
    /// Change interactability of the toggle and its visuals
    /// </summary>
    /// <param name="is_interactable"></param>
    public void Change_Interactability(bool is_interactable) {
        toggleEMP.interactable = is_interactable;

        if (is_interactable) {
            imgToggle.color = Color.white;
        } else {
            imgToggle.color = colorNoninteractable;
        }
    }

}
