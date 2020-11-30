using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    public bool IsOpen { get; set; }

    public Tile doorTile;
    public ECardType cardType;
    public Animator animator;
    public UnityEngine.UI.Toggle toggleDoor;
    public AudioSource audioOpenDoor;

    /// <summary>
    /// Play the animation to open the door
    /// </summary>
    public void OpenDoor() {
        IsOpen = true;

        if (animator) {
            animator.SetBool(animator.parameters[0].name, true);
        }

        if (audioOpenDoor) {
            audioOpenDoor.Play();
        }
    }

    /// <summary>
    /// Mark the door as closed
    /// </summary>
    public void CloseDoor() { 
        IsOpen = false;
        toggleDoor.isOn = false;
    }

    /// <summary>
    /// Event for when the toggle of the door is used
    /// </summary>
    public void OnToggleValueChanged() {
        IsOpen = toggleDoor.isOn;

        if (IsOpen) {
            TurnManager.instance.thief.OpenDoorMidPath(doorTile);
        } else {
            TurnManager.instance.thief.CloseDoorMidPath(doorTile);
        }
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
