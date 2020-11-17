using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerMaskTest : MonoBehaviour
{
    public Camera myCamera;

    public int characterMask = ~(1<<8);

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit ,Mathf.Infinity, characterMask))
        {
            Debug.Log("Hit: " + hit.collider.gameObject.name);

            // Do something with the object that was hit by the raycast.
        }
    }

    private void OnMouseOver()
    {
        if(LayerMask.GetMask() == 1<<8)
        {
            Debug.Log("This is the character");
        }
    }

}
