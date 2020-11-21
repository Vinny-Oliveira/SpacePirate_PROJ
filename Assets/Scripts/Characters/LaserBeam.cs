using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : Enemy {

    public List<bool> listLaserStates = new List<bool>();
    Queue<bool> queLaserStates = new Queue<bool>();

    /// <summary>
    /// Setup all initial values for the Laser Beam
    /// </summary>
    public void SetupLaserStart() {
        GameUtilities.EnqueueList(ref listLaserStates, ref queLaserStates);
    }

    /// <summary>
    /// Have the laser start its blinking pattern if it is active
    /// </summary>
    public override void MoveOnPath() {
        PlayMovePattern(BlinkLaserBeam());
    }

    /// <summary>
    /// Blink the laser beam according to its pattern
    /// </summary>
    /// <returns></returns>
    IEnumerator BlinkLaserBeam() {
        CanStep = false;

        // Blink the vision cylinder ("cone") of the laser beam, the object that has the collider
        bool laserState = queLaserStates.Dequeue();
        queLaserStates.Enqueue(laserState);
        visionCones.SetActive(laserState);

        // Wait
        yield return StartCoroutine(WaitOnTile());
        CanStep = true;
    }

    /// <summary>
    /// Check if the Laser Beam has detected the Thief
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {
        Thief touchedThief = other.gameObject.GetComponent<Thief>();
        if (touchedThief) {
            TurnManager.instance.HandleThiefCaught();
        }
    }

}
