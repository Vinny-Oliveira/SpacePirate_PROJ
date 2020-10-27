using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterFollow : MonoBehaviour
{
    public TMPro.TextMeshProUGUI counterLable;
    public Camera mainCam;

    /// <summary>
    /// Update the position of the counter
    /// </summary>
    public void UpdateCounterPosition() {
        if (counterLable) {
            Vector3 counterPose = mainCam.WorldToScreenPoint(transform.position);
            counterLable.transform.position = counterPose;
        }
    }
}
