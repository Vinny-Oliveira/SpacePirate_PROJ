using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    public ECardType cardType;
    public Animator animator;
    public bool IsOpen { get; set; }

    /// <summary>
    /// Play the animation to open the door
    /// </summary>
    [ContextMenu("Open Door")]
    public void OpenDoor() {
        IsOpen = true;

        if (!animator) {
            Debug.Log("Attach an Animator");
            return;
        }

        animator.SetBool(animator.parameters[0].name, true);
    }

}
