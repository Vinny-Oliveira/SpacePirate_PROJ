using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoController : MonoBehaviour {

    public UnityEngine.Video.VideoPlayer videoPlayer;
    public SceneChanger sceneChanger;

    protected virtual void Start() {
        // Add the event for the end of the video
        videoPlayer.loopPointReached += OnVideoEndReached;
    }

    /// <summary>
    /// Event for when the video has reached its end: load the main menu scene
    /// </summary>
    /// <param name="player"></param>
    protected virtual void OnVideoEndReached(UnityEngine.Video.VideoPlayer player) {
        sceneChanger.LoadScene();
    }

}
