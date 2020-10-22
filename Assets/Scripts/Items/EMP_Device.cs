using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMP_Device : Item {

    public float fltRange;
    public int intChargeCounter;
    public int intTurnsAffected;

    /// <summary>
    /// Disable cubes within range of the EMP charge
    /// </summary>
    public void Activate_EMP() { 
        foreach (var cube in TurnManager.instance.listCubes) { 
            if (Vector3.Magnitude(TurnManager.instance.thief.transform.position - cube.transform.position) < fltRange) {
                cube.DisableCube(intTurnsAffected);
            }
        }
    }

}
