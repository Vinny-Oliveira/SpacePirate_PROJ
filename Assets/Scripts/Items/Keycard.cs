using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ECardType { 
    CIRCLE = 0,
    SQUARE = 1,
    TRIANGLE = 2
}

public class Keycard : Item {

    public ECardType cardType;

}
