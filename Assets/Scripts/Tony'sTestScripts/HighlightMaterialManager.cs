using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightMaterialManager : MonoBehaviour
{
    public GameObject highlightQuad;
    public Material highlightMaterial;
    [ColorUsage(true,true)]
    Color newColour;
    public string materialColourTag;
    public string materialSineSpeedTag;
    public string materialIsVisibleTag;
    //public string materialIsVisionTag;
    public float sineSpeed; 
    //public float alpha;
    public bool isVisible;
    //public bool isVision;
    public int colourNumber;

    public List<string> listOfMaterialColours;

    private void Start()
    {
        // This makes sure that the game object has a reference of itself as well as 
        // an instance of the shader so that it affects only itself
        // If you put this code on a parent to control a child, this will need to be
        // changed to look for the child instead of this game object
        highlightQuad = this.gameObject;
        highlightMaterial = highlightQuad.GetComponent<MeshRenderer>().material;
    }

    /// <summary>
    /// This function will take a number (x) and find the string in that position in a
    /// list of string representing ID Tags, it will then change the current colour of 
    /// the highlight shader to the colour associated with that ID Tag
    /// 
    /// -----------------------------------
    /// 
    /// Current list of colours:
    /// 0 = Red
    /// 1 = Blue
    /// 2 = Green
    /// 
    /// ***********************************
    /// *Please update if anything changes*
    /// ***********************************
    /// </summary>
    /// <param name="x"></param>
    public void ColourChanger(int x)
    {
        // Change the HighlightColour to the colour attached to listOfMaterialColours[x]
        highlightMaterial.SetColor(materialColourTag, highlightMaterial.GetColor(listOfMaterialColours[x]));
    }


    // For testing the ColourChanger function
    [ContextMenu("ChangeColour")]
    public void ChangeColour()
    {
        ColourChanger(colourNumber);
    }

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
