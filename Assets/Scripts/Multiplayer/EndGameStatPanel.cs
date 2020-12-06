using System.Collections.Generic;
using Characters;
using TMPro;
using UnityEngine;

namespace Multiplayer
{
    public class EndGameStatPanel : MonoBehaviour
    {
        [SerializeField] private Transform contentHolder;
        [SerializeField] private GameObject contentPrefab;
        [SerializeField] private GameObject quitButton;
        [SerializeField] private GameObject winnerLabel;
        [SerializeField] private GameObject waitingForOtherPlayer;
        [SerializeField] private TextMeshProUGUI winningPlayerText;
        
        public void SpawnPlayerListItem(string playerName, List<PlayerLevelStats> stats)
        {
            EndPlayerListItem spawnedListItem = Instantiate(contentPrefab, contentHolder).GetComponent<EndPlayerListItem>();
            spawnedListItem.Init(playerName, stats);
        }

        public void DeclareWinner(string playerName)
        {
            winnerLabel.SetActive(true);
            winningPlayerText.text = playerName;
        }

        public void AllowExit()
        {
            quitButton.SetActive(true);
        }

        public void EndWaiting()
        {
            waitingForOtherPlayer.SetActive(false);
        }
    }
}