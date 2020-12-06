using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Multiplayer
{
    [CreateAssetMenu(order =  0, fileName = "New Custimization Setting", menuName = "Creat Custimization Setting")]
    public class PlayerCustimizationSO : ScriptableObject
    {
        [SerializeField] private List<ColorPropertyTagValue> colorPropertyList;
        public List<ColorPropertyTagValue> ColorPropertyList => colorPropertyList;
    }
}

[System.Serializable]
public class ColorPropertyTagValue
{
    public string propertyTag;
    [ColorUsage(true, true)]
    public Color propertyValue;
}