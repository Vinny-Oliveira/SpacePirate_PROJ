using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanZoom : MonoBehaviour {

    Vector3 clickStart;
    public Camera mainCam;
    public float fltMaxRadius = 15f;
    public float fltZoomOutMin = 1f;
    public float fltZoomOutMax = 8f;
    public CounterFollow counterFollow;

    // Update is called once per frame
    void Update() {
        PanCamera();
        Zoom(Input.GetAxis("Mouse ScrollWheel"));
    }

    /// <summary>
    /// Pan the camera on the scene
    /// </summary>
    void PanCamera() {
        // Right mouse click start
        if (Input.GetMouseButtonDown(1)) {
            clickStart = mainCam.ScreenToWorldPoint(Input.mousePosition);
        }

        // Right mouse click continues down
        if (Input.GetMouseButton(1)) {
            Vector3 direction = clickStart - mainCam.ScreenToWorldPoint(Input.mousePosition);
            
            // Set bounds to the pan
            if ((mainCam.transform.position + direction).magnitude < fltMaxRadius) { 
                mainCam.transform.position += direction;
            }

            counterFollow.UpdateCounterPosition();
        }
    }

    /// <summary>
    /// Zoom in or out using within a range
    /// </summary>
    /// <param name="increment"></param>
    void Zoom(float increment) {
        mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize - increment, fltZoomOutMin, fltZoomOutMax);
        counterFollow.UpdateCounterPosition();
    }
}
