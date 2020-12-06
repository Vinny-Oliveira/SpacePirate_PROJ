using System.Collections.Generic;
using Characters;
using TMPro;
using UnityEngine;

namespace Multiplayer
{
    public class EndPlayerListItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI totalTimeText;
        [SerializeField] private TextMeshProUGUI totalTurnsText;
        [SerializeField] private TextMeshProUGUI totalDeathsText;
        
        private List<PlayerLevelStats> _levelStats = new List<PlayerLevelStats>();
        
        public void Init(string playerName, List<PlayerLevelStats> playerLevelStats)
        {
            gameObject.name = playerName + " End List Object";
            _levelStats = playerLevelStats;
            //Populate Prefab with Data!
            playerNameText.text = playerName;
            PopulateData();
        }

        private void PopulateData()
        {
            int totalTime = 0;
            int totalDeaths = 0;
            int totalTurns = 0;
            
            foreach (var stat in _levelStats)
            {
                totalDeaths += stat.Deaths;
                totalTurns += stat.MovesMade;
                totalTime += stat.TimeTaken;
            }

            totalTurnsText.text = totalTurns.ToString();
            totalDeathsText.text = totalDeaths.ToString();
            
            string minutes = Mathf.Floor(totalTime / 60).ToString("00");
            string seconds = Mathf.Floor(totalTime % 60).ToString("00");
            
            totalTimeText.text = minutes + ":" + seconds;
        }
    }
}