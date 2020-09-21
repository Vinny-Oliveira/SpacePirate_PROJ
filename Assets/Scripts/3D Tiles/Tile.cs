using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType {
    EVEN = 0,
    ODD = 1
}
public class Tile : MonoBehaviour {

    public GridManager gridManager;
    public Vector3 coordinates;
    public TileType tileType;
    public List<Tile> listNeighbors;

    bool canMoveHere = false;
    public Material defaultMaterial;

    public void SetDefaultMaterial() {
        defaultMaterial = GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void ResetMaterial() {
        GetComponent<MeshRenderer>().sharedMaterial = defaultMaterial;
    }

    public void SetTileMaterial(Material newMaterial) {
        GetComponent<MeshRenderer>().sharedMaterial = newMaterial;
    }

    /// <summary>
    /// Setter of coordinates and tileType
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="type"></param>
    public void SetLocation(int x, int z, TileType type) {
        coordinates = new Vector3(x, 0, z);
        tileType = type;
    }

    /// <summary>
    /// Getter of coordinates
    /// </summary>
    /// <returns></returns>
    public Vector3 GetLocation() {
        return coordinates;
    }

    /// <summary>
    /// Event for when the tile is clicked
    /// </summary>
    private void OnMouseDown() {
        if (gridManager != null && gridManager.player != null && canMoveHere) {
            gridManager.player.MoveToTile(this);
        }
    }
}
