using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerItemUIInfo : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public Button readyBtn;
    public Image readyImg;

    public void Init(int playerNum, string pName)
    {
        playerName.text = playerNum + ". " + pName;

        //We dont show the ready button if the player is not the local player. Only the local player can press their respective "ready" button.

        if (PhotonNetwork.LocalPlayer.ActorNumber != playerNum)
        {
            readyBtn.gameObject.SetActive(false);
        }

        else //we are a local player. so the ready button should be active, as it is by default
        {

        }
    }


    public void OnReadyButtonClicked()
    {

        readyBtn.gameObject.SetActive(false);
        SetReadyState(true);
        //Transmit to photon network that our local player button has been clicked.
        ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable() { { "pReady", true } };

        PhotonNetwork.LocalPlayer.SetCustomProperties(properties);

    }



    public void SetReadyState(bool isReady)
    {
        //This is called on a local player's game instance via OnReadyButtonClicked()
        //Also for every remote player that joins the room (and when a remote player marks as ready)
        //
        if (isReady)
        {
            readyImg.enabled = true;
        }
    }

}
