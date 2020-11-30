using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraPanZoom : MonoBehaviour {

    Vector3 clickStart;
    public Camera mainCam;

    [Header("Pan and Zoom Values")]
    public float fltMaxRadius = 15f;
    public float fltZoomOutMin = 1f;
    public float fltZoomOutMax = 8f;
    public float zoomSpeed = 1f;
    public float fltRotationSpeed = 20f;

    [Header("Pivots and Door Toggles")]
    public Transform thief;
    public List<Transform> listButtons = new List<Transform>();

    //Vector3 distToPivot;
    //Quaternion initRotation;

    private void Start() {
        ResetCamera();
    }

    // Update is called once per frame
    void Update() {
        // Game must not be paused
        if (!PauseManager.IsPaused()) { 
            PanCamera();
            Zoom(Input.GetAxis("Mouse ScrollWheel") * zoomSpeed);
            RotateCamera();
            RotateActiveButtons();

            if (Input.GetKeyDown(KeyCode.Space)) {
                ResetCamera();
            }
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
            if ((transform.position + direction).magnitude < fltMaxRadius) { 
                transform.position += direction;
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
    /// Use keys A and D to rotate the camera to see the entire level
    /// </summary>
    void RotateCamera() {
        RotateCamera(KeyCode.A, fltRotationSpeed);
        RotateCamera(KeyCode.D, -fltRotationSpeed);
    }

    /// <summary>
    /// Rotate the camera to see the entire level
    /// </summary>
    /// <param name="keyCode"></param>
    /// <param name="speed"></param>
    void RotateCamera(KeyCode keyCode, float speed) { 
        if (Input.GetKey(keyCode)) {
            transform.RotateAround(transform.position, Vector3.up, speed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Reset the camera position
    /// </summary>
    void ResetCamera() { 
        transform.position = new Vector3(thief.position.x, transform.position.y, thief.position.z);
    }

    /// <summary>
    /// Make active buttons look away from the camera
    /// </summary>
    void RotateActiveButtons() { 
        foreach (var button in listButtons) {
            if (button.gameObject.activeInHierarchy) { 
                button.LookAt(2 * button.position - mainCam.gameObject.transform.position);
            }
        }
    }

    [ContextMenu("Rotate")]
    public void RotateAroundPivot() {
        mainCam.transform.RotateAround(transform.position, Vector3.up, -45f);
    }
}
