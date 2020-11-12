using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public GameObject spacePirate;
    public float rotateValue;

    /// <summary>
    ///rotate the camera around a point.
    /// the point should be movable using the pan that is already implemented.
    /// </summary>

    void RotateCamera()
    {
        transform.LookAt(spacePirate.transform.position);

        if (rotateValue > 0.0f)
        {
            if (Input.GetKey(KeyCode.A))
            {
                Vector3 centre = spacePirate.transform.position; //find the position of the centre of the spaceship level. this could be a new game object or the "spaceship" game object. 
                Vector3 distToCentre = transform.position - centre; //find the distance from that centre point to the main camera
                Vector3 angles = new Vector3(0, rotateValue, 0);
                Quaternion newRotation = Quaternion.Euler(angles);
                Vector3 newDir = newRotation * distToCentre;
                transform.position = centre + newDir;
                transform.LookAt(centre);
            }
            if (Input.GetKey(KeyCode.D))
            {
                Vector3 centre = spacePirate.transform.position; //find the position of the centre of the spaceship level. this could be a new game object or the "spaceship" game object. 
                Vector3 distToCentre = transform.position - centre; //find the distance from that centre point to the main camera
                Vector3 angles = new Vector3(0, -rotateValue, 0);
                Quaternion newRotation = Quaternion.Euler(angles);
                Vector3 newDir = newRotation * distToCentre;
                transform.position = centre + newDir;
                transform.LookAt(centre);
            }

        }

    }

    private void Start()
    {
        transform.LookAt(spacePirate.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        //RotateCamera();
        transform.RotateAround(spacePirate.transform.position, new Vector3(0.0f, 1.0f, 0.0f), 20 * Time.deltaTime/* * speedMod*/);
    }
}

