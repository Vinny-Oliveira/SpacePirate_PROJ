using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMP_Device : Item {

    public float fltRange;
    public int intChargeTurns;
    public int intTurnsAffected;
    public ParticleSystem particle;
    public UnityEngine.UI.Button btnActivate_EMP;
    public GameObject empBody;

    public bool CanActivate { 
        get { return (intWaitTurns < 1); }
    }
    int intWaitTurns;

    /// <summary>
    /// Push the button to activate the EMP
    /// </summary>
    public void OnActivateBtnDown() {
        Activate_EMP();
    }

    /// <summary>
    /// Called when the EMP is picked up. Setup all initial values
    /// </summary>
    public void OnDevicePickedUp() {
        empBody.SetActive(false);
        intWaitTurns = 0;
    }

    /// <summary>
    /// Disable cubes within range of the EMP charge
    /// </summary>
    public void Activate_EMP() {
        // Play particles
        if (particle) { 
            particle.Play();
        }

        List<Enemy> listEnemies = new List<Enemy>();
        listEnemies.AddRange(TurnManager.instance.listCubes);
        listEnemies.AddRange(TurnManager.instance.listSecCams);

        // Disable enemies
        foreach (var enemy in listEnemies) { 
            if (Vector3.Magnitude(TurnManager.instance.thief.transform.position - enemy.transform.position) < fltRange) {
                enemy.DisableEnemy(intTurnsAffected);
            }
        }

        // Set turns to wait
        intWaitTurns = intChargeTurns;
        btnActivate_EMP.interactable = false;
    }

    /// <summary>
    /// Charge the EMP for 1 turn
    /// </summary>
    public void ChargeOneTurn() {
        if (intWaitTurns > 0) { 
            intWaitTurns--;
        } else {
            btnActivate_EMP.interactable = true;
        }
    }

}
