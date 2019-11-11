using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

//
public class ClientNetworkController : MonoBehaviourPun, IOnEventCallback {
    
    public int nLocalPlayerID;
    public int nEnemyPlayerID;

    public Text txtNetworkDebug;

    //public PlayerController playerMe;
    //public PlayerController playerEnemy;

    private static ClientNetworkController inst;
    

    public void SetPlayerIDs() {
        nLocalPlayerID = PhotonNetwork.LocalPlayer.ActorNumber;

        SetDebugText(NetworkConnectionManager.Get().matchType.ToString());

        if(PhotonNetwork.CurrentRoom.MaxPlayers < 2) {
            //If there's only 1 player in the room, we'll have to generate the enemy's id manually
            Debug.Log("Setting IDs manually");
            nEnemyPlayerID = 2;

        } else {
            //If we have another player in the room, then we'll select their actor number
            // NOTE - this assumes that there are only two players in the room
            nEnemyPlayerID = PhotonNetwork.PlayerListOthers[0].ActorNumber;
        }
    }

    public void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);

        if (photonView.Owner.IsLocal == false) {
            gameObject.SetActive(false);
            return;
        }
        inst = this;

        SetPlayerIDs();
        
        //playerMe = RoomManager.Get().playerMe;
        //playerEnemy = RoomManager.Get().playerEnemy;

        Debug.Log("local PlayerId is " + nLocalPlayerID);
        //playerMe.SetPlayerID(nLocalPlayerID);
        //playerEnemy.SetPlayerID(nEnemyPlayerID);

        SendCharacterSelections();
    }

    public void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public static ClientNetworkController Get() {
        return inst;
    }

    public void SendCharacterSelections() {

        Debug.Log("Sending Character Selections");

        //If we're the first player to connect
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1) {
            //Then we can save our picks in the chr1 slot
            CharacterSelection.Get().SubmitSelection(0);

            //Note: At some point this maybe should be changed to fetch the match type stored in the room properties
            NetworkConnectionManager.MATCHTYPE type = NetworkConnectionManager.Get().matchType;

            //If we're playing alone, then we can also submit chr2's character selections
            if (type == NetworkConnectionManager.MATCHTYPE.AI || type == NetworkConnectionManager.MATCHTYPE.SOLO) {

                Debug.Log("Since we're alone, we'll also submit our selections for player 2");
                CharacterSelection.Get().SubmitSelection(1);
            }
        } else {
            Debug.Log("We're not the player id 1, so we should submit our selection under slot 2");
            //Otherwise, if we are the second player to connect, then we have to move our character selection data into 
            // the character 2 array, then submit under the chr2 slot
            CharacterSelection.Get().SwapPlayerSelections();
            CharacterSelection.Get().SubmitSelection(1);
        }
    }

    public void OnButtonClick(int nPlayerID, int _nAbility) {
        Debug.Log("Locally registered a button click");
        
        int nCharacter = 1 + (_nAbility / 2);
        int nAbility = 1 + (_nAbility % 2);
        object[] arnContent = new object[3] { nPlayerID, nCharacter, nAbility };

        NetworkConnectionManager.SendEventToMaster(MasterNetworkController.evtMSubmitAbility, arnContent);

    }

    public void OnEvent(ExitGames.Client.Photon.EventData photonEvent) {

        byte eventCode = photonEvent.Code;
        if (eventCode >= 200) return; //Don't respond to built-in events

        object[] arContent = (object[])photonEvent.CustomData;

        //Players should only react to authoritative commands from the master 
        switch (eventCode) {
            case MasterNetworkController.evtCNewReadiedCharacter:
                Debug.Log("Received ReadiedCharacter event with " + arContent[0] + " and " + arContent[1]);
                break;

            case MasterNetworkController.evtCTimerTick:
                //Debug.Log("Recieved timer tick with " + arContent[0]);
                int nTime = (int)arContent[0];
                HandleTimerTick(nTime);
                break;

            case MasterNetworkController.evtCAbilityUsed:
                Debug.Log("Recieved ability clicked in PlayerNetwork");
                HandleAbilityUsed((int)arContent[0], (int)arContent[1], (int)arContent[2]);
                break;

            case MasterNetworkController.evtCCharactersSelected:
                CharacterSelection.Get().SaveSelections((int[][])arContent);

                break;

            default:
                //Debug.Log(name + " shouldn't handle event code " + eventCode);
                break;
        }

    }
    public void HandleTimerTick(int nTime) {
        //Debug.Log("Timer: " + nTime);
       // SetDebugText("Timer: " + nTime);
    }

    public void HandleAbilityUsed(int nPlayerID, int nCharacter, int nAbility) {
        Debug.Log("HandleAbility");
        SetDebugText("Button for Player " + nPlayerID + ", character:" + nCharacter + ", Ability: " + nAbility);

        //For this test, just choose the opposite player
        //PlayerController playerAffected = playerMe;
        //if (playerAffected.nPlayerID == nPlayerID) {
        //    playerAffected = playerEnemy;
        //}

        //playerAffected.ChangeHealth(nCharacter);
    }

    public bool CanUseAbility(int nPlayerID, int nCharacter, int nAbility) {
        return true;
    }


    // Update is called once per frame
    void Update() {

        DebugPrintCharacterSelections();

    }

    public void DebugPrintCharacterSelections() {

        string sSelections1 = "Player 1: Unreceived";
        if(CharacterSelection.Get().arChrSelections[0] != null) {
            sSelections1 = "Player 1: " + CharacterSelection.Get().arChrSelections[0][0] + ", " + CharacterSelection.Get().arChrSelections[0][1] + ", " + CharacterSelection.Get().arChrSelections[0][2];
        }

        string sSelections2 = "Player 2: Unreceived";
        if (CharacterSelection.Get().arChrSelections[1] != null) {
            sSelections2 = "Player 2: " + CharacterSelection.Get().arChrSelections[1][0] + ", " + CharacterSelection.Get().arChrSelections[1][1] + ", " + CharacterSelection.Get().arChrSelections[1][2];
        }

        SetDebugText(sSelections1 + "\n" + sSelections2);
    }

    public void SetDebugText(string sMessage) {
        txtNetworkDebug.text = sMessage;
    }


}
