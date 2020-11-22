using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanZoom : MonoBehaviour {

    Vector3 clickStart;
    public Camera mainCam;
    public float fltMaxRadius = 15f;
    public float fltZoomOutMin = 1f;
    public float fltZoomOutMax = 8f;
    public float fltRotationSpeed = 20f;
    public Transform pivot;
    public List<Transform> listButtons = new List<Transform>();
    Vector3 distToPivot;
    Quaternion initRotation;

    private void Start() {
        // Set the vector that represents the distance to the pivot
        distToPivot = transform.position - pivot.position;
        initRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update() {
        PanCamera();
        Zoom(Input.GetAxis("Mouse ScrollWheel"));
        RotateCamera();
        if (Input.GetKeyDown(KeyCode.Space)) {
            ResetCamera();
        }
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
            direction = new Vector3(direction.x, 0f, direction.z);
            
            // Set bounds to the pan
            if ((mainCam.transform.position + direction).magnitude < fltMaxRadius) { 
                mainCam.transform.position += direction;
            }

        }
    }

    /// <summary>
    /// Zoom in or out using within a range
    /// </summary>
    /// <param name="increment"></param>
    void Zoom(float increment) {
        mainCam.orthographicSize = Mathf.Clamp(mainCam.orthographicSize - increment, fltZoomOutMin, fltZoomOutMax);
    }

    /// <summary>
    /// Rotate the camera to see the entire level
    /// </summary>
    void RotateCamera() {
        RotateCamera(KeyCode.A, fltRotationSpeed);
        RotateCamera(KeyCode.D, -fltRotationSpeed);
    }

    void RotateCamera(KeyCode keyCode, float speed) { 
        if (Input.GetKey(keyCode)) {
            //transform.LookAt(pivot.position);
            transform.RotateAround(pivot.position, Vector3.up, speed * Time.deltaTime);

            // Make buttons look away from the camera
            foreach (var button in listButtons) {
                button.LookAt(2 * button.position - mainCam.gameObject.transform.position);
            }
        }
    }

    /// <summary>
    /// Reset the camera position
    /// </summary>
    void ResetCamera() { 
        transform.position = pivot.position + distToPivot;
        transform.rotation = initRotation;
    }

}
