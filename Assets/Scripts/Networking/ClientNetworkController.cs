using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class ClientNetworkController : MonoBehaviourPun, IOnEventCallback {

    public int nLocalClientID;

    public Text txtNetworkDebug;

    private static ClientNetworkController inst;


    public bool IsPlayerLocallyControlled(Player plyr) {
        return nLocalClientID == CharacterSelection.Get().arnPlayerOwners[plyr.id];
    }

    public void SetLocalClientID() {
        nLocalClientID = PhotonNetwork.LocalPlayer.ActorNumber;
    }

    public void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);

        if(photonView.Owner.IsLocal == false) {
            gameObject.SetActive(false);
            return;
        }
        inst = this;

        SetLocalClientID();

        Debug.Log("local PlayerId is " + nLocalClientID);
    }

    public void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public static ClientNetworkController Get() {
        return inst;
    }



    //Can optionally pass in any extra serialized fields to communicate player choices to the master
    public void SendTurnPhaseFinished(int nSerializedInfo = 0) {

        NetworkConnectionManager.SendEventToMaster(MasterNetworkController.evtMFinishedTurnPhase, new object[2] {
            (int)ContTurns.Get().curStateTurn,
            nSerializedInfo
        });

    }

    public void OnButtonClick(int nPlayerID, int _nAbility) {
        Debug.Log("Locally registered a button click");

        int nCharacter = 1 + (_nAbility / 2);
        int nAbility = 1 + (_nAbility % 2);
        object[] arnContent = new object[3] { nPlayerID, nCharacter, nAbility };

        //NetworkConnectionManager.SendEventToMaster(MasterNetworkController.evtMSubmitAbility, arnContent);

    }

    public void OnEvent(ExitGames.Client.Photon.EventData photonEvent) {

        byte eventCode = photonEvent.Code;
        if(eventCode >= 200) return; //Don't respond to built-in events

        Debug.Log("Client Event Received: " + eventCode);

        object[] arContent = (object[])photonEvent.CustomData;


        //Players should only react to authoritative commands from the master 
        switch(eventCode) {
        case MasterNetworkController.evtCNewReadiedCharacter:
            Debug.Log("Received ReadiedCharacter event with " + arContent[0] + " and " + arContent[1]);
            break;

        case MasterNetworkController.evtCTimerTick:
            //Debug.Log("Recieved timer tick with " + arContent[0]);
            int nTime = (int)arContent[0];
            HandleTimerTick(nTime);
            break;

        case MasterNetworkController.evtCOwnershipSelected:
            //Note - we ignore arContent, since we need to cast to an int[],
            //       so we just do the proper cast directly from the stored CustomData
            CharacterSelection.Get().SaveOwnerships(LibConversions.ArObjToArInt((object[])photonEvent.CustomData));
            break;

        case MasterNetworkController.evtCInputTypesSelected:
            //Note - have to directly cast PhotonEvent.CustomData, since we can't convert back from arContent
            CharacterSelection.Get().SaveInputTypes(LibConversions.ArObjToArInputType((object[])photonEvent.CustomData));
            break;

        case MasterNetworkController.evtCCharactersSelected:
            CharacterSelection.Get().SaveSelections((int[][])arContent);

            break;

        case MasterNetworkController.evtCMoveToNewTurnPhase:
            //Pass along whatever phase of the turn we're now in to the ContTurns
            Debug.Log("Master told us to move to " + ((ContTurns.STATETURN)arContent[0]).ToString());
            ContTurns.Get().SetTurnState((ContTurns.STATETURN)arContent[0], arContent[1]);
            break;

        default:
            //Debug.Log(name + " shouldn't handle event code " + eventCode);
            break;
        }

    }
    public void HandleTimerTick(int nTime) {
        SetDebugText("Timer: " + nTime);
    }

    // Update is called once per frame
    void Update() {


    }

    public void DebugPrintCharacterSelections() {

        string sSelections1 = "Player 1: Unreceived";
        if(CharacterSelection.Get().arChrSelections[0] != null) {
            sSelections1 = "Player 1: " + CharacterSelection.Get().arChrSelections[0][0] + ", " + CharacterSelection.Get().arChrSelections[0][1] + ", " + CharacterSelection.Get().arChrSelections[0][2];
        }

        string sSelections2 = "Player 2: Unreceived";
        if(CharacterSelection.Get().arChrSelections[1] != null) {
            sSelections2 = "Player 2: " + CharacterSelection.Get().arChrSelections[1][0] + ", " + CharacterSelection.Get().arChrSelections[1][1] + ", " + CharacterSelection.Get().arChrSelections[1][2];
        }

        //SetDebugText(sSelections1 + "\n" + sSelections2);
    }

    public void SetDebugText(string sMessage) {
        txtNetworkDebug.text = sMessage;
    }


}
