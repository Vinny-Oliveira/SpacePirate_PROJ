using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventPlayer : MonoBehaviour {

    public SecurityCamera securityCamera;

    /// <summary>
    /// Event to set the field of view of a Security Camera
    /// </summary>
    public void OnCameraAnimEvent() {
        securityCamera.SetFieldOfView();
        securityCamera.CheckForThiefCaught();
    }

}
