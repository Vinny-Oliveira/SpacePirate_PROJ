using System.Collections;
using System.Collections.Generic;
using Characters;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                DisplayResults();
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
            if (_currentPlayer == null)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                Destroy(_networkManager.gameObject);
                Destroy(gameObject);
                SceneManager.LoadScene("MainMenus");
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
            Debug.Log(_currentPlayerLevelData.Deaths);

        }

        private void OnMoveCompleted()
        {
            _currentPlayerLevelData.MovesMade++;
            Debug.Log(_currentPlayerLevelData.MovesMade);
        }

        private void Update()
        {
            if(!PlayerExists) return;
            _totalTime += Time.deltaTime;
        }

        private IEnumerator StartScoreDisplay()
        {
            var boop = _networkManager.GetPlayersIdentites();

            for (int i = 0; i < boop.Count; i++)
            {
                // Populate List Item with this player
                
                //Wait for them to finish
                while (!boop[i]._hasFinishedRace)
                {
                    yield return new WaitForSeconds(_fallOffTime);
                    _fallOffTime += 2f;
                }
                
                // Display Scores
            }
            // Declare winner
            
            //End game?
        }
    }
}
