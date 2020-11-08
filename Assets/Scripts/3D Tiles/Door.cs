using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    public bool IsOpen { get; set; }
    
    public ECardType cardType;
    public Animator animator;
    public UnityEngine.UI.Toggle toggleDoor;
    public Color openColor;

    /// <summary>
    /// Play the animation to open the door
    /// </summary>
    public void OpenDoor() {
        IsOpen = true;

        if (!animator) {
            Debug.Log("Attach an Animator");
            return;
        }

        animator.SetBool(animator.parameters[0].name, true);
    }

    /// <summary>
    /// Event for when the toggle of the door is used
    /// </summary>
    public void OnToggleValueChanged() {
        IsOpen = toggleDoor.isOn;
    }

    /// <summary>
    /// Turn the toggle game object on
    /// </summary>
    public void EnableToggle() {
        toggleDoor.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// Turn the toggle game object off
    /// </summary>
    public void DisableToggle() {
        toggleDoor.gameObject.SetActive(false);
    }

}
