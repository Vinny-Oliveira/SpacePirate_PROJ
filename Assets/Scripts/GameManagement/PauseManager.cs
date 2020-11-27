using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage the pause state of the game
/// Attach this script to the Canvas_Level
/// </summary>
public class PauseManager : MonoBehaviour {

    //public static PauseManager instance;

    //private void Awake() {
    //    instance = this;
    //}

    /// <summary>
    /// Pause the game
    /// </summary>
    public void PauseGame() {
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Unpause the game
    /// </summary>
    public void ResumeGame() {
        UnPauseGame();
    }

    /// <summary>
    /// Static method to un pause the game
    /// </summary>
    public static void UnPauseGame() {
        Time.timeScale = 1f;
    }

}
