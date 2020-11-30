using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorDialogHandler : MonoBehaviour {

    public GameObject errorPanel;

    public GameObject errorText_NoPlayerName;
    public GameObject errorText_NoRoomCreated;
    public GameObject errorText_NotEnoughPlayers;
    public GameObject errorText_WrongRoom;

    /// <summary>
    /// Turn off all messages and then turn on the one you need
    /// </summary>
    /// <param name="error_text"></param>
    void TurnOnPanel(GameObject error_text) {
        errorText_NoPlayerName.SetActive(false);
        errorText_NoRoomCreated.SetActive(false);
        errorText_NotEnoughPlayers.SetActive(false);
        errorText_WrongRoom.SetActive(false);

        errorPanel.SetActive(true);
        error_text.SetActive(true);
    }

    public void TurnOnPanel_NoPlayerName() {
        TurnOnPanel(errorText_NoPlayerName);
    }
    
    public void TurnOnPanel_NoRoomCreated() {
        TurnOnPanel(errorText_NoRoomCreated);
    }
    
    public void TurnOnPanel_NotEnoughPlayers() {
        TurnOnPanel(errorText_NotEnoughPlayers);
    }
    
    public void TurnOnPanel_WrongRoom() {
        TurnOnPanel(errorText_WrongRoom);
    }

}
