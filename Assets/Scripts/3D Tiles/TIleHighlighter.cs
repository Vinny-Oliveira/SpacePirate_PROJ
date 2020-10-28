using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlighter : MonoBehaviour {

    public GameObject highlightQuad; // Game Object highlighted
    Material highlightMaterial;

    const string matIsVisibleTag = "_HighlightIsVisible";
    bool canHighlight = false;

    /* Colors of Highlighters */
    static readonly string materialColorTag = "_HighlightColour";
    static readonly string colorMoveTag = "_HighlightColourGreen";
    static readonly string colorEmpTag = "_HighlightColourBlue";
    static readonly string colorEnemyVisionTag = "_HighlightColourRed";

    /// <summary>
    /// Get the material from the Quad's MeshRenderer and assign it to the highlightMaterial
    /// </summary>
    void SetHighlightMaterial() { 
        if (highlightQuad != null && highlightMaterial == null) {
            highlightMaterial = highlightQuad.GetComponent<MeshRenderer>().material;
            highlightMaterial.SetFloat(matIsVisibleTag, 0f);
        }
    }

    /// <summary>
    /// Change the HighlightColour to the color selected
    /// </summary>
    /// <param name="colorTag"></param>
    void ChangeColor(string colorTag) {
        SetHighlightMaterial();
        highlightMaterial.SetColor(materialColorTag, highlightMaterial.GetColor(colorTag));
    }

    /// <summary>
    /// Change the color of the shader to match the Thief range color
    /// </summary>
    public void ChangeColorToThiefMove() {
        ChangeColor(colorMoveTag);
    }

    ///// <summary>
    ///// Change the color of the shader to match the Thief path color
    ///// </summary>
    //public void ChangeColorToThiefPath() {
    //    ChangeColor(colorThiefPathTag);
    //}
    
    /// <summary>
    /// Change the color of the shader to the EMP range color
    /// </summary>
    public void ChangeColorToEmp() {
        ChangeColor(colorEmpTag);
    }

    /// <summary>
    /// Change the color of the shader to match the Cube's field of view color
    /// </summary>
    public void ChangeColorToEnemyVision() {
        ChangeColor(colorEnemyVisionTag);
    }

    /// <summary>
    /// Toggle the shader on or off so that the object is highlighted or goes back to normal
    /// </summary>
    public void ToggleHighlight() {
        // If the shader isn't visible, make it visible
        if (!canHighlight) {
            TurnHighlighterOn();
        } else { // Make the shader invisible
            TurnHighlighterOff();
        }
    }

    /// <summary>
    /// Make the hilight shader visible
    /// </summary>
    public void TurnHighlighterOn() {
        highlightMaterial.SetFloat(matIsVisibleTag, 1f);
        canHighlight = true;
    }

    /// <summary>
    /// Make the highlight shader invisible
    /// </summary>
    public void TurnHighlighterOff() {
        highlightMaterial.SetFloat(matIsVisibleTag, 0f);
        canHighlight = false;
    }

    /// <summary>
    /// EDITOR USE
    /// Add this gameObject to the quad
    /// </summary>
    [ContextMenu("Add Self")]
    public void AddSelf() {
        highlightQuad = gameObject;
    }
}
