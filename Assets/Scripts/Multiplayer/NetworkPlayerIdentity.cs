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
            // photonView.RPC(nameof(RPC_LoadLevel), p, null);

            if (PhotonNetwork.LocalPlayer.Equals(p))
            {
                StartCoroutine(WaitForGameStart());
                if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("HasFinished"))
                {
                    PhotonNetwork.LocalPlayer.CustomProperties.Add("HasFinished", false);
                }
                else
                {
                    PhotonNetwork.LocalPlayer.CustomProperties["HasFinished"] = false;
                }
                _playerName = p.NickName;
                PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);

            }

            // PhotonNetwork.LocalPlayer.CustomProperties.Add("HasFinished", false);

            // if(!PhotonNetwork.IsMasterClient) return;
            // foreach (Player newPlayer in PhotonNetwork.PlayerList)
            // {
            //     Dictionary<Object,int> dataTosendDic = new Dictionary<Object, int>()
            //     {
            //         {_networkManager._playerMenuItemDictionary[p.ActorNumber], newPlayer.ActorNumber}
            //     };
            //
            //     photonView.RPC(nameof(_networkManager.RPC_UpdateDictionaries), RpcTarget.AllBuffered, dataTosendDic);
            // }
        }

        private IEnumerator WaitForGameStart()
        {
            bool shouldStart = (bool)PhotonNetwork.LocalPlayer.CustomProperties["Started"];

            while (!shouldStart)
            {
                yield return new WaitForSeconds(1f);

                shouldStart = (bool)PhotonNetwork.LocalPlayer.CustomProperties["Started"];
                Debug.Log("VALUE OF STARTED BOOL for player: " +PhotonNetwork.LocalPlayer.NickName +" is " + (shouldStart));
                foreach (var player in PhotonNetwork.PlayerList)
                {
                    Debug.Log("VALUE OF STARTED BOOL for player on server is : " + player.NickName +" is " + (shouldStart));

                }
            }
            
            RPC_LoadLevel();
        }

        // [PunRPC]
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
                _currentPlayerLevelData.TimeTaken = (int)_totalTime;

                if (_currentPlayerLevelData != null)
                {                
                    byte[] data = new byte[]
                    {
                        (byte) _currentPlayerLevelData.Deaths, 
                        (byte) _currentPlayerLevelData.MovesMade, 
                        (byte)_currentPlayerLevelData.TimeTaken
                    };
                    
                    if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(_currentPlayerLevelData.LevelName))
                    {
                        PhotonNetwork.LocalPlayer.CustomProperties.Add(_currentPlayerLevelData.LevelName, data);
                    }
                    else
                    {
                        PhotonNetwork.LocalPlayer.CustomProperties[_currentPlayerLevelData.LevelName] = data;
                    }
                    // _networkManager.playerCustomProperties.Add(_currentPlayerLevelData.LevelName, data);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
                    
                    _currentPlayerLevelData.TimeTaken = (int)_totalTime;
                }
                DisplayResults();

                // RPC_DisplayResults();
                SceneManager.sceneLoaded -= OnSceneLoaded;
                return;
            }
            if (!_playerLevelStatsDictionary.ContainsKey(scene.name))
            {

                if (_currentPlayerLevelData != null)
                {                
                    _currentPlayerLevelData.TimeTaken = (int)_totalTime;

                    byte[] data = new byte[]
                    {
                        (byte) _currentPlayerLevelData.Deaths, 
                        (byte) _currentPlayerLevelData.MovesMade, 
                        (byte)_currentPlayerLevelData.TimeTaken
                    };
                    
                    if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey(_currentPlayerLevelData.LevelName))
                    {
                        PhotonNetwork.LocalPlayer.CustomProperties.Add(_currentPlayerLevelData.LevelName, data);
                    }
                    else
                    {
                        PhotonNetwork.LocalPlayer.CustomProperties[_currentPlayerLevelData.LevelName] = data;
                    }
                    // _networkManager.playerCustomProperties.Add(_currentPlayerLevelData.LevelName, data);
                    PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
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

        private void DisplayResults()
        {
            StartCoroutine(StartScoreDisplay());
        }
        
        private void OnPlayerDied()
        {
            _currentPlayerLevelData.Deaths++;
            // Debug.Log("Player"+ PhotonNetwork.LocalPlayer.ActorNumber +" has died, death count : "+ _currentPlayerLevelData.Deaths);
        }

        private void OnMoveCompleted()
        {
            _currentPlayerLevelData.MovesMade++;
            // Debug.Log("Player"+ PhotonNetwork.LocalPlayer.ActorNumber +" made a move, move count : "+_currentPlayerLevelData.MovesMade);
        }

        private void Update()
        {
            if(!PlayerExists) return;
            _totalTime += Time.deltaTime;
        }

        private IEnumerator StartScoreDisplay()
        {

            // List<int> playerScores = new List<int>(PhotonNetwork.PlayerList.Length);
            PhotonNetwork.LocalPlayer.CustomProperties["HasFinished"] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);

            _endGamePanel = FindObjectOfType<EndGameStatPanel>();
            List<String> levelNames = new List<string>();
            foreach (var kvp in _playerLevelStatsDictionary)
            {
                levelNames.Add(kvp.Key);
            }
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                //Wait for them to finish
                bool hasFinished = (bool)PhotonNetwork.PlayerList[i].CustomProperties["HasFinished"];
                
                while (!hasFinished)
                {
                    yield return new WaitForSeconds(_fallOffTime);
                    if (_fallOffTime <= 15)
                    {
                        _fallOffTime += 2f;
                    }
                    hasFinished = (bool)PhotonNetwork.PlayerList[i].CustomProperties["HasFinished"];
                }
                
                List<PlayerLevelStats> playerStatsList = new List<PlayerLevelStats>();
                
                foreach (var nameOfLevel in levelNames)
                {
                    PlayerLevelStats statsToAdd = new PlayerLevelStats();
                    byte[] data = (byte[])PhotonNetwork.PlayerList[i].CustomProperties[nameOfLevel];
                    statsToAdd.Deaths = data[0];
                    statsToAdd.MovesMade = data[1];
                    statsToAdd.TimeTaken = data[2];
                    playerStatsList.Add(statsToAdd);
                }

                // Display Scores : Already done above?
                // Populate List Item with this player
                _endGamePanel.SpawnPlayerListItem(PhotonNetwork.PlayerList[i].NickName, playerStatsList);
            }
            _endGamePanel.EndWaiting();
            
            yield return new WaitForSeconds(1f);

            List<int> deathIndices = new List<int>();
            List<int> turnIndices = new List<int>();
            List<int> timeIndices = new List<int>();
            foreach (var nameOfLevel in levelNames)
            {
                
                byte[] data = (byte[])PhotonNetwork.PlayerList[0].CustomProperties[nameOfLevel];
                    
                int leastDeathIndex = 0;
                int leastTurnIndex = 0;
                int leastTimeIndex = 0;

                int lowestDeathCount = data[0];
                int lowesTurnCount = data[1];
                int lowestTimeCount = data[2];
                
                for (int i = 1; i < PhotonNetwork.PlayerList.Length; i++)
                {
                    byte[] newData = (byte[])PhotonNetwork.PlayerList[i].CustomProperties[nameOfLevel];

                    if (newData[0] < lowestDeathCount)
                    {
                        leastDeathIndex = i;
                        lowestDeathCount = newData[0];
                        deathIndices.Clear();
                        deathIndices.Add(leastDeathIndex);

                    }
                    else if (newData[0] == lowestDeathCount)
                    {
                        deathIndices.Add(leastDeathIndex);
                        deathIndices.Add(i);
                        leastDeathIndex = i;
                        lowestDeathCount = newData[0];
                    }
                    if (newData[1] < lowesTurnCount)
                    {
                        leastTurnIndex = i;
                        lowesTurnCount = lowestDeathCount = newData[1];
                        turnIndices.Clear();
                        turnIndices.Add(leastTurnIndex);
                    }
                    else if (newData[1] == lowesTurnCount)
                    {
                        turnIndices.Add(leastTurnIndex);
                        turnIndices.Add(i);
                        leastTurnIndex = i;
                        lowesTurnCount = lowestDeathCount = newData[1];
                    }
                    if (newData[2] < lowestTimeCount)
                    {
                        leastTimeIndex = i;
                        lowestTimeCount = newData[2];
                        timeIndices.Clear();
                        timeIndices.Add(leastTimeIndex);

                    }
                    else if (newData[2] == lowestTimeCount)
                    {
                        timeIndices.Add(leastTimeIndex);
                        timeIndices.Add(i);
                        leastTimeIndex = i;
                        lowestTimeCount = newData[2];
                    }
                    UpdateScore(leastDeathIndex);
                    UpdateScore(leastTimeIndex);
                    UpdateScore(leastTimeIndex);

                    // playerScores[leastDeathIndex]++;
                    // playerScores[leastTimeIndex]++;
                    // playerScores[leastTimeIndex]++;
                }
                // for (int i = 0; i < deathIndices.Count; i++)
                // {
                //     playerScores[deathIndices[i]]++;
                // }
                // for (int i = 0; i < turnIndices.Count; i++)
                // {
                //     playerScores[turnIndices[i]]++;
                // }
                // for (int i = 0; i < timeIndices.Count; i++)
                // {
                //     playerScores[timeIndices[i]]++;
                // }
            }
            
            int winningPlayerIndex = 0;
            int previousHighestScore = (int)PhotonNetwork.PlayerList[0].CustomProperties["Score"];
            bool tied = false;
            // for (int i = 1; i < playerScores.Count; i++)
            // {
            //     if (playerScores[i] > previousHighestScore)
            //     {
            //         winningPlayerIndex = i;
            //         tied = false;
            //     }
            //     else if (playerScores[i] == previousHighestScore)
            //     {
            //         tied = true;
            //     }
            // }

            if (tied)
            {
                _endGamePanel.GameTied();
            }
            else
            {
                // Declare winner
                _endGamePanel.DeclareWinner(PhotonNetwork.PlayerList[winningPlayerIndex].NickName);
            }
            
            yield return new WaitForSeconds(3f);
            //End game?
            _endGamePanel.AllowExit();
        }

        private static void UpdateScore(int atIndex)
        {
            if (!PhotonNetwork.PlayerList[atIndex].CustomProperties.ContainsKey("Score"))
            {
                PhotonNetwork.PlayerList[atIndex].CustomProperties.Add("Score", 1);
            }
            else
            {
                int value = (int) PhotonNetwork.LocalPlayer.CustomProperties["Score"];
                PhotonNetwork.PlayerList[atIndex].CustomProperties["Score"] = value + 1;
            }

            // _networkManager.playerCustomProperties.Add(_currentPlayerLevelData.LevelName, data);
            PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.PlayerList[atIndex].CustomProperties);
        }

        private void OnDestroy()
        {
            Debug.Log("I'm Fucking dead");
        }
    }
}
