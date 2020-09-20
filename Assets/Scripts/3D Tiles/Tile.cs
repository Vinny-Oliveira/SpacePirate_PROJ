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

    public void SetLocation(int x, int z, TileType type) {
        coordinates = new Vector3(x, 0, z);
        tileType = type;
    }

    public Vector3 GetLocation() {
        return coordinates;
    }

    private void OnMouseDown() {
        if (gridManager != null && gridManager.player != null) {
            gridManager.player.MoveToTile(this);
        }
    }
}
