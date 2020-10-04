using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlighter : MonoBehaviour {

    public GameObject highlightQuad; // Game Object highlighted
    Material highlightMaterial;

    const string matIsVisibleTag = "_HighlightIsVisible";
    bool canHighlight;

    /* Colors of Highlighters */
    static readonly string materialColorTag = "_HighlightColour";
    static readonly string colorThiefRangeTag = "_HighlightColourGreen";
    static readonly string colorThiefPathTag = "_HighlightColourBlue";
    static readonly string colorCubeViewTag = "_HighlightColourRed";

    private void Start() {
        canHighlight = false;

        // Get material of the quad's MeshRenderer
        if (highlightQuad != null) { 
            highlightMaterial = highlightQuad.GetComponent<MeshRenderer>().material;
            highlightMaterial.SetFloat(matIsVisibleTag, 0f);
        }
    }

    /// <summary>
    /// Change the HighlightColour to the color selected
    /// </summary>
    /// <param name="colorTag"></param>
    void ChangeColor(string colorTag) {
        highlightMaterial.SetColor(materialColorTag, highlightMaterial.GetColor(colorTag));
    }

    /// <summary>
    /// Change the color of the shader to match the Thief range color
    /// </summary>
    public void ChangeColorToThiefRange() {
        ChangeColor(colorThiefRangeTag);
    }

    /// <summary>
    /// Change the color of the shader to match the Thief path color
    /// </summary>
    public void ChangeColorToThiefPath() {
        ChangeColor(colorThiefPathTag);
    }

    /// <summary>
    /// Change the color of the shader to match the Cube's field of view color
    /// </summary>
    public void ChangeColorToCubeView() {
        ChangeColor(colorCubeViewTag);
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
}
