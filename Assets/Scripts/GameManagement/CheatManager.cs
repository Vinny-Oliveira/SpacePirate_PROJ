using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour {

    public Dictionary<string, System.Action> cheatFunctions = new Dictionary<string, System.Action> { 
        { "VC7930", TurnVisionConesOff },
        { "DM3516", DoubleThiefsMoves },
        { "KC5432", CollectEveryKeycard }
    };

    public void ActivateCheat(string code) { 
        if (cheatFunctions.ContainsKey(code)) { 
            
        }
    }

    static void TurnVisionConesOff() {

    }
    
    static void DoubleThiefsMoves() {

    }
    
    static void CollectEveryKeycard() {

    }

}
