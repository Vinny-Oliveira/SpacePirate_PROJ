using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TIleHighlighter : MonoBehaviour {

    public GameObject highlightQuad;
    public Material highlightMaterial;
    public string materialColourTag;

    public string matColorThiefRange;
    public string matColorThiefPath;
    public string matColorCubeView;

    public string materialSineSpeedTag;
    public string materialIsVisibleTag;
    public float sineSpeed;
    public bool isVisible;
    
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
    public void ColorChanger(string colorTag) {
        highlightMaterial.SetColor(materialColourTag, highlightMaterial.GetColor(colorTag));
    }


    //// For testing the ColourChanger function
    //[ContextMenu("ChangeColour")]
    //public void ChangeColor() {
    //    //ColorChanger(colourNumber);
    //}

    // This was for testing the Alpha
    //[ContextMenu("AlphaChange")]
    //public void AlphaChange()
    //{
    //    highlightMaterial.SetFloat(materialAlphaTag, alpha);
    //}

    /// <summary>
    /// This function is for testing purposes and changes the frequency at which the sine wave that
    /// controls the shader's alpha(opaqueness) over time.
    /// </summary>
    [ContextMenu("SineSpeedChange")]
    public void SineSpeedChange()
    {
        highlightMaterial.SetFloat(materialSineSpeedTag, sineSpeed);
    }

    /// <summary>
    /// This function toggles visibility of the shader and toggles the "isVisible" bool
    /// The "isVisible" bool keeps track of whether or not the sharder is currently visible
    /// or not. There is not function for "SetBool" so I use "SetFloat" and it intakes 1 as true
    /// and 0 as false.
    /// </summary>
    [ContextMenu("VisibilityToggle")]
    public void VisibilityToggle()
    {
        // If the shader isn't visible
        if (!isVisible)
        {
            // Make the shader visible;
            highlightMaterial.SetFloat(materialIsVisibleTag, 1);
            isVisible = true;
        }
        else
        {
            // Make the shader invisible
            highlightMaterial.SetFloat(materialIsVisibleTag, 0);
            isVisible = false;
        }
    }

    //[ContextMenu("ColourToggle")]
    //public void colourToggle()
    //{
    //    if(isVision)
    //    {
    //        highlightMaterial.SetFloat(materialIsVisionTag, 1);
    //        isVision = false;
    //    }
    //    else
    //    {
    //        highlightMaterial.SetFloat(materialIsVisionTag, 0);
    //        isVision = true;
    //    }
    //}
}
