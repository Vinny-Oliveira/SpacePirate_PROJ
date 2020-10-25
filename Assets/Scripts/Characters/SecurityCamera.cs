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
    public EDirection centerDirection;
    [SerializeField]
    bool isForward;

    /* Coordinates of fields of view */
    public List<Vector2> leftCoords = new List<Vector2>();
    public List<Vector2> centerCoords = new List<Vector2>();
    public List<Vector2> rightCoords = new List<Vector2>();

    /// <summary>
    /// Get the coordinates of the field of view depending on the camera's position
    /// </summary>
    /// <returns></returns>
    List<Vector2> GetCoordsOfField() {
        switch (camPosition) {
            case ECameraPosition.LEFT:
                return leftCoords;
            case ECameraPosition.CENTER:
                return centerCoords;
            case ECameraPosition.RIGHT:
                return rightCoords;
            default:
                return new List<Vector2> { Vector2.zero };
        }
    }

    /// <summary>
    /// Get the direction the camera faces if it starts on the center position
    /// </summary>
    /// <returns></returns>
    Vector3 GetFrontDirection(Vector2 coord) {
        switch (centerDirection) {
            case EDirection.NORTHEAST:
                return coord.x * Vector3.right + coord.y * Vector3.forward;
            case EDirection.NORTHWEST:
                return coord.x * Vector3.forward + coord.y * Vector3.left;
            case EDirection.SOUTHEAST:
                return coord.x * Vector3.back + coord.y * Vector3.right;
            case EDirection.SOUTHWEST:
                return coord.x * Vector3.left + coord.y * Vector3.back;
            default:
                return Vector3.zero;
        }
    }

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

    /// <summary>
    /// Set the camera's new field of view
    /// </summary>
    public void SetFieldOfView() {
        DisableFieldOfView();

        // Add tiles to the field of view depending on where the camera faces
        foreach (var coord in GetCoordsOfField()) {
            Vector3 newTileCoord = currentTile.coordinates + GetFrontDirection(coord);
            AddTileWithCoordinates(newTileCoord);
        }

        HighlightFieldOfView();
    }

    /// <summary>
    /// Enable the enemy and turn the field of view on
    /// </summary>
    public override void EnableEnemy() { 
        base.EnableEnemy();
        SetFieldOfView();
    }

}
