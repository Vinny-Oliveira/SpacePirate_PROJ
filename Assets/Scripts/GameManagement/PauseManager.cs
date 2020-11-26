using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage the pause state of the game
/// Attach this script to the Canvas_Level
/// </summary>
public class PauseManager {

    //private void Start(){
    //    ResumeGame(); // Make sure the game is not paused when a level starts
    //}

    /// <summary>
    /// Pause the game
    /// </summary>
    public void PauseGame() {
        Time.timeScale = 0;
    }

    /// <summary>
    /// Unpause the game
    /// </summary>
    public static void ResumeGame() {
        Time.timeScale = 1;
    }

}
