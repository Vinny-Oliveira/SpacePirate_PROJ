using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMP_Counter : MonoBehaviour {

    public int IntTurnCounter { get; set; }
    public GameObject inventoryPanel;
    public TMPro.TextMeshProUGUI tmpCounter;

    /// <summary>
    /// Enable the EMP Counter and start counting
    /// </summary>
    /// <param name="turns"></param>
    public void EnableCounter(int turns) {
        IntTurnCounter = turns;
        tmpCounter.text = IntTurnCounter.ToString();
        inventoryPanel.SetActive(true);
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Countdown the EMP turns
    /// </summary>
    public void CountdownTurns() {
        IntTurnCounter--;
        tmpCounter.text = IntTurnCounter.ToString();
        if (IntTurnCounter < 1) {
            StartCoroutine(DisableWithDelay());
        }
    }

    /// <summary>
    /// Disable the EMP Counter after some delay
    /// </summary>
    /// <returns></returns>
    IEnumerator DisableWithDelay() {
        yield return new WaitForSeconds(0.25f);
        gameObject.SetActive(false);
        inventoryPanel.SetActive(false);
    }
}
