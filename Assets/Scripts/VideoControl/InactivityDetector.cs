using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class InactivityDetector : VideoController {

    float fltTimeCounter;

    protected override void Start() {
        fltTimeCounter = 0f;
        base.Start();
        StartCoroutine(CountTimeOnScreen());
        StartCoroutine(Wait_For_Input());
    }

    /// <summary>
    /// Play the end video event if the user was inactive for a time greater than the length of the video.
    /// Event: load the intro scene
    /// </summary>
    /// <param name="player"></param>
    protected override void OnVideoEndReached(VideoPlayer player) {
        if (fltTimeCounter > videoPlayer.length - 2f) { 
            base.OnVideoEndReached(player);
        }
    }

    /// <summary>
    /// Count every second the user spend on this scene
    /// </summary>
    /// <returns></returns>
    IEnumerator CountTimeOnScreen() {
        yield return new WaitForSeconds(1f);
        fltTimeCounter++;
        StartCoroutine(CountTimeOnScreen());
    }

    /// <summary>
    /// Zero out the counter whenever the user inputs something
    /// </summary>
    /// <returns></returns>
    IEnumerator Wait_For_Input() {
        yield return new WaitUntil(() => Input.anyKeyDown);
        fltTimeCounter = 0f;
        StartCoroutine(Wait_For_Input());
    }

}
