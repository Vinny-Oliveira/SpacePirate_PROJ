using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ECameraPosition { 
    LEFT = -1,
    CENTER = 0,
    RIGHT = 1
}

public class SecurityCamera : Enemy {

    public ECameraPosition camPosition;
    bool isForward;

    /* Coordinates of fields of view */
    public List<Vector2> leftCoords = new List<Vector2>();
    public List<Vector2> centerCoords = new List<Vector2>();
    public List<Vector2> rightCoords = new List<Vector2>();

    /* Map each position enum to a list of coordinates */
    Dictionary<ECameraPosition, List<Vector2>> dicCoords;

    /// <summary>
    /// Calculate the next camera position
    /// </summary>
    public void NextPosition() {
        // Bounce back if maximum or minimum positions reached
        if (camPosition == System.Enum.GetValues(typeof(ECameraPosition)).Cast<ECameraPosition>().Max()) {
            camPosition = System.Enum.GetValues(typeof(ECameraPosition)).Cast<ECameraPosition>().Max() - 1;
            isForward = false;
        
        } else if (camPosition == System.Enum.GetValues(typeof(ECameraPosition)).Cast<ECameraPosition>().Min()) {
            camPosition = System.Enum.GetValues(typeof(ECameraPosition)).Cast<ECameraPosition>().Min() + 1;
            isForward = true;
        
        // Simply go forward or backwards
        } else {
            camPosition = (isForward) ? (camPosition + 1) : (camPosition - 1);
        }

    }



}
