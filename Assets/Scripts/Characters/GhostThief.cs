using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostThief : MonoBehaviour {

    public GameObject ghost;
    public Animator animator;
    const string WALK_ANIM_NAME = "IsWalking";
    const string GRAB_ANIM_NAME = "Grab";

    /// <summary>
    /// Play the ghost's walk animation
    /// </summary>
    public void PlayWalkAnimation() { 
        if (animator) {
            animator.SetBool(WALK_ANIM_NAME, true);
        }
    }
    
    /// <summary>
    /// Play the ghost's grab animation
    /// </summary>
    public void PlayGrabAnimation() { 
        if (animator) {
            animator.SetTrigger(GRAB_ANIM_NAME);
        }
    }

}
