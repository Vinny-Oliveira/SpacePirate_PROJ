using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroVideoController : MonoBehaviour
{
    public UnityEngine.Video.VideoPlayer videoPlayer;
    public SceneChanger sceneChanger;

    private void Start() {
        // Add the event for the end of the video
        videoPlayer.loopPointReached += OnVideoEndReached;
    }

    /// <summary>
    /// Event for when the video has reached ots end: change
    /// </summary>
    /// <param name="player"></param>
    void OnVideoEndReached(UnityEngine.Video.VideoPlayer player) {
        sceneChanger.LoadScene();
    }

}
