using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileLocation : MonoBehaviour
{
    public TileInfo tileInfo = new TileInfo();
    public List<TileLocation> listNeighbors;

    public void SetLocation(int x, int z, TileType type) {
        tileInfo.coordinates = new Vector3(x, 0, z);
        tileInfo.tileType = type;
    }

    public void GetLocation() {
        print("LocationInfo is: " + tileInfo.coordinates);
    }
}
