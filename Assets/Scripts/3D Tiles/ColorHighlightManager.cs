using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorHighlightManager : MonoBehaviour {

    public string materialColourTag;

    public string colorThiefRangeTag;
    public string colorThiefPathTag;
    public string colorCubeViewTag;

    public static ColorHighlightManager instance;

    private void Awake() {
        instance = this;
    }
}
