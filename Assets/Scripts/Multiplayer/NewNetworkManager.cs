﻿using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using WebSocketSharp;
using Random = UnityEngine.Random;
using Object = System.Object;

namespace Multiplayer
{
    public class NewNetworkManager : MonoBehaviourPunCallbacks
    {
        private string _roomName;
        private string _roomCode;
        private string _playerName;
        private PlayerCustimizationSO _playerCustimizationSo;
        public Dictionary<int, NetworkPlayerIdentity> _playerMenuItemDictionary = new Dictionary<int, NetworkPlayerIdentity>();
        public Hashtable playerCustomProperties = new Hashtable();
        
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
        public string endSceneName;
        
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

            if (PhotonNetwork.LocalPlayer.Equals(newPlayer))
            {
                if (!PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Started"))
                {
                    PhotonNetwork.LocalPlayer.CustomProperties.Add("Started", false);
                }
                else
                {
                    PhotonNetwork.LocalPlayer.CustomProperties["Started"] = false;
                }
                GameObject go = Instantiate(playerNetworkPrefab, Vector3.zero, Quaternion.identity);
                go.GetComponent<NetworkPlayerIdentity>().Init(PhotonNetwork.LocalPlayer);
                PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
            }

            // _playerMenuItemDictionary.Add(newPlayer.ActorNumber, item);
            
            // GameObject go = PhotonNetwork.Instantiate(playerNetworkPrefab.name, Vector3.zero, Quaternion.identity);
            // go.GetComponent<NetworkPlayerIdentity>().Init(newPlayer);
            // if (_playerMenuItemDictionary.ContainsKey(PhotonNetwork.LocalPlayer.ActorNumber))
            // {
            //     _playerMenuItemDictionary[newPlayer.ActorNumber] = go;
            //
            // }
            // else
            // {
            //     _playerMenuItemDictionary.Add(newPlayer.ActorNumber, go);
            // }
        }
        
        //Start Race!
        public void StartGame()
        {
            if(!PhotonNetwork.IsMasterClient) return;
            if (PhotonNetwork.PlayerList.Length < 1) return;

            foreach (var player in PhotonNetwork.PlayerList)
            {
                player.CustomProperties["Started"] = true;
                player.SetCustomProperties(player.CustomProperties);
            }
            // foreach(Player p in PhotonNetwork.PlayerList)
            // {
            //     if (p.UserId != PhotonNetwork.LocalPlayer.UserId)
            //     {
            //         GameObject go = Instantiate(playerNetworkPrefab, Vector3.zero, Quaternion.identity);
            //         go.GetComponent<NetworkPlayerIdentity>().Init(PhotonNetwork.LocalPlayer);
            //         if (_playerMenuItemDictionary.ContainsKey(PhotonNetwork.LocalPlayer.ActorNumber))
            //         {
            //             _playerMenuItemDictionary[p.ActorNumber] = go.GetComponent<NetworkPlayerIdentity>();
            //         }
            //         else
            //         {
            //             _playerMenuItemDictionary.Add(p.ActorNumber, go.GetComponent<NetworkPlayerIdentity>());
            //         }
            //     }
            //     //
            //     // Destroy(_playerMenuItemDictionary[PhotonNetwork.LocalPlayer.ActorNumber]);
            //     // _playerMenuItemDictionary[PhotonNetwork.LocalPlayer.ActorNumber] = go;
            // }

            // foreach (var kvp in _playerMenuItemDictionary)
            // {
            //     if(PhotonNetwork.LocalPlayer.NickName == PhotonNetwork.PlayerList[kvp.Key].NickName)
            //         kvp.Value.GetComponent<NetworkPlayerIdentity>().Init(PhotonNetwork.PlayerList[kvp.Key]);
            // }
            
            
            createdRoomPanel.SetActive(false);
        }
        

        //Leave Room

        public void LeaveRoom()
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                var objToDestroy = _playerMenuItemDictionary[player.ActorNumber];
                _playerMenuItemDictionary.Remove(player.ActorNumber);
                Destroy(objToDestroy);
            }
            PhotonNetwork.LeaveRoom();
        }
        
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log(otherPlayer.NickName + " Left the Room");
            GameObject objectToKill = _playerMenuItemDictionary[otherPlayer.ActorNumber].gameObject;
            _playerMenuItemDictionary.Remove(otherPlayer.ActorNumber);
            Destroy(objectToKill);
        }

        //Close Room


        public List<NetworkPlayerIdentity> GetPlayersIdentites()
        {
            List<NetworkPlayerIdentity> boop = new List<NetworkPlayerIdentity>();
            foreach (var kvp in _playerMenuItemDictionary)
            {
                boop.Add(kvp.Value.GetComponent<NetworkPlayerIdentity>());
            }

            return boop;
        }

        //Quit Multiplayer
        public void QuitMultiplayerSession()
        {
            PhotonNetwork.Disconnect();
            Destroy(gameObject);
        }

        [PunRPC]
        public void RPC_UpdateDictionaries(Dictionary<Object,int> parameters)
        {
            foreach (var kvp in parameters)
            {
                if (_playerMenuItemDictionary.ContainsKey(PhotonNetwork.LocalPlayer.ActorNumber))
                {
                    _playerMenuItemDictionary[kvp.Value] =  (NetworkPlayerIdentity)kvp.Key;
                }
                else
                {
                    _playerMenuItemDictionary.Add(kvp.Value, (NetworkPlayerIdentity)kvp.Key);
                }
            }
            
            // Debug.Log("Dictionary updated for player " + PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }
}