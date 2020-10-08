using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quit the application
/// </summary>
public class GameQuitter : MonoBehaviour {
    
    public void QuitGame() {
        Debug.Log("Quitting the game");
        Application.Quit();
    }

}
