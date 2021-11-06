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


    public bool IsPlayerLocallyControlled(int iPlyrId) {
        return nLocalClientID == CharacterSelection.Get().arnPlayerOwners[iPlyrId];
    }

    public bool IsPlayerLocallyControlled(Player plyr) {
        return IsPlayerLocallyControlled(plyr.id);
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


    //By default, no extra information is needed, can pass extra info via the overloaded method if needed
    public void SendTurnPhaseFinished() {
        SendTurnPhaseFinished(new int[0]);
    }


    //Can optionally pass in any extra serialized fields to communicate player choices to the master
    public void SendTurnPhaseFinished(int[] arnSerializedInfo) {

        NetworkConnectionManager.SendEventToMaster(MasterNetworkController.evtMFinishedTurnPhase, new object[2] {
            (int)ContTurns.Get().curStateTurn,
            arnSerializedInfo
        });

    }

    public void OnEvent(ExitGames.Client.Photon.EventData photonEvent) {

        byte eventCode = photonEvent.Code;
        if(eventCode >= 200) return; //Don't respond to built-in events

        //Debug.Log("Client Event Received: " + eventCode);

        object[] arContent = (object[])photonEvent.CustomData;


        //Players should only react to authoritative commands from the master 
        switch(eventCode) {

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
            CharacterSelection.Get().SaveChrSelections((int[][])arContent);
            break;

        case MasterNetworkController.evtCLoadoutsSelected:
            CharacterSelection.Get().SaveLoadoutSelections((int[][][])arContent);
            break;

        case MasterNetworkController.evtCMoveToNewTurnPhase:

            ContTurns.STATETURN newTurnState = (ContTurns.STATETURN)arContent[0];

            Debug.Log("Master told us to move to " + newTurnState.ToString());

            //First, check if we're in the draft phase (no ContTurns interaction needed)
            if(newTurnState == ContTurns.STATETURN.BAN || newTurnState == ContTurns.STATETURN.DRAFT || newTurnState == ContTurns.STATETURN.LOADOUTSETUP) {
                //If we're in the draft, just let the controller know that we're good to move on to the next phase of the draft
                DraftController.Get().FinishDraftPhaseStep();
            } else {
                //Pass along whatever phase of the turn we're now in to the ContTurns
                ContTurns.Get().SetTurnState((ContTurns.STATETURN)arContent[0], arContent[1]);
            }

            break;

        case MasterNetworkController.evtCBanCharacter:
            //The master passed along which character should be banned
            DraftController.Get().BanChr((CharType.CHARTYPE)arContent[0]);
            break;

        case MasterNetworkController.evtCDraftCharacter:
            //The master passed along which character should be drafted next (and for which player)
            DraftController.Get().DraftChr((int)arContent[0], (CharType.CHARTYPE)arContent[1]);
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
