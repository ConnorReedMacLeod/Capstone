using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class NetworkConnectionManager : MonoBehaviourPunCallbacks {

    public Button btnConnectMaster;
    public Button btnConnectRoom;
    public Slider sliderLevel;
    public Dropdown dropdownMatchType;

    public Text txtDisplayMessage;

    public bool bTriesToConnectToMaster;
    public bool bTriesToConnectToRoom;

    public bool bInMatch;

    public static int nMyLevel = 1;

    public enum MATCHTYPE { SOLO, STANDARD };
    public static MATCHTYPE matchType;

    // Start is called before the first frame update
    void Start() {

        DontDestroyOnLoad(gameObject);
        bTriesToConnectToMaster = false;
        bTriesToConnectToRoom = false;
        bInMatch = false;
    }

    // Update is called once per frame
    void Update() {

        //TODO:: Changing this on every frame seems atrocious
        if (btnConnectMaster != null) {
            btnConnectMaster.gameObject.SetActive(PhotonNetwork.IsConnected == false && bTriesToConnectToMaster == false);
        }
        if (btnConnectRoom != null) {
            btnConnectRoom.gameObject.SetActive(PhotonNetwork.IsConnected && bTriesToConnectToMaster == false
                && bTriesToConnectToRoom == false && PhotonNetwork.InRoom == false);
        }
        if (sliderLevel != null) {
            sliderLevel.gameObject.SetActive(PhotonNetwork.IsConnected && bTriesToConnectToMaster == false
                && bTriesToConnectToRoom == false && PhotonNetwork.InRoom == false);
        }
        if (txtDisplayMessage != null) {
            txtDisplayMessage.gameObject.SetActive(PhotonNetwork.InRoom);
            if (PhotonNetwork.InRoom) {
                txtDisplayMessage.text = "Waiting for players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers
                    + "\n with level " + PhotonNetwork.CurrentRoom.CustomProperties["lvl"] + " in room " + PhotonNetwork.CurrentRoom.Name;
            }
        }

        if (dropdownMatchType != null) {
            dropdownMatchType.gameObject.SetActive(PhotonNetwork.IsConnected && bTriesToConnectToMaster == false
                && bTriesToConnectToRoom == false && PhotonNetwork.InRoom == false);
        }

        //We should direct loading the next level if we are the master client
        if (bInMatch == false &&
            PhotonNetwork.IsMasterClient &&
            PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers) {
            Debug.Log("We now have enough players to start the match!");

            bInMatch = true;

            PhotonNetwork.LoadLevel("_MATCH");
        }

    }


    public void OnClickConnectToMaster() {
        //Settings (optional for tutorial purposes)
        PhotonNetwork.OfflineMode = false; //true would "fake" an online connection
        PhotonNetwork.NickName = "Name_" + Time.time; //sets the player's name
        PhotonNetwork.AutomaticallySyncScene = true; //PhotonNetwork.LoadLevel() will keep the same
                                                     // level for everyone in the room
        PhotonNetwork.GameVersion = "v1"; //Only players with the same game version can play together


        bTriesToConnectToMaster = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnClickConnectToRoom() {
        //If we're not connected to the network service, then we can't possibly join a room
        if (PhotonNetwork.IsConnected == false) return;

        Debug.Log("Trying to connect to level " + nMyLevel);

        bTriesToConnectToRoom = true;
        //PhotonNetwork.CreateRoom("Custom Name"); // Create a specific room - Error: OnCreateRoomFailed
        //PhotonNetwork.JoinRoom("Custom Name"); //Join a specific room - Error: OnJoinRoomFailed
        //PhotonNetwork.JoinRandomRoom(); //Join a random room - Error: OnJoinRandomRoomFailed

        ExitGames.Client.Photon.Hashtable expectedRoomProperties;
        byte nMaxPlayers;

        switch (matchType) {
            case MATCHTYPE.SOLO:
                expectedRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "type", matchType } };
                nMaxPlayers = 1;
                break;

            case MATCHTYPE.STANDARD:
                expectedRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "type", matchType }, { "lvl", nMyLevel } };
                nMaxPlayers = 2;
                break;

            default:
                nMaxPlayers = 0;
                expectedRoomProperties = new ExitGames.Client.Photon.Hashtable();
                break;

        }

        PhotonNetwork.JoinRandomRoom(expectedRoomProperties, nMaxPlayers);
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
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();

        bTriesToConnectToRoom = false;
        Debug.Log("Master: " + PhotonNetwork.IsMasterClient +
            " | On Region: " + PhotonNetwork.CloudRegion +
            " | In Room: " + PhotonNetwork.CurrentRoom.Name +
            " | Level: " + PhotonNetwork.CurrentRoom.CustomProperties["lvl"] +
            " | Number of Players: " + PhotonNetwork.CurrentRoom.PlayerCount);

    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        base.OnJoinRandomFailed(returnCode, message);

        //If no random room is available to join, then we should make our own new one
        //The null is just not specifying a name - it'll create a random one

        //Debug.LogError("Failed to join a room: " + returnCode + " " + message);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "type", matchType }, { "lvl", nMyLevel }, { "trn", 0 } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "type", "lvl" };

        switch (matchType) {
            case MATCHTYPE.SOLO:

                roomOptions.MaxPlayers = 1;
                break;

            case MATCHTYPE.STANDARD:

                roomOptions.MaxPlayers = 2;
                break;

            default:
                roomOptions.MaxPlayers = 0;
                break;

        }

        Debug.Log("Creating a room with properties " + roomOptions.CustomRoomProperties["lvl"]);

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
