using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSelector : MonoBehaviour {

    public NetworkManager networkManager;
    public Button thiefBtn;
    public Button securityBtn;

    /// <summary>
    /// Enable the buttons only for the Master Client
    /// </summary>
    private void OnEnable() {
        if (Photon.Pun.PhotonNetwork.IsMasterClient) {
            thiefBtn.interactable = true;
            securityBtn.interactable = true;
        }
    }

}
