﻿using System.Collections;
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
    public const byte evtCCharactersSelected = TOCLIENTEVENTBASE + 3;

    public const byte TOMASTEREVENTBASE = 100;
    public const byte evtMSubmitAbility = TOMASTEREVENTBASE + 1;
    public const byte evtMSubmitCharacters = TOMASTEREVENTBASE + 2;

    public Text txtMasterDisplay;
    public bool bIsMaster;

    int nTime;

    //We'll keep this as an int since we can't transmit custom types with photon events
    public int[][] arnCharacterSelectsReceived = new int[Player.MAXPLAYERS][];


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

        object[] arContent = (object[])photonEvent.CustomData;

        //The master controller should only react to player-submitted input events
        switch (eventCode) {

            case MasterNetworkController.evtMSubmitAbility:
                Debug.Log("Recieved ability clicked in MasterGameflow");
                if (CanUseAbility((int)arContent[0], (int)arContent[1], (int)arContent[2])) {
                    Debug.Log("Can use this ability");
                    
                    NetworkConnectionManager.SendEventToClients(evtCAbilityUsed, arContent);
                } else {
                    Debug.LogError("Cannot use this ability!  It shouldn't have been legal to send this reqest");
                }
                break;

            case MasterNetworkController.evtMSubmitCharacters:
                Debug.Log("Recieved submitted characters");

                int nPlayer = (int)arContent[0];

                //Save the results in the appropriate selection
                arnCharacterSelectsReceived[nPlayer] = new int[Player.MAXCHRS];
                ((int[])arContent[1]).CopyTo(arnCharacterSelectsReceived[nPlayer], 0);

                Debug.LogError("Master recieved selections for player " + nPlayer + " of " + arnCharacterSelectsReceived[nPlayer][0] + ", " + arnCharacterSelectsReceived[nPlayer][1] + ", " + arnCharacterSelectsReceived[nPlayer][2]);

                //Now check if we've received selections for all players
                if (HasReceivedAllCharacterSelections()) {

                    Debug.Log("Sending out player selections since all player selections have been received");
                    OnReceivedAllCharacterSelections();
                }

                break;

            default:
                //Debug.Log(name + " shouldn't handle event code " + eventCode);
                break;
        }

    }

    public bool HasReceivedAllCharacterSelections() {
       
        //Check through all received selections and ensure none are missing
        for(int i=0; i<Player.MAXPLAYERS; i++) {
            if (arnCharacterSelectsReceived[i] == null) {

                Debug.Log("arnCharacterSelectsReceived["+i+"] is still null, so we haven't receieved everything yet");
                return false;

            }
        }

        return true;
    }
    
    public void OnReceivedAllCharacterSelections() {

        NetworkConnectionManager.SendEventToClients(evtCCharactersSelected, arnCharacterSelectsReceived);

    }

    //Should only be decided by the master client
    public void SelectNextReadyCharacter() {

        int nPlayer = Random.Range(1, 3);
        int nCharacter = Random.Range(1, 4);

        NetworkConnectionManager.SendEventToClients(evtCNewReadiedCharacter, new object[2] { nPlayer, nCharacter });
    }

    public void Update() {
        if (bIsMaster == false) return;

        int nNewTime = Mathf.FloorToInt(Time.time);
        if (nNewTime > nTime) {
            nTime = nNewTime;
            NetworkConnectionManager.SendEventToClients(evtCTimerTick, new object[1] { nTime });
        }
    }

    public bool CanUseAbility(int nPlayerID, int nCharacter, int nAbility) {
        //Just piggyback off of the local player to determine if we can use the ability
        Debug.LogError("Need to implement a check to ensure the ability can be used");
        return true;
    }

    
}