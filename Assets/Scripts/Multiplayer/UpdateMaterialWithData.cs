using UnityEngine;

namespace Multiplayer
{
    public class UpdateMaterialWithData : MonoBehaviour
    {
        public Material helmetMaterial;

        public void UpdateMaterialProperties(PlayerCustimizationSO data)
        {
            foreach (var kvp in data.ColorPropertyList)
            {
                helmetMaterial.SetColor(kvp.propertyTag, kvp.propertyValue);
            }
        }
    }
}