using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {

    protected List<Tile> listFieldOfView = new List<Tile>();

    /* Effects of the EMP */
    public bool IsDisabled { get; set; }
    protected int intWaitTurns;

    public List<Tile> GetFieldOfView() {
        return listFieldOfView;
    }

}
