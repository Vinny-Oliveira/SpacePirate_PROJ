using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manage how scenes are changed
/// </summary>
public class SceneChanger : MonoBehaviour {

    public SceneField nextScene;

    /// <summary>
    /// Load a scene
    /// </summary>
    public void LoadScene() {
        SceneManager.LoadScene(nextScene);
    }

    /// <summary>
    /// Restart the current scene
    /// </summary>
    public void RestartScene() { 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
