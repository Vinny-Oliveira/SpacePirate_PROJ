using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterFollow : MonoBehaviour
{
    public TMPro.TextMeshProUGUI counterLabel;
    public Camera mainCam;

    /// <summary>
    /// Update the position of the counter
    /// </summary>
    public void UpdateCounterPosition() {
        if (counterLabel) {
            Vector3 counterPose = mainCam.WorldToScreenPoint(transform.position);
            counterLabel.transform.position = counterPose;
        }
    }
}
