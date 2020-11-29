using System.Collections.Generic;
using Characters;
using UnityEngine;

namespace Multiplayer
{
    public class EndPlayerListItem : MonoBehaviour
    {
        private List<PlayerLevelStats> _levelStats = new List<PlayerLevelStats>();
        
        public void Init(string playerName, List<PlayerLevelStats> playerLevelStats)
        {
            gameObject.name = playerName + " End List Object";
            _levelStats = playerLevelStats;
            //Populate Prefab with Data!
        }
    }
}