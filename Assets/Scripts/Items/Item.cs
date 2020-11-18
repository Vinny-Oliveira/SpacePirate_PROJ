using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    public Tile placeTile;
    public Animator animatorPanel;
    public string strAnimationState;

    /// <summary>
    /// Animate the instructions panel of the item
    /// </summary>
    public void PlayAnimationPanel() { 
        if (animatorPanel) {
            animatorPanel.Play(strAnimationState);
        }
    }

}
