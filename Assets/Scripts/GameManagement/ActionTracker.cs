﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTracker : MonoBehaviour {

    public TMPro.TextMeshProUGUI tmpActionName;
    public UnityEngine.UI.Image imgAction;

    static Dictionary<EThiefStatus, Color> statusColorPairs = new Dictionary<EThiefStatus, Color> {
        {EThiefStatus.WAIT, new Color(1f, 1f, 1f, 1f) },
        {EThiefStatus.MOVE, new Color(1f, 0f, 0f, 1f) },
        {EThiefStatus.EMP, new Color(0f, 0f, 1f, 1f) },
        {EThiefStatus.OPEN, new Color(0f, 1f, 0f, 1f) }
    };

    /// <summary>
    /// 
    /// </summary>
    /// <param name="thiefStatus"></param>
    public void SetNewAction(EThiefStatus thiefStatus) {
        tmpActionName.text = thiefStatus.ToString();
        imgAction.color = statusColorPairs[thiefStatus];
        gameObject.SetActive(true);
    }

}
