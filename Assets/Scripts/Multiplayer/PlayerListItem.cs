using TMPro;
using UnityEngine;

namespace Multiplayer
{
    public class PlayerListItem : MonoBehaviour
    {
        public TextMeshProUGUI playerNameText;

        public void Init(string playerName)
        {
            playerNameText.text = playerName;
        }
    }
}