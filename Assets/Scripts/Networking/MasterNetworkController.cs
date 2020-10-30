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
    public const byte evtCCharactersSelected = TOCLIENTEVENTBASE + 3;
    public const byte evtCMoveToNewTurnPhase = TOCLIENTEVENTBASE + 4;

    public const byte TOMASTEREVENTBASE = 100;

    public const byte evtMSubmitCharacters = TOMASTEREVENTBASE + 2;
    public const byte evtMFinishedTurnPhase = TOMASTEREVENTBASE + 3;

    public Text txtMasterDisplay;
    public bool bIsMaster;

    int nTime;

    //Once we're passed this by the active player picking their ability, save the selection so it
    // can be disseminated to all players once the Execute ability phase starts
    int nSavedSerializedTargetSelection;

    //We'll keep these as ints since we can't transmit custom types with photon events
    public int[][] arnCharacterSelectsReceived = new int[Player.MAXPLAYERS][];

    public int[] arnPlayerExpectedPhase = new int[Player.MAXPLAYERS];

    public MasterManaDistributer manadistributer;


    // Start is called before the first frame update
    public void OnEnable() {

        //TODO:: Make sure this works when the current master disconnects so the other player's
        //       MasterNetworkController will become enabled
        if(PhotonNetwork.IsMasterClient == false) {
            bIsMaster = false;
            return;
        } else {
            bIsMaster = true;
        }

        manadistributer = GetComponent<MasterManaDistributer>();

        PhotonNetwork.AddCallbackTarget(this);

        txtMasterDisplay.text = "bIsMaster: " + bIsMaster;

        nTime = 0;

        //Initialize the starting phase of the game
        for(int i = 0; i < arnPlayerExpectedPhase.Length; i++) {
            arnPlayerExpectedPhase[i] = 0;
        }

    }

    public void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(ExitGames.Client.Photon.EventData photonEvent) {
        if(bIsMaster == false) return;

        byte eventCode = photonEvent.Code;
        if(eventCode >= 200) return; //Don't respond to built-in events

        object[] arContent = (object[])photonEvent.CustomData;

        //The master controller should only react to player-submitted input events (addressed to evtM...)
        switch(eventCode) {

        case MasterNetworkController.evtMSubmitCharacters:
            Debug.Log("Recieved submitted characters");

            int nPlayer = (int)arContent[0];

            //Save the results in the appropriate selection
            arnCharacterSelectsReceived[nPlayer] = new int[Player.MAXCHRS];
            ((int[])arContent[1]).CopyTo(arnCharacterSelectsReceived[nPlayer], 0);

            Debug.LogError("Master recieved selections for player " + nPlayer + " of " + arnCharacterSelectsReceived[nPlayer][0] + ", " + arnCharacterSelectsReceived[nPlayer][1] + ", " + arnCharacterSelectsReceived[nPlayer][2]);

            //Now check if we've received selections for all players
            if(HasReceivedAllCharacterSelections()) {

                Debug.Log("Sending out player selections since all player selections have been received");
                OnReceivedAllCharacterSelections();

                Debug.Log("NOTE - once everything's correct with character selection, can move ahead with turn processing by adding more code here");
            }

            break;

        case MasterNetworkController.evtMFinishedTurnPhase:
            int nPlayerID = (int)arContent[0];
            int nTurnState = (int)arContent[1];
            object[] arAdditionalInfo = (object[])arContent[2];
            Debug.Log("Recieved signal that player " + nPlayerID + " has finished phase " + ((ContTurns.STATETURN)nTurnState).ToString());

            OnPlayerFinishedPhase(nPlayerID, nTurnState, arAdditionalInfo);

            break;

        default:
            //Debug.Log(name + " shouldn't handle event code " + eventCode);
            break;
        }

    }


    public bool HasReceivedAllCharacterSelections() {

        //Check through all received selections and ensure none are missing
        for(int i = 0; i < Player.MAXPLAYERS; i++) {
            if(arnCharacterSelectsReceived[i] == null) {

                Debug.Log("arnCharacterSelectsReceived[" + i + "] is still null, so we haven't receieved everything yet");
                return false;

            }
        }

        return true;
    }

    public void OnReceivedAllCharacterSelections() {

        NetworkConnectionManager.SendEventToClients(evtCCharactersSelected, arnCharacterSelectsReceived);

    }

    public void MoveToPhase(int nNewTurnState) {

        //First, check if everyone has actually reached this expected phase - if not, we don't need to officially do anything yet
        for(int i = 0; i < Player.MAXPLAYERS; i++) {
            if(arnPlayerExpectedPhase[i] != nNewTurnState) {
                //At least one player hasn't reached the expected turn yet
                return;
            }
        }


        object[] arAdditionalInfo = null;

        switch(nNewTurnState) {
        case (int)ContTurns.STATETURN.GIVEMANA:

            //Ask our MasterManaDistributer component to determine what mana should be given to each player
            arAdditionalInfo = new object[2] { nNewTurnState, manadistributer.TakeNextMana() };

            break;

        case (int)ContTurns.STATETURN.EXECUTEACTIONS:

            //Fetch the saved ability selection info and pass it along so each client knows what ability is being used
            arAdditionalInfo = new object[2] { nNewTurnState, nSavedSerializedTargetSelection };
            break;

        default:

            arAdditionalInfo = new object[1] { nNewTurnState };
            break;
        }

        //Let each player know that they can actually progress to that phase
        NetworkConnectionManager.SendEventToClients(evtCMoveToNewTurnPhase, arAdditionalInfo);


    }

    public void MoveToNextPhase(int nCurTurnPhase) {
        MoveToPhase((nCurTurnPhase + 1) % ((int)ContTurns.STATETURN.ENDFLAG));
    }

    public void OnPlayerFinishedPhase(int nPlayerID, int nCurTurnPhase, object[] arAdditionalInfo) {

        //Double check that the phase they claim to have ended is the one we expect them to be on
        Debug.Assert(arnPlayerExpectedPhase[nPlayerID] == nCurTurnPhase);

        //Check if we're in any weird phases that need us to do something special
        if(nCurTurnPhase == (int)ContTurns.STATETURN.CHOOSEACTIONS) {

            //If we're receiving this signal from the non-active player, they can just advance and wait
            if(nPlayerID != ContTurns.Get().GetNextActingChr().plyrOwner.id) {
                //Just have this player advance to their next phase
                MoveToNextPhase(nCurTurnPhase);
            } else {

                //Otherwise, we are the active player so we should examine the Additional Info to see if the passed ability is valid

                if(arAdditionalInfo == null) {

                    //If no additional info was passed, then interpret this as a rest action
                    ResetSavedAbilitySelection();

                } else {
                    if(
                    CanUseAbility((int)arAdditionalInfo[0], (int)arAdditionalInfo[1])) {

                        //If the ability selection and targetting passed can be used, then save that selection and try to move to the next phase

                        SaveAbilitySelection((int)arAdditionalInfo[0], (int)arAdditionalInfo[1]);

                    } else {
                        //Otherwise, if we were passed an invalid ability selection 

                        Debug.LogError("Invalid ability selection of " + (int)arAdditionalInfo[0] + " " + (int)arAdditionalInfo[1] + " " + (int[])arAdditionalInfo[2]);

                        //Then we'll override that ability selection and just assign them a rest (they can locally fail to select a correct ability as many times as they 
                        // want, but they should only ever submit to the master if they're sure they have a finallized good selection)

                        SaveAbilitySelection(ContTurns.Get().GetNextActingChr().globalid, Chr.idResting, null);

                    }
                }

                MoveToNextPhase(nCurTurnPhase);
            }

        } else if(nCurTurnPhase == (int)ContTurns.STATETURN.EXECUTEACTIONS) {
            //Then we need to decide if we should move back to another ability selection phase, or if it's time to end the turn

            //TODONOW
        } else {
            //If it's not a special case, then we can just move to the next phase;

            MoveToNextPhase(nCurTurnPhase);
        }

    }

    //Should only be decided by the master client
    public void SelectNextReadyCharacter() {

        int nPlayer = Random.Range(1, 3);
        int nCharacter = Random.Range(1, 4);

        NetworkConnectionManager.SendEventToClients(evtCNewReadiedCharacter, new object[2] { nPlayer, nCharacter });
    }

    public void Update() {
        if(bIsMaster == false) return;

        int nNewTime = Mathf.FloorToInt(Time.time);
        if(nNewTime > nTime) {
            nTime = nNewTime;
            NetworkConnectionManager.SendEventToClients(evtCTimerTick, new object[1] { nTime });
        }
    }

    public void SaveAbilitySelection(int _nSavedSerializedTargetSelection) {
        nSavedSerializedTargetSelection = _nSavedSerializedTargetSelection;
    }

    public void ResetSavedAbilitySelection() {
        nSavedSerializedTargetSelection = SelectionSerializer.SerializeRest();
    }

    public bool CanUseAbility(int nSerializedSelectionInfo) {
        //Just piggyback off of the local player to determine if we can use the ability

        SelectionSerializer.SelectionInfo infoDeserialized = SelectionSerializer.Deserialize(ContTurns.Get().chrNextReady, nSerializedSelectionInfo);


        //Ensure the serialized selection is indeed for this 
        Debug.Assert(infoDeserialized.chrOwner == ContTurns.Get().GetNextActingChr());

        return (infoDeserialized.actUsed.CanPayMana() && infoDeserialized.CanActivate());

    }


}
