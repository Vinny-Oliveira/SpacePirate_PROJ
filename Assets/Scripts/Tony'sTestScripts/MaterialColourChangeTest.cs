using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialColourChangeTest : MonoBehaviour
{
    public Material tileMaterial;
    public string materialColourTag;
    [ColorUsage(true,true)]
    public Color newColour;
    public string materialAlphaTag;
    public string materialSineSpeedTag;
    public float sineSpeed; 
    public float alpha;

    [ContextMenu("ColourChange")]
    public void ColourChanger()
    {
        tileMaterial.SetColor(materialColourTag, newColour);
    }

    [ContextMenu("AlphaChange")]
    public void AlphaChange()
    {
        tileMaterial.SetFloat(materialAlphaTag, alpha);
    }
    [ContextMenu("SineSpeedChange")]
    public void SineSpeedChange()
    {
        tileMaterial.SetFloat(materialSineSpeedTag, sineSpeed);
    }

}
