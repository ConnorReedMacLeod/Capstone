using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class NetworkConnectionManager : MonoBehaviourPunCallbacks {

    public Button btnConnectMaster;
    public Button btnConnectRoom;
    public Slider sliderLevel;

    public PlayerSelector plyrselector1;
    public PlayerSelector plyrselector2;

    public Text txtDisplayMessage;

    public bool bOfflineMode;

    public bool bTriesToConnectToMaster;
    public bool bTriesToConnectToRoom;

    public bool bInMatch;

    public static int nMyLevel = 1;

    public static NetworkConnectionManager inst;

    public void Awake() {

        if(inst != null) {
            //If an static instance exists,
            // then panic!  Destroy ourselves
            Debug.LogError("Warning!  This singleton already exists (" + gameObject.name + "), so we shouldn't instantiate a new one");
            Destroy(gameObject);

        } else {
            inst = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public static NetworkConnectionManager Get() {

        if(inst == null) {
            Debug.LogError("Error! Static instance not set!");
        }

        return inst;
    }

    // Start is called before the first frame update
    void Start() {

        bTriesToConnectToMaster = false;
        bTriesToConnectToRoom = false;
        bInMatch = false;

        Debug.Log("Offline mode is " + bOfflineMode);
        PhotonNetwork.OfflineMode = bOfflineMode; //true would "fake" an online connection
    }

    public void ShowIfFindingRoom(MonoBehaviour uiElem) {
        if(uiElem != null) {
            uiElem.gameObject.SetActive(PhotonNetwork.IsConnected && bTriesToConnectToMaster == false
                && bTriesToConnectToRoom == false && PhotonNetwork.InRoom == false);
        }
    }

    // Update is called once per frame
    void Update() {

        //TODO:: Changing this on every frame seems atrocious
        if(btnConnectMaster != null) {
            btnConnectMaster.gameObject.SetActive(PhotonNetwork.IsConnected == false && bTriesToConnectToMaster == false);
        }

        ShowIfFindingRoom(btnConnectRoom);
        ShowIfFindingRoom(sliderLevel);
        ShowIfFindingRoom(plyrselector1);
        ShowIfFindingRoom(plyrselector2);

        if(txtDisplayMessage != null) {
            txtDisplayMessage.gameObject.SetActive(PhotonNetwork.InRoom);
            if(PhotonNetwork.InRoom) {
                txtDisplayMessage.text = "Waiting for players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers
                    + "\n with level " + PhotonNetwork.CurrentRoom.CustomProperties["lvl"] + " in room " + PhotonNetwork.CurrentRoom.Name;
            }
        }


        //We should direct loading the next level if we are the master client
        if(bInMatch == false &&
            PhotonNetwork.IsMasterClient &&
            ArePlayersConnected()) {
            Debug.Log("We now have enough players to start the match!");

            bInMatch = true;

            if(SceneManager.GetActiveScene().name == "_MATCH") {
                Debug.Log("We're already in the _MATCH scene, so no need to transfer to it");
            } else {
                PhotonNetwork.LoadLevel("_MATCH");
            }
        }

    }

    public bool ArePlayersConnected() {
        return PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;

    }


    public void OnClickConnectToMaster() {

        PhotonNetwork.NickName = "Name_" + Time.time; //sets the player's name
        PhotonNetwork.AutomaticallySyncScene = true; //PhotonNetwork.LoadLevel() will keep the same
                                                     // level for everyone in the room
        PhotonNetwork.GameVersion = "v1"; //Only players with the same game version can play together


        bTriesToConnectToMaster = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnClickConnectToRoom() {
        //If we're not connected to the network service, then we can't possibly join a room
        if(PhotonNetwork.IsConnected == false) return;

        Debug.Log("Trying to connect to level " + nMyLevel);

        bTriesToConnectToRoom = true;
        //PhotonNetwork.CreateRoom("Custom Name"); // Create a specific room - Error: OnCreateRoomFailed
        //PhotonNetwork.JoinRoom("Custom Name"); //Join a specific room - Error: OnJoinRoomFailed
        //PhotonNetwork.JoinRandomRoom(); //Join a random room - Error: OnJoinRandomRoomFailed

        ExitGames.Client.Photon.Hashtable expectedRoomProperties;
        byte nMaxPlayers;


        expectedRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "lvl", nMyLevel } };
        nMaxPlayers = (byte)CalcMaxPlayersInRoom();

        Debug.Log(nMaxPlayers);

        PhotonNetwork.JoinRandomRoom(expectedRoomProperties, nMaxPlayers);
    }

    //This currently will calculate the number of input types set to None, and expect one unique
    // client per needed player.  Will eventually need to expand to allow other clients to control
    // multiple players and to allow for spectators
    public int CalcMaxPlayersInRoom() {

        int nNeededPlayers = 1;
        for(int i = 0; i < Player.MAXPLAYERS; i++) {
            if(CharacterSelection.Get().arInputTypes[i] == Player.InputType.NONE) {
                nNeededPlayers++;
            }
        }

        return nNeededPlayers;
    }


    public override void OnDisconnected(DisconnectCause cause) {
        base.OnDisconnected(cause);

        //If we disconnected, then we know we're not trying to currently connect
        bTriesToConnectToMaster = false;
        bTriesToConnectToRoom = false;
        bInMatch = false;
        Debug.Log(cause);
    }

    public override void OnConnectedToMaster() {
        base.OnConnectedToMaster();
        bTriesToConnectToMaster = false;
        Debug.Log("Connected to Master!");

        if(bOfflineMode) {
            //Pretend like we clicked a button to join a room
            OnClickConnectToRoom();
        }
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();

        bTriesToConnectToRoom = false;
        Debug.Log("Master: " + PhotonNetwork.IsMasterClient +
            " | On Region: " + PhotonNetwork.CloudRegion +
            " | In Room: " + PhotonNetwork.CurrentRoom.Name +
            " | Level: " + PhotonNetwork.CurrentRoom.CustomProperties["lvl"] +
            " | Number of Players: " + PhotonNetwork.CurrentRoom.PlayerCount +
            " | Max Number of Players: " + PhotonNetwork.CurrentRoom.MaxPlayers);

        //Now that we've connected to the new room, we can send our character selections
        for(int i = 0; i < Player.MAXPLAYERS; i++) {
            CharacterSelection.Get().SubmitSelection(i);
        }

    }

    public static void SendEventToMaster(byte evtCode, object content) {

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient };
        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };

        PhotonNetwork.RaiseEvent(evtCode, content, raiseEventOptions, sendOptions);
    }

    public static void SendEventToClients(byte evtCode, object content) {

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };

        PhotonNetwork.RaiseEvent(evtCode, content, raiseEventOptions, sendOptions);
    }


    public override void OnJoinRandomFailed(short returnCode, string message) {
        base.OnJoinRandomFailed(returnCode, message);

        //If no random room is available to join, then we should make our own new one
        //The null is just not specifying a name - it'll create a random one

        //Debug.LogError("Failed to join a room: " + returnCode + " " + message);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "lvl", nMyLevel }, { "trn", 0 } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "type", "lvl" };

        roomOptions.MaxPlayers = (byte)CalcMaxPlayersInRoom();

        //Debug.Log("Creating a room with properties " + roomOptions.CustomRoomProperties["lvl"]);

        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log(message);
        bTriesToConnectToRoom = false;
    }

    //Don't need to extend OnCreateRoom since it will automatically call
    // OnJoinRoom for us which is all we really need


}
