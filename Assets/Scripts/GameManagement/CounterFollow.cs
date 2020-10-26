using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CounterFollow : MonoBehaviour
{
    public TextMeshProUGUI counterLable;

    // Update is called once per frame
    void Update()
    {
        Vector3 counterPose = Camera.main.WorldToScreenPoint(this.transform.position);
        counterLable.transform.position = counterPose;
    }
}
