﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class NetworkConnectionManager : MonoBehaviourPunCallbacks {

    public Button btnConnectMaster;

    public Button btnConnectSoloRoom;
    public Button btnConnectPVPRoom;

    public Slider sliderLevel;

    public Button btnStartDraft;
    public Button btnDirectToMatch;

    public PlayerSelector plyrselector1;
    public PlayerSelector plyrselector2;

    public LoadLogfileSelect loadlogfileselect;

    public Text txtDisplayMessage;

    public bool bOfflineMode;

    public bool bTriesToConnectToMaster;
    public bool bTriesToConnectToRoom;

    public bool bInMatch;

    public static int nMyLevel = 1;

    public int nMostRecentMaxPlayersInRoom; //Set to whatever max-players amount we've mostly recently queue'd up with

    public static NetworkConnectionManager inst;

    public void Awake() {

        if(inst != null) {
            //If a static instance exists,
            // then panic!  Destroy ourselves
            //Debug.Log("Warning!  This singleton already exists (" + gameObject.name + "), so we shouldn't instantiate a new one");
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

    public void ShowIfAllPlayersConnectedInRoom(MonoBehaviour uiElem) {
        if(uiElem != null) {
            uiElem.gameObject.SetActive(PhotonNetwork.IsConnected && PhotonNetwork.InRoom && ArePlayersConnected());
        }
    }

    public void ShowIfAllPlayersConnectedInRoomAndMaster(MonoBehaviour uiElem) {
        if(uiElem != null) {
            uiElem.gameObject.SetActive(PhotonNetwork.IsConnected && PhotonNetwork.InRoom && ArePlayersConnected() && PhotonNetwork.IsMasterClient);
        }
    }

    // Update is called once per frame
    void Update() {

        //TODO:: Changing this on every frame seems atrocious
        if(btnConnectMaster != null) {
            btnConnectMaster.gameObject.SetActive(PhotonNetwork.IsConnected == false && bTriesToConnectToMaster == false);
        }

        ShowIfFindingRoom(btnConnectSoloRoom);
        ShowIfFindingRoom(btnConnectPVPRoom);
        ShowIfFindingRoom(sliderLevel);

        ShowIfAllPlayersConnectedInRoom(plyrselector1);
        ShowIfAllPlayersConnectedInRoom(plyrselector2);

        ShowIfAllPlayersConnectedInRoomAndMaster(btnStartDraft);
        ShowIfAllPlayersConnectedInRoomAndMaster(btnDirectToMatch);

        ShowIfAllPlayersConnectedInRoomAndMaster(loadlogfileselect);

        if(txtDisplayMessage != null) {
            txtDisplayMessage.gameObject.SetActive(PhotonNetwork.InRoom);
            if(PhotonNetwork.InRoom && ArePlayersConnected() == false) {
                txtDisplayMessage.text = "Waiting for players: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers
                    + "\n with level " + PhotonNetwork.CurrentRoom.CustomProperties["lvl"] + " in room " + PhotonNetwork.CurrentRoom.Name;
            } else if(PhotonNetwork.InRoom) {
                txtDisplayMessage.text = "All players connected: Submit your selections and wait for master to progress to draft/match";
            }
        }

    }

    public void InitRandomization() {
        //Attempt to set the randomization seed (we'll only succeed if we're the master, but we should still try)
        NetworkMatchSetup.SetRandomizationSeed(Random.Range(0, 1000000));
    }

    public void TransferToDraftScene() {
        if(PhotonNetwork.IsMasterClient == false) {
            Debug.LogError("A non-master tried to move to the draft phase - ignoring");
            return;
        } else if(ArePlayersConnected() == false) {
            Debug.LogError("Tried to move to draft when not all characters are connected");
            return;
        }

        if(SceneManager.GetActiveScene().name == "_DRAFT") {
            Debug.Log("We're already in the _DRAFT scene, so no need to transfer to it");
        } else {
            Debug.Log("We as the master are moving us to the Draft scene");
            PhotonNetwork.LoadLevel("_DRAFT");
        }
    }

    public void TransferToLoadoutScene() {
        if (PhotonNetwork.IsMasterClient == false) {
            Debug.LogError("A non-master tried to move directly to the loadout phase - ignoring");
            return;
        } else if (ArePlayersConnected() == false) {
            Debug.LogError("Tried to move directly to the loadout phase when not all characters are connected");
            return;
        }

        if (SceneManager.GetActiveScene().name == "_LOADOUT") {
            Debug.Log("We're already in the _LOADOUT scene, so no need to transfer to it");
        } else {

            Debug.Log("We as the master are moving us to the Loadout scene");
            PhotonNetwork.LoadLevel("_LOADOUT");
        }
    }

    public void TransferToMatchScene() {
        if(PhotonNetwork.IsMasterClient == false) {
            Debug.LogError("A non-master tried to move directly to match - ignoring");
            return;
        } else if(ArePlayersConnected() == false) {
            Debug.LogError("Tried to move to directly to match when not all characters are connected");
            return;
        } else if(NetworkMatchSetup.HasAllMatchSetupInfo() == false) {
            Debug.LogError("MatchSetup is not filled out enough to start a match");
            return;
        }

        if(SceneManager.GetActiveScene().name == "_MATCH") {
            Debug.Log("We're already in the _MATCH scene, so no need to transfer to it");
        } else {
            Debug.Log(NetworkMatchSetup.MatchSetupToString());
            Debug.Log("We as the master are moving us to the Match scene");
            PhotonNetwork.LoadLevel("_MATCH");
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


    public void OnClickJoinSoloRoom() {
        OnClickConnectToRoom(1);
    }

    public void OnClickJoinPVPRoom() {
        OnClickConnectToRoom(2);
    }


    public void OnClickConnectToRoom(int nMaxPlayersInRoom) {
        //If we're not connected to the network service, then we can't possibly join a room
        if(PhotonNetwork.IsConnected == false) return;

        Debug.Log("Trying to connect to level " + nMyLevel + " with " + nMaxPlayersInRoom + " max players");

        bTriesToConnectToRoom = true;
        //PhotonNetwork.CreateRoom("Custom Name"); // Create a specific room - Error: OnCreateRoomFailed
        //PhotonNetwork.JoinRoom("Custom Name"); //Join a specific room - Error: OnJoinRoomFailed
        //PhotonNetwork.JoinRandomRoom(); //Join a random room - Error: OnJoinRandomRoomFailed

        ExitGames.Client.Photon.Hashtable expectedRoomProperties;

        nMostRecentMaxPlayersInRoom = nMaxPlayersInRoom;

        expectedRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "lvl", nMyLevel } };

        PhotonNetwork.JoinRandomRoom(expectedRoomProperties, (byte)nMaxPlayersInRoom);
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

        if (bOfflineMode) {
            //Pretend like we clicked a button to join a room
            OnClickJoinSoloRoom();
        }
    }


    public void SpawnSceneNetworkManager(string sPrefabName) {
        GameObject goSceneNetworkManager;

        //Debug.LogFormat("Spawning {0}", sPrefabName);

        //Spawn the networking manager for the local player
        goSceneNetworkManager = PhotonNetwork.InstantiateSceneObject(string.Format("Prefabs/Networking/{0}", sPrefabName), Vector3.zero, Quaternion.identity);

        if (goSceneNetworkManager = null) {
            Debug.LogErrorFormat("No prefab found for {0}", sPrefabName);
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

        InitRandomization();
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        base.OnJoinRandomFailed(returnCode, message);

        //If no random room is available to join, then we should make our own new one
        //The null is just not specifying a name - it'll create a random one

        //Debug.LogError("Failed to join a room: " + returnCode + " " + message);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "lvl", nMyLevel } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "type", "lvl" };

        //Set the max players to be the amount we most recently queue'd up for
        roomOptions.MaxPlayers = (byte)nMostRecentMaxPlayersInRoom;

        //Debug.Log("Creating a room with properties " + roomOptions.CustomRoomProperties["lvl"]);

        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log(message);
        bTriesToConnectToRoom = false;
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
        base.OnPlayerEnteredRoom(newPlayer);
        
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
        base.OnPlayerLeftRoom(otherPlayer);
    }

    //Don't need to extend OnCreateRoom since it will automatically call
    // OnJoinRoom for us which is all we really need


}
