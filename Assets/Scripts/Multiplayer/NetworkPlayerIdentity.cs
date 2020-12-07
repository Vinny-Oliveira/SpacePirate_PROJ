using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = System.Object;

namespace Multiplayer
{
    public class NetworkPlayerIdentity : MonoBehaviourPunCallbacks
    {
        private string _playerName;
        private float _totalTime;
        private PlayerLevelStats _currentPlayerLevelData;
        private Thief _currentPlayer;
        private NewNetworkManager _networkManager;
        private bool _hasFinishedRace;
        private float _fallOffTime = 1f;
        private EndGameStatPanel _endGamePanel;

        private Dictionary<string, PlayerLevelStats> _playerLevelStatsDictionary = new Dictionary<string, PlayerLevelStats>();
        
        public string PlayerName
        {
            get => _playerName;
            set => _playerName = value;
        }
        
        public bool PlayerExists => _currentPlayer != null;

        public bool HasFinishedRace
        {
            get => _hasFinishedRace;
            set => _hasFinishedRace = value;
        }


        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _networkManager = FindObjectOfType<NewNetworkManager>();
        }

        public void Init(Player p)
        {
            photonView.RPC(nameof(RPC_LoadLevel), p, null);
            
            if(!PhotonNetwork.IsMasterClient) return;
            foreach (Player newPlayer in PhotonNetwork.PlayerList)
            {
                Dictionary<Object,int> dataTosendDic = new Dictionary<Object, int>()
                {
                    {_networkManager._playerMenuItemDictionary[p.ActorNumber], newPlayer.ActorNumber}
                };

                photonView.RPC(nameof(_networkManager.RPC_UpdateDictionaries), RpcTarget.AllBuffered, dataTosendDic);
            }
        }
        
        [PunRPC]
        private void RPC_LoadLevel()
        {
            SceneManager.LoadScene("Level_1");
            SceneManager.sceneLoaded += OnSceneLoaded;
            _networkManager.createdRoomPanel.SetActive(false);
        }
        
        public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name == _networkManager.endSceneName)
            {
                photonView.RPC(nameof(RPC_DisplayResults), PhotonNetwork.LocalPlayer, null);

                // RPC_DisplayResults();
                SceneManager.sceneLoaded -= OnSceneLoaded;
                return;
            }
            if (!_playerLevelStatsDictionary.ContainsKey(scene.name))
            {
                if (_currentPlayerLevelData != null)
                {                
                    _currentPlayerLevelData.TimeTaken = (int)_totalTime;
                }
                _totalTime = 0;
                _currentPlayerLevelData = new PlayerLevelStats();
                _currentPlayerLevelData.LevelName = scene.name; 
                _playerLevelStatsDictionary.Add(scene.name, _currentPlayerLevelData);
            }
            else
            {
                _currentPlayerLevelData = _playerLevelStatsDictionary[scene.name];
            }
            _currentPlayer = FindObjectOfType<Thief>();
            // _currentPlayer.GetComponent<MeshRenderer>().material.SetColor("_MainColor", _playerCustimizationSo.ColorPropertyList);
            if (_currentPlayer == null)
            {
                // SceneManager.sceneLoaded -= OnSceneLoaded;
                // Destroy(_networkManager.gameObject);
                // Destroy(gameObject);
                // SceneManager.LoadScene("MainMenus");
                return;
            }
            _currentPlayer.MoveCompleted += OnMoveCompleted;
            _currentPlayer.ThiefDead += OnPlayerDied;
        }

        [PunRPC]
        private void RPC_DisplayResults()
        {
            StartCoroutine(StartScoreDisplay());
        }
        
        private void OnPlayerDied()
        {
            _currentPlayerLevelData.Deaths++;
            Debug.Log("Player"+ PhotonNetwork.LocalPlayer.ActorNumber +" has died, death count : "+ _currentPlayerLevelData.Deaths);

        }

        private void OnMoveCompleted()
        {
            _currentPlayerLevelData.MovesMade++;
            Debug.Log("Player"+ PhotonNetwork.LocalPlayer.ActorNumber +" made a move, move count : "+_currentPlayerLevelData.MovesMade);
        }

        private void Update()
        {
            if(!PlayerExists) return;
            _totalTime += Time.deltaTime;
        }

        private IEnumerator StartScoreDisplay()
        {
            List<NetworkPlayerIdentity> playersIdentites = _networkManager.GetPlayersIdentites();
            List<int> playerScores = new List<int>(playersIdentites.Count);
            _hasFinishedRace = true;
            _endGamePanel = FindObjectOfType<EndGameStatPanel>();
            
            for (int i = 0; i < playersIdentites.Count; i++)
            {
                List<PlayerLevelStats> playerStatsList = new List<PlayerLevelStats>();
                foreach (var kvp in playersIdentites[i]._playerLevelStatsDictionary)
                {
                    playerStatsList.Add(kvp.Value);
                }
                
                //Wait for them to finish
                while (!playersIdentites[i]._hasFinishedRace)
                {
                    yield return new WaitForSeconds(_fallOffTime);
                    if (_fallOffTime <= 15)
                    {
                        _fallOffTime += 2f;
                    }
                }
                // Display Scores : Already done above?
                // Populate List Item with this player
                _endGamePanel.SpawnPlayerListItem(playersIdentites[i]._playerName, playerStatsList);
            }
            _endGamePanel.EndWaiting();
            
            yield return new WaitForSeconds(1f);

            List<int> deathIndices = new List<int>();
            List<int> turnIndices = new List<int>();
            List<int> timeIndices = new List<int>();
            
            foreach (var kvp in _playerLevelStatsDictionary)
            {
                int leastDeathIndex = 0;
                int leastTurnIndex = 0;
                int leastTimeIndex = 0;

                int lowestDeathCount = playersIdentites[0]._playerLevelStatsDictionary[kvp.Key].Deaths;
                int lowesTurnCount = playersIdentites[0]._playerLevelStatsDictionary[kvp.Key].MovesMade;
                int lowestTimeCount = playersIdentites[0]._playerLevelStatsDictionary[kvp.Key].TimeTaken;
                
                for (int i = 1; i < playersIdentites.Count; i++)
                {
                    if (playersIdentites[i]._playerLevelStatsDictionary[kvp.Key].Deaths < lowestDeathCount)
                    {
                        leastDeathIndex = i;
                        lowestDeathCount = playersIdentites[i]._playerLevelStatsDictionary[kvp.Key].Deaths;
                        deathIndices.Clear();
                        deathIndices.Add(leastDeathIndex);

                    }
                    else if (playersIdentites[i]._playerLevelStatsDictionary[kvp.Key].Deaths == lowestDeathCount)
                    {
                        deathIndices.Add(leastDeathIndex);
                        deathIndices.Add(i);
                        leastDeathIndex = i;
                        lowestDeathCount = playersIdentites[i]._playerLevelStatsDictionary[kvp.Key].Deaths;
                    }
                    if (playersIdentites[i]._playerLevelStatsDictionary[kvp.Key].MovesMade < lowesTurnCount)
                    {
                        leastTurnIndex = i;
                        lowesTurnCount = lowestDeathCount = playersIdentites[i]._playerLevelStatsDictionary[kvp.Key].MovesMade;
                        turnIndices.Clear();
                        turnIndices.Add(leastTurnIndex);

                    }
                    else if (playersIdentites[i]._playerLevelStatsDictionary[kvp.Key].MovesMade == lowesTurnCount)
                    {
                        turnIndices.Add(leastTurnIndex);
                        turnIndices.Add(i);
                        leastTurnIndex = i;
                        lowesTurnCount = lowestDeathCount = playersIdentites[i]._playerLevelStatsDictionary[kvp.Key].MovesMade;
                    }
                    if (playersIdentites[i]._playerLevelStatsDictionary[kvp.Key].TimeTaken < lowestTimeCount)
                    {
                        leastTimeIndex = i;
                        lowestTimeCount = playersIdentites[i]._playerLevelStatsDictionary[kvp.Key].TimeTaken;
                        timeIndices.Clear();
                        timeIndices.Add(leastTimeIndex);

                    }
                    else if (playersIdentites[i]._playerLevelStatsDictionary[kvp.Key].TimeTaken == lowestTimeCount)
                    {
                        timeIndices.Add(leastTimeIndex);
                        timeIndices.Add(i);
                        leastTimeIndex = i;
                        lowestTimeCount = playersIdentites[i]._playerLevelStatsDictionary[kvp.Key].TimeTaken;
                    }
                }
            }

            for (int i = 0; i < deathIndices.Count; i++)
            {
                playerScores[deathIndices[i]]++;
            }
            for (int i = 0; i < turnIndices.Count; i++)
            {
                playerScores[turnIndices[i]]++;
            }
            for (int i = 0; i < timeIndices.Count; i++)
            {
                playerScores[timeIndices[i]]++;
            }

            int winningPlayerIndex = 0;
            int previousHighestScore = playerScores[0];
            bool tied = false;
            for (int i = 1; i < playerScores.Count; i++)
            {
                if (playerScores[i] > previousHighestScore)
                {
                    winningPlayerIndex = i;
                    tied = false;
                }
                else if (playerScores[i] == previousHighestScore)
                {
                    tied = true;
                }
            }

            if (tied)
            {
                _endGamePanel.GameTied();
            }
            else
            {
                // Declare winner
                _endGamePanel.DeclareWinner(playersIdentites[winningPlayerIndex]._playerName);
            }
            
            yield return new WaitForSeconds(3f);
            //End game?
            _endGamePanel.AllowExit();
        }

        private void OnDestroy()
        {
            Debug.Log("I'm Fucking dead");
        }
    }
}
