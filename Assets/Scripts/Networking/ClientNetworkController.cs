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
        return nLocalClientID == MatchSetup.Get().curMatchParams.arnPlayersOwners[iPlyrId];
    }

    public bool IsPlayerLocallyControlled(Player plyr) {
        return IsPlayerLocallyControlled(plyr.id);
    }

    public void SetLocalClientID() {
        nLocalClientID = PhotonNetwork.LocalPlayer.ActorNumber;
        Debug.LogError("Local ID is = " + nLocalClientID);
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


    //For when we've finished a phase in the middle of a match and can get our current turnphase from ContTurns
    public void SendMatchTurnPhaseFinished(int[] arnSerializedInfo) {
        SendTurnPhaseFinished(ContTurns.Get().curStateTurn, arnSerializedInfo);
    }

    //Be default, no extra information is needed, can pass extra info via the overloaded method if needed
    public void SendTurnPhaseFinished(ContTurns.STATETURN stateturnFinished) {
        SendTurnPhaseFinished(stateturnFinished, new int[0]); //just pass an empty array for our extra info param
    }

    //For when some portion of the turn is finished and we want to let the master know that we're done
    public void SendTurnPhaseFinished(ContTurns.STATETURN stateturnFinished, int[] arnSerializedInfo) {
        NetworkConnectionManager.SendEventToMaster(MasterNetworkController.evtMFinishedTurnPhase, new object[2] {
            stateturnFinished,
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

            case MasterNetworkController.evtCStartDraftWithParams:
                Debug.Log("Got the signal from the master to start a new draft");
                MatchSetup.Get().SaveMatchParams(MatchSetup.UnserializeMatchParams(arContent));
                break;

            case MasterNetworkController.evtCStartLoadoutWithParams:
                Debug.Log("Got the signal from the master to start a new match");
                MatchSetup.Get().SaveMatchParams(MatchSetup.UnserializeMatchParams(arContent));
                break;

            case MasterNetworkController.evtCStartMatchWithParams:
                Debug.Log("Got the signal from the master to start a new match");
                MatchSetup.Get().SaveMatchParams(MatchSetup.UnserializeMatchParams(arContent));
                break;

            case MasterNetworkController.evtCMoveToNewTurnPhase:

                ContTurns.STATETURN newTurnState = (ContTurns.STATETURN)arContent[0];

                Debug.Log("Master told us to move to " + newTurnState.ToString());

                    MoveToTurnPhase(newTurnState, arContent[1]);

                break;

            default:
                //Debug.Log(name + " shouldn't handle event code " + eventCode);
                break;
        }

    }

    public void MoveToTurnPhase(ContTurns.STATETURN newTurnState, object additionalInfo) {

        switch (newTurnState) {
            
            default:
                //By default, we are in the middle of a match
                //Pass along whatever phase of the turn we're now in to the ContTurns
                ContTurns.Get().SetTurnState(newTurnState, additionalInfo);

                break;
        }

    }

    public void HandleTimerTick(int nTime) {
        SetDebugText("Timer: " + nTime);
    }

    // Update is called once per frame
    void Update() {


    }

    public void SetDebugText(string sMessage) {
        txtNetworkDebug.text = sMessage;
    }


}
