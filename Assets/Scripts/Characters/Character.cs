using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    [Header("Grid Control")]
    public Tile currentTile;

    /// <summary>
    /// Is moving on the path
    /// </summary>
    protected bool IsMoving { get; set; }

    /// <summary>
    /// Can take the next step on the path
    /// </summary>
    public bool CanStep { get; set; }

    /* Movement times */
    protected const float stepTime = 0.5f;
    const float waitOnTileTime = 0.2f;

    /// <summary>
    /// Move the player to given tile
    /// </summary>
    /// <param name="nextTile"></param>
    protected virtual void MoveToTile(ref Tile nextTile) {
        currentTile = nextTile;
        Vector3 target = new Vector3(nextTile.gameObject.transform.position.x, transform.position.y, nextTile.gameObject.transform.position.z);
        transform.position = target;
    }

    /// <summary>
    /// Set the value of the tile the player starts on and move them there
    /// </summary>
    protected void SetStartingTile() { 
#if UNITY_EDITOR
        if (!currentTile) {
            Debug.Log("ERROR: Assign a Tile!");
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }
#endif
        //MoveToTile(ref currentTile);
    }

    /// <summary>
    /// Move on its path
    /// </summary>
    public virtual void MoveOnPath() { 
    
    }

    /// <summary>
    /// Have the character wait on a tile for a while after they land on it
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator WaitOnTile() {
        yield return new WaitForSeconds(waitOnTileTime);
    }
}
