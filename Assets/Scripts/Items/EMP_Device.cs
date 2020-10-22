using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMP_Device : Item {

    public float fltRange;
    public int intChargeTurns;
    public int intTurnsAffected;
    public ParticleSystem particle;
    public bool CanActivate { 
        get { return (intWaitTurns < 1); }
    }
    int intWaitTurns;

    /// <summary>
    /// Disable cubes within range of the EMP charge
    /// </summary>
    [ContextMenu("EMP")]
    public void Activate_EMP() {
        // Play particles
        if (particle) { 
            particle.Play();
        }

        // Disable cubes
        foreach (var cube in TurnManager.instance.listCubes) { 
            if (Vector3.Magnitude(TurnManager.instance.thief.transform.position - cube.transform.position) < fltRange) {
                cube.DisableCube(intTurnsAffected);
            }
        }

        // Set turns to wait
        intWaitTurns = intChargeTurns;
    }

    /// <summary>
    /// Charge the EMP for 1 turn
    /// </summary>
    public void ChargeOneTurn() {
        if (intWaitTurns > 0) { 
            intWaitTurns--;
        }
    }

}
