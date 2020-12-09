using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostThief : MonoBehaviour {

    public Animator animator;
    const string WALK_ANIM_STATE_NAME = "Crouched Walking";
    const string GRAB_ANIM_STATE_NAME = "Taking Item";

    /// <summary>
    /// Play the ghost's walk animation
    /// </summary>
    public void PlayWalkAnimation() { 
        if (animator) {
            animator.Play(WALK_ANIM_STATE_NAME);
        }
    }
    
    /// <summary>
    /// Play the ghost's grab animation
    /// </summary>
    public void PlayGrabAnimation() { 
        if (animator) {
            animator.Play(GRAB_ANIM_STATE_NAME);
        }
    }

}
