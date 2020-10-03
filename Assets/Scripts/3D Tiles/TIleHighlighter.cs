using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlighter : MonoBehaviour {

    public GameObject highlightQuad;
    public Material highlightMaterial;

    public string materialIsVisibleTag;
    bool canHighlight;
    
    private void Start() {
        // Get material of the quad's MeshRenderer
        if (highlightQuad != null) { 
            highlightMaterial = highlightQuad.GetComponent<MeshRenderer>().material;
        }
    }

    /// <summary>
    /// Change the HighlightColour to the color selected
    /// </summary>
    /// <param name="colorTag"></param>
    void ChangeColor(string colorTag) {
        highlightMaterial.SetColor(ColorHighlightManager.instance.materialColourTag, highlightMaterial.GetColor(colorTag));
    }

    /// <summary>
    /// Change the color of the shader to match the Thief range color
    /// </summary>
    public void ChanceColorToThiefRange() {
        ChangeColor(ColorHighlightManager.instance.colorThiefRangeTag);
    }

    /// <summary>
    /// Change the color of the shader to match the Thief path color
    /// </summary>
    public void ChanceColorToThiefPath() {
        ChangeColor(ColorHighlightManager.instance.colorThiefPathTag);
    }

    /// <summary>
    /// Change the color of the shader to match the Cube's field of view color
    /// </summary>
    public void ChanceColorToCubeView() {
        ChangeColor(ColorHighlightManager.instance.colorCubeViewTag);
    }

    /// <summary>
    /// Toggle the shader on or off so that the object is highlighted or goes back to normal
    /// </summary>
    [ContextMenu("VisibilityToggle")]
    public void ToggleHighlight() {

        // If the shader isn't visible, make it visible
        if (!canHighlight) {
            highlightMaterial.SetFloat(materialIsVisibleTag, 1f);
            canHighlight = true;
        
        } else { // Make the shader invisible
            highlightMaterial.SetFloat(materialIsVisibleTag, 0f);
            canHighlight = false;
        }
    }

}
