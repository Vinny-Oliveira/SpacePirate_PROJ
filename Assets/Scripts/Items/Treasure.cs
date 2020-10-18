using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : Item {

    public GameObject coins;

    /// <summary>
    /// Turn the Coins object off
    /// </summary>
    public void StealCoins() {
        coins.SetActive(false);
        Debug.Log("Treasure Found!");
    }

}
