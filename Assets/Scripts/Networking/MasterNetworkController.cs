using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

//Doesn't need to actually be networked - just assume this is only instantiated on 
// the master client - it can be in charge of dictating the flow of the game by receiving
// events from clients, processing them (mostly verifying that they're legal), then broadcasting the results to players

public class MasterNetworkController : MonoBehaviour, IOnEventCallback {

    public const byte TOCLIENTEVENTBASE = 0;
    public const byte evtCNewReadiedCharacter = TOCLIENTEVENTBASE + 0;
    public const byte evtCAbilityUsed = TOCLIENTEVENTBASE + 1;
    public const byte evtCTimerTick = TOCLIENTEVENTBASE + 2;

    public const byte TOMASTEREVENTBASE = 100;
    public const byte evtMSubmitAbility = TOMASTEREVENTBASE + 1;

    public Text txtMasterDisplay;
    public bool bIsMaster;

    int nTime;

    // Start is called before the first frame update
    public void OnEnable() {

        //TODO:: Make sure this works when the current master disconnects so the other player's
        //       MasterNetworkController will become enabled
        if (PhotonNetwork.IsMasterClient == false) {
            bIsMaster = false;
            return;
        } else {
            bIsMaster = true;
        }

        PhotonNetwork.AddCallbackTarget(this);

        txtMasterDisplay.text = "bIsMaster: " + bIsMaster;

        nTime = 0;

    }

    public void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(ExitGames.Client.Photon.EventData photonEvent) {
        if (bIsMaster == false) return;

        byte eventCode = photonEvent.Code;
        if (eventCode >= 200) return; //Don't respond to built-in events

        object[] arnContent = (object[])photonEvent.CustomData;

        //The master controller should only react to player-submitted input events
        switch (eventCode) {

            case MasterNetworkController.evtMSubmitAbility:
                Debug.Log("Recieved ability clicked in MasterGameflow");
                if (CanUseAbility((int)arnContent[0], (int)arnContent[1], (int)arnContent[2])) {
                    Debug.Log("Can use this ability");

                    RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                    // We set this to All so that every player can react to this ability in the same way

                    ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };

                    PhotonNetwork.RaiseEvent(MasterNetworkController.evtCAbilityUsed, arnContent, raiseEventOptions, sendOptions);
                } else {
                    Debug.LogError("Cannot use this ability!  It shouldn't have been legal to send this reqest");
                }
                break;

            default:
                //Debug.Log(name + " shouldn't handle event code " + eventCode);
                break;
        }

    }


    public void SendTimerTick() {
        byte evCode = evtCTimerTick;
        object[] arnContent = new object[1] { nTime };

        //Debug.Log("Sending Timer: " + nTime);

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        // We set this to All so that everyone (including the local client) recieves this message

        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, arnContent, raiseEventOptions, sendOptions);
    }

    //Should only be decided by the master client
    public void SelectNextReadyCharacter() {

        byte evCode = evtCNewReadiedCharacter;
        int nPlayer = Random.Range(1, 3);
        int nCharacter = Random.Range(1, 4);
        object[] arnContent = new object[2] { nPlayer, nCharacter };

        Debug.Log("Selected player " + nPlayer + " and character " + nCharacter);

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        // We set this to All so that everyone (including the local client) recieves this message

        ExitGames.Client.Photon.SendOptions sendOptions = new ExitGames.Client.Photon.SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(evCode, arnContent, raiseEventOptions, sendOptions);
    }

    public void Update() {
        if (bIsMaster == false) return;

        int nNewTime = Mathf.FloorToInt(Time.time);
        if (nNewTime > nTime) {
            nTime = nNewTime;
            SendTimerTick();
        }
    }

    public bool CanUseAbility(int nPlayerID, int nCharacter, int nAbility) {
        //Just piggyback off of the local player to determine if we can use the ability
        Debug.LogError("Need to implement a check to ensure the ability can be used");
        return true;
    }

    
}
