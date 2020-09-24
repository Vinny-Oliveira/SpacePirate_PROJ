using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewScript : MonoBehaviour
{


    //a boolean state to check whether we are in a drag mode
    private bool isDragModeON = false;


    private List<Tile> tilesDragged;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            isDragModeON = true;


        if (isDragModeON)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit,100.0f))
            {
              if(  hit.transform.GetComponent<Tile>())
                {
                    //we hit a tile....
                    //change color of that tile...
                    //you want ensure we didn't already click on this til e earlier
                    //then add this to the tilesDragged list if it is not already in the list...
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragModeON = false;
                return;
            }



        }

    }



    void StartMovement()
    {

    }


}
