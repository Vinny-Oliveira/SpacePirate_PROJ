using System.Collections.Generic;
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
        private LevelThiefData _currentLevelData;
        private Thief _currentPlayer;
        private NewNetworkManager _networkManager;

        private Dictionary<string, LevelThiefData> _levelThiefData = new Dictionary<string, LevelThiefData>();
        
        public string PlayerName
        {
            get => _playerName;
            set => _playerName = value;
        }
        
        public bool PlayerExists => _currentPlayer != null;


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

            if (!_levelThiefData.ContainsKey(scene.name))
            {
                if (_currentLevelData != null)
                {                
                    _currentLevelData.TimeTaken = (int)_totalTime;
                }
                _totalTime = 0;
                _currentLevelData = new LevelThiefData();
                _levelThiefData.Add(scene.name, _currentLevelData);
            }
            else
            {
                _currentLevelData = _levelThiefData[scene.name];
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

        private void OnPlayerDied()
        {
            _currentLevelData.Deaths++;
            Debug.Log(_currentLevelData.Deaths);

        }

        private void OnMoveCompleted()
        {
            _currentLevelData.MovesMade++;
            Debug.Log(_currentLevelData.MovesMade);
        }

        private void Update()
        {
            if(!PlayerExists) return;
            _totalTime += Time.deltaTime;
        }
    }
}