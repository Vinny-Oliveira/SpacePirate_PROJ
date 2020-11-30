using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTracker : MonoBehaviour {

    public TMPro.TextMeshProUGUI tmpActionName;
    public UnityEngine.UI.Image imgAction;

    static readonly Dictionary<EThiefStatus, Color> statusColorPairs = new Dictionary<EThiefStatus, Color> {
        { EThiefStatus.WAIT, new Color(1f, 0.4575f, 0.4575f, 1f) },
        { EThiefStatus.MOVE, new Color(0.4575f, 1f, 0.4575f, 1f) },
        { EThiefStatus.EMP,  new Color(0.4575f, 0.9f, 1f, 1f) },
        { EThiefStatus.OPEN, new Color(0.9f, 1f, 0.4575f, 1f) }
    };

    /// <summary>
    /// Change the information of the action tracker depending on the current status and set it active
    /// </summary>
    /// <param name="thiefStatus"></param>
    public void SetNewAction(EThiefStatus thiefStatus) {
        tmpActionName.text = thiefStatus.ToString();
        imgAction.color = statusColorPairs[thiefStatus];
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Turn the action tracker off
    /// </summary>
    public void TurnActionOff() {
        gameObject.SetActive(false);
    }

}
