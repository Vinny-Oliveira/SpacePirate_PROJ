using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    private string _playerName;
    private string _roomName;

    public string playerName
    {
        set { _playerName = value; }
        get { return _playerName; }
    }

    //Setup a dynamic property to pass the room name to a roomName dynamic property here
    public string roomName
    {
        set { _roomName = value; }
        get { return _roomName; }
    }

    public TextMeshProUGUI playerNameError_tmp;

    //saved via the 'SetGameMode()' function below.
    private string _gameMode;


    public GameObject waitingToConnectPanel;
    public GameObject gamelobbyOptionsPanel;
    public GameObject joiningRoomPanel;


    // The panel to create a room by entering room name and selecting a game mode
    //drag and drop ref in inspector
    public GameObject createRoomPanel;

    //this is the transition panel that shoes a 'creating room' message while the room is being created on the photon server. 
    //  Drag/Drop the panel reference
    public GameObject creatingRoomPanel;


    //drag and drop the room info text here
    public UnityEngine.UI.Text roomInfoText;

    //grab playerList from Room User Panel
    public Transform playerListHolder;

    public GameObject playerListItemPrefab;

    public UnityEngine.UI.Button startGameBtn;

    //This is the panel that shows all users in the room and the option to select the car before we start the game. 
    public GameObject roomUserPanel;

    //keeps track of all playerList GOs instantiated so we can remove it from the display wen a player leaves the room.
    private Dictionary<int, GameObject> playerDictGOs;


    #region CONNECT_TO_SERVER
    //Master Client - > Is the one who creates a room 
    //Client -> all the other clients are the ones who join an existing room created by a master client

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    /// <summary>
    /// My custom function that will connect to the server
    /// </summary>
    void Connect()
    {
        //if we are not connected to PhotonNetwork, then let's connect
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
    }


    /// <summary>
    /// This will be called when the login button is pressed in the UI
    /// </summary>
    public void CreateOrJoinRoom()
    {
        if (string.IsNullOrEmpty(playerName))
        {
            playerNameError_tmp.text = "Please Enter a Nickname";
            Debug.Log("Player name not entered. Can't connect to server without it.");
            return;
        }
        // else
        Connect();

    }

    public override void OnConnected()
    {
        Debug.Log("Connection established with Photon");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " got connected");
        waitingToConnectPanel.SetActive(false);
        gamelobbyOptionsPanel.SetActive(true);

    }
    #endregion

    #region CREATE_ROOM
    /// <summary>
    /// This is called from the "CreateRoomPanel". A two letter 'game mode' code is passed depending on which chame mode check box is selected
    /// Race Mode: rm
    /// Death mode: dm
    /// </summary>

    public void SetGameMode(string gameMode)
    {
        _gameMode = gameMode;
    }

    // Called when we click 'CreateRoom button under CreateRoomPanel in the UI
    public void OnCreateRoomButtonPressed()
    {
        //if room name is not entered, then print debug log message and return. 
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.Log("<color=red> Please enter room name. Can't connect to server without it.</color>");
            return;
        }

        if (string.IsNullOrEmpty(_gameMode))
        {
            Debug.Log("<color=red> Please select a Game Mode. Can't connect to server without it.</color>");
            return;
        }

        createRoomPanel.SetActive(false);
        creatingRoomPanel.SetActive(true);
        // else
        CreateRoom();

    }

    private void CreateRoom()
    {
        Photon.Realtime.RoomOptions ro = new Photon.Realtime.RoomOptions();
        ro.MaxPlayers = 5;

        //we use 'm as a short hand property for denoting our gameMode
        ro.CustomRoomPropertiesForLobby = new string[] { "m" };

        ExitGames.Client.Photon.Hashtable gameRoomProperties = new ExitGames.Client.Photon.Hashtable()
        {
            {
                 "m", _gameMode
            }
        };

        ro.CustomRoomProperties = gameRoomProperties;


        PhotonNetwork.CreateRoom(roomName + UnityEngine.Random.Range(100000, 999999), ro);
        //SetGameMode(_gameMode);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("<color = green> Room created successfully </color>");
        creatingRoomPanel.SetActive(false);
        roomUserPanel.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        joiningRoomPanel.SetActive(false);
        roomUserPanel.SetActive(true);

        Debug.Log("<color = green> User: " + PhotonNetwork.LocalPlayer.NickName + "joined " + PhotonNetwork.CurrentRoom.Name + " </color>");
        //we need to print the properties of the room, such as room name, game mode, and list of players currently in the room. 

        roomInfoText.text = PhotonNetwork.CurrentRoom.Name + " | Players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

        // playerDictGOs is null, create the object for it
        if (playerDictGOs == null)
            playerDictGOs = new Dictionary<int, GameObject>();

        //Populate a list of players in the current room (populate player List UI)
        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
        {
            CreatePlayerListItem(p);
        }

        //1...room game mode
        //OUT parameter
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("m"))
        {
            object gameModeName = "";
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("m", out gameModeName))
            {
                Debug.Log("<color=magenta> Game Mode: " + gameModeName + "</color>");
            }
        }
    }

    private void CreatePlayerListItem(Player newPlayer)
    {
        GameObject item = Instantiate(playerListItemPrefab, playerListHolder);
        item.GetComponent<PlayerItemUIInfo>().Init(newPlayer.ActorNumber, newPlayer.NickName);

        playerDictGOs.Add(newPlayer.ActorNumber, item);

        object _isRemotePlayerReady;

        if (newPlayer.CustomProperties.TryGetValue("pReady", out _isRemotePlayerReady))
        {
            item.GetComponent<PlayerItemUIInfo>().SetReadyState((bool)_isRemotePlayerReady);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        CreatePlayerListItem(newPlayer);
        roomInfoText.text = PhotonNetwork.CurrentRoom.Name + " | Players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;

    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("<color = red> Failed to create room. Error Code: " + returnCode + "msg = " + message + " </color>");
    }

    #endregion

    #region UPDATE_PLAYER_PROPERTIES

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        object _isRemotePlayerReady;
        if (changedProps.TryGetValue("pReady", out _isRemotePlayerReady))
        {

            playerDictGOs[targetPlayer.ActorNumber].GetComponent<PlayerItemUIInfo>().SetReadyState((bool)_isRemotePlayerReady);
        }
        startGameBtn.interactable = IsGameReadyToStart();
    }


    #endregion

    #region JOIN_RANDOM_ROOM

    //Add an event on 'JoinRandomRoom' button under GameOptions panel and call this function on the button's click

    //call this on the two buttons: Racing GameMode and DeathGameMode under JoinRandomRoomPanel > Backgound > GameModes > 
    //Race mode = rm
    //Death Mode = dm
    public void OnJoinRandomRoomGameModeTypeClicked(string gameModeCode)
    {
        Debug.Log("Trying to find a random room of " + gameModeCode + " game type");

        ExitGames.Client.Photon.Hashtable expectedProperties = new ExitGames.Client.Photon.Hashtable
        {
            {"m", gameModeCode}
        };
        PhotonNetwork.JoinRandomRoom(expectedProperties, 0);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Join random room failed with message = " + message);

        Transform failedPanelTrans = joiningRoomPanel.transform.Find("FailedPanel");


        if (failedPanelTrans != null)
        {
            failedPanelTrans.gameObject.SetActive(true);
            Debug.Log("Couldn't Joing the room");
            //failedPanelTrans.GetComponent<ErrorMessageDisplay>().DisplayMessage("Couldnt join a random room. Error message: " + message, CallOnRandomRoomFailedAfterDelay);
        }

        //gamelobbyOptionsPanel.SetActive(true);
        //joiningRandomRoomPanel.SetActive(false);

        //Lets create a room instrad with a gamemode that the user tried to join for.
        //CreateRoom();
        //Debug.Log("new room created");

    }

    void CallOnRandomRoomFailedAfterDelay()
    {
        Invoke("OnRandomRoomFailed", 3.0f);
    }

    void OnRandomRoomFailed()
    {
        joiningRoomPanel.SetActive(false);
        gamelobbyOptionsPanel.SetActive(true);
    }

    #endregion

    #region GAME_START_FUNCTION

    //A private function that returns true if the game is ready to start

    private bool IsGameReadyToStart()
    {
        //This check should only happen on the MasterClient's side.
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isRemotePlayerReady;

            if (p.CustomProperties.TryGetValue("pReady", out isRemotePlayerReady))
            {
                if (!(bool)isRemotePlayerReady)
                {
                    return false;
                }
            }
            else
            {
                //error, cant find pReady property. maybe we misspelled either here or in playerItemInfoUI. both should match...
                return false;
            }

        }
        return true;
    }

    #endregion

    #region START_GAME

    //Hook this up to the "start Game button" in room user panel
    public void OnStartGameButtonClicked()
    {
        object gameModeCode;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("m", out gameModeCode))
        {
            if ((string)gameModeCode == "rm")
            {
                PhotonNetwork.LoadLevel("RaceModeLevel");
            }
            else if ((string)gameModeCode == "dm")
            {
                PhotonNetwork.LoadLevel("DeathModeLevel");
            }
            else
            {
                Debug.Log("Didnt recognize game mode code: " + gameModeCode);
                //for your project, you will show the errors/warnings in the UI, not in Debug
            }
        }
        else
        {
            Debug.Log("Can't find 'm' property in the room");
        }


    }

    #endregion

}
