using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    [Header("Game References")]
    public Tile placeTile;
    public GameObject pickableBody;
    public GameObject inventory_icon;

    [Header("Pickup Audio")]
    public AudioClip clipPickup;
    public AudioSource audioSource;

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
        PlayPickupSound();
        pickableBody.SetActive(false);
        inventory_icon.SetActive(true);
        PlayAnimationPanel();
    }

    /// <summary>
    /// Play a sound effect when the item is picked up
    /// </summary>
    void PlayPickupSound() {
        GameUtilities.PlayAudioClip(ref clipPickup, ref audioSource);
    }

}
