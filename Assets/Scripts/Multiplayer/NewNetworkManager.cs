using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;
using Random = UnityEngine.Random;

namespace Multiplayer
{
    public class NewNetworkManager : MonoBehaviourPunCallbacks
    {
        private string _roomName;
        private string _roomCode;
        private string _playerName;
        private Dictionary<int, GameObject> playerMenuItemDictionary = new Dictionary<int, GameObject>(); 
        
        [Header("Game Panels and UI")]
        public GameObject namePanel; //where the player submits their name.
        public GameObject gameSetupPanel; //where the player chooses to either join or create a game.
        public GameObject createdRoomPanel; //where named players congregate and get ready to play.
        public Transform playerListHolder;
        public GameObject joinRoomPanel; //Where players input the room name to join.
        public GameObject playerListItemPrefab;
        public GameObject playerNetworkPrefab;
        public TextMeshProUGUI roomCodeText;
        public TextMeshProUGUI roomNameText;
        

        public string RoomName
        {
            get => _roomName;
            set => _roomName = value;
        }

        public string PlayerName
        {
            get => _playerName;
            set => _playerName = value;
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            
        }
        //Connect to photon

        public void TryConnectPhoton()
        {
            if (_playerName.IsNullOrEmpty())
            {
                Debug.Log("Invalid Player Name : RETRY");
                return;
            }
            Connect();
            namePanel.SetActive(false);
        }
        
        public void Connect()
        {
            //if we are not connected to PhotonNetwork, then let's connect
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.LocalPlayer.NickName = _playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        
        public override void OnConnected()
        {
            Debug.Log("Connection established with Photon");
        }
        
        public override void OnConnectedToMaster()
        {
            Debug.Log(PhotonNetwork.LocalPlayer.NickName + " got connected");
            gameSetupPanel.SetActive(true);
        }

        //Create Room

        public void TryCreateNewRoom()
        {
            _roomCode = Random.Range(10000, 99999).ToString();
            PhotonNetwork.CreateRoom(_roomCode);
        }
        
        public override void OnCreatedRoom()
        {
            Debug.Log("Room created successfully");
            gameSetupPanel.SetActive(false);
            roomCodeText.gameObject.SetActive(true);
            roomCodeText.text = _roomCode;
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            CreatePlayerListItem(newPlayer);
            Debug.Log("Current Player Count: " + PhotonNetwork.CurrentRoom.PlayerCount);
        }
        
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("<color = red> Failed to create room. Error Code: " + returnCode + "msg = " + message + " </color>");
        }
        //Find Room via name
        
        //Join Room
        public void TryJoinRoom()
        {
            if (!_roomName.IsNullOrEmpty())
            {
                PhotonNetwork.JoinRoom(_roomName);
            }
        }

        public override void OnJoinedRoom()
        {
            //base.OnJoinedRoom();
            joinRoomPanel.SetActive(false);
            createdRoomPanel.SetActive(true);
            roomNameText.text = PhotonNetwork.PlayerList[0].NickName + "'s Room";
            foreach(Player p in PhotonNetwork.PlayerList)
            {
                CreatePlayerListItem(p);
            }
        }
        
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            //base.OnJoinRoomFailed(returnCode, message);
            Debug.Log("Join random room failed with message = " + message + " Code = " + returnCode);
        }

        private void CreatePlayerListItem(Player newPlayer)
        {
            GameObject item = Instantiate(playerListItemPrefab, playerListHolder);
            item.GetComponent<PlayerListItem>().Init(newPlayer.NickName);
            playerMenuItemDictionary.Add(newPlayer.ActorNumber, item);
        }
        
        //Start Race!
        public void StartGame()
        {
            if (PhotonNetwork.PlayerList.Length < 1) return;
            foreach(Player p in PhotonNetwork.PlayerList)
            {
                GameObject go = PhotonNetwork.Instantiate(playerNetworkPrefab.name, Vector3.zero, Quaternion.identity);
                go.GetComponent<NetworkPlayerIdentity>().Init(p);
            }
            createdRoomPanel.SetActive(false);
        }

        //Leave Room

        public void LeaveRoom()
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                var objToDestroy = playerMenuItemDictionary[player.ActorNumber];
                playerMenuItemDictionary.Remove(player.ActorNumber);
                Destroy(objToDestroy);
            }
            PhotonNetwork.LeaveRoom();
        }
        
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log(otherPlayer.NickName + " Left the Room");
            GameObject objectToKill = playerMenuItemDictionary[otherPlayer.ActorNumber];
            playerMenuItemDictionary.Remove(otherPlayer.ActorNumber);
            Destroy(objectToKill);
        }

        //Close Room
    
    }
}
