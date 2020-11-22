using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    [Header("Game References")]
    public Tile placeTile;
    public GameObject pickableBody;
    public GameObject inventory_icon;

    [Header("Pickup Audio")]
    public AudioSource audioPickup;

    [Header("Panel Animation")]
    public Animator animatorPanel;
    public string strAnimationState;

    /// <summary>
    /// Animate the instructions panel of the item
    /// </summary>
    public void PlayAnimationPanel() { 
        if (animatorPanel != null && animatorPanel.isActiveAndEnabled) {
            animatorPanel.Play(strAnimationState);
        }
    }

    /// <summary>
    /// Event for when the item is picked up
    /// </summary>
    /// <param name="itemObject"></param>
    public virtual void On_ItemPickedUp() {
        if (audioPickup) {
            audioPickup.Play();
        }
        pickableBody.SetActive(false);
        inventory_icon.SetActive(true);
        PlayAnimationPanel();
    }

}
