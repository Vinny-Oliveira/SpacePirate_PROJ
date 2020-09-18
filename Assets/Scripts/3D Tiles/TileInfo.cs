using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType {
    //Edge = 0,
    //Walkable = 1,
    //Obstacle = 2
    EVEN = 0,
    ODD = 1
}

public class TileInfo {

    public Vector3 coordinates;
    public TileType tileType;

}
