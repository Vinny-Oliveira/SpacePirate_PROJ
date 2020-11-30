using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorDialogHandler : MonoBehaviour {

    public GameObject errorPanel;

    public GameObject errorText1;
    public GameObject errorText2;
    public GameObject errorText3;

    void TurnOnPanel(GameObject error_text) {
        errorPanel.SetActive(true);
        error_text.SetActive(true);
    }

    public void TurnOnPanel_1() {
        TurnOnPanel(errorText1);
    }
    
    public void TurnOnPanel_2() {
        TurnOnPanel(errorText2);
    }
    
    public void TurnOnPanel_3() {
        TurnOnPanel(errorText3);
    }

}
