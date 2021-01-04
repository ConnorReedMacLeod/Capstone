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
    public const byte evtCOwnershipSelected = TOCLIENTEVENTBASE + 3;
    public const byte evtCInputTypesSelected = TOCLIENTEVENTBASE + 4;
    public const byte evtCCharactersSelected = TOCLIENTEVENTBASE + 5;
    public const byte evtCMoveToNewTurnPhase = TOCLIENTEVENTBASE + 6;

    public const byte TOMASTEREVENTBASE = 100;

    public const byte evtMSubmitCharacters = TOMASTEREVENTBASE + 2;
    public const byte evtMFinishedTurnPhase = TOMASTEREVENTBASE + 3;

    public Text txtMasterDisplay;

    public bool bIsMaster {
        get { return PhotonNetwork.IsMasterClient; }
    }

    int nTime;

    //Once we're passed this by the active player picking their ability, save the selection so it
    // can be disseminated to all players once the Execute ability phase starts
    int nSavedSerializedTargetSelection;

    //We'll keep these as ints since we can't transmit custom types with photon events
    public int[][] arnCharacterSelectsReceived = new int[Player.MAXPLAYERS][];

    //Whenever we recieve character selections, record which client sent it,
    //  and claim ownership of that player for that client
    public int[] arnPlayerOwners = new int[Player.MAXPLAYERS];
    public int[] arnPlayerInputTypes = new int[Player.MAXPLAYERS];

    public int[] arnPlayerExpectedPhase = new int[Player.MAXPLAYERS];

    public MasterManaDistributer manadistributer;
    public MasterTimeoutController timeoutcontroller;


    public void OnEnable() {

        manadistributer = GetComponent<MasterManaDistributer>();
        timeoutcontroller = GetComponent<MasterTimeoutController>();

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
        if(bIsMaster == false) return; //Only respond to Master events if we're the master

        byte eventCode = photonEvent.Code;
        if(eventCode >= 200) return; //Don't respond to built-in events

        object[] arContent = (object[])photonEvent.CustomData;


        Debug.Log("Master Event Received: " + eventCode);

        //The master controller should only react to player-submitted input events (addressed to evtM...)
        switch(eventCode) {

        case MasterNetworkController.evtMSubmitCharacters:
            //Submitted as 0:PlayerID, 1:arCharacterSelections, 2:InputType
            Debug.Log("Recieved submitted characters");

            int nClientID = photonEvent.Sender;

            Debug.Log("Submission was sent by client " + nClientID);

            int nPlayer = (int)arContent[0];

            //Save a record of which client sent the selections for this character
            // - they will claim ownership of that player (can be overwritten by later selection submittals)
            arnPlayerOwners[nPlayer] = nClientID;

            //Grab and save what type of input type (human/computer) we're expecting for this player
            int nInputType = (int)arContent[2];

            arnPlayerInputTypes[nPlayer] = nInputType;

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
            int nSerializedInfo = (int)arContent[2];
            Debug.Log("Recieved signal that player " + nPlayerID + " has finished phase " + ((ContTurns.STATETURN)nTurnState).ToString());

            OnPlayerFinishedPhase(nPlayerID, nTurnState, nSerializedInfo);

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

        Debug.Log("Master received all character selections");

        //Let everyone know which characters should be populated for each team
        NetworkConnectionManager.SendEventToClients(evtCCharactersSelected, (object[])arnCharacterSelectsReceived);

        Debug.Log("Master sent out evtCCharactersSelected");

        //Let all clients know who is controlling which player
        NetworkConnectionManager.SendEventToClients(evtCOwnershipSelected, LibConversions.ArIntToArObj(arnPlayerOwners));

        Debug.Log("Master sent out evtCOwnershipSelected");

        //Let all clients know what type of input each player is using (human/ai)
        NetworkConnectionManager.SendEventToClients(evtCInputTypesSelected, LibConversions.ArIntToArObj(arnPlayerInputTypes));

        Debug.Log("Master sent out evtCInputTypesSelected");

    }

    public void MoveToPhase(int nPlayerId, int nNewTurnState) {

        //First set that player's expected turn phase to the passed state
        arnPlayerExpectedPhase[nPlayerId] = nNewTurnState;

        //Then, check if everyone has actually reached this expected phase - if not, we don't need to officially do anything yet
        for(int i = 0; i < Player.MAXPLAYERS; i++) {
            if(arnPlayerExpectedPhase[i] != nNewTurnState) {
                //At least one player hasn't reached the expected turn yet

                //Start the timeout timer to ensure the finished players aren't waiting forever on a stalled toaster
                timeoutcontroller.StartTimeoutTimer((ContTurns.STATETURN)arnPlayerExpectedPhase[i]);
                return;
            }
        }

        //At this point, everyone agrees on what turnstate we should be moving to, so prep it
        //  and let everyone know they can progress to it

        object[] arAdditionalInfo = null;

        switch(nNewTurnState) {
        case (int)ContTurns.STATETURN.GIVEMANA:

            //Ask our MasterManaDistributer component to determine what mana should be given to each player
            arAdditionalInfo = new object[2] { nNewTurnState, manadistributer.TakeNextMana() };
            break;

        case (int)ContTurns.STATETURN.CHOOSEACTIONS:

            //Reset any old stored abilityselection
            ResetSavedAbilitySelection();
            break;

        case (int)ContTurns.STATETURN.EXECUTEACTIONS:

            //Fetch the saved ability selection info and pass it along so each client knows what ability is being used
            arAdditionalInfo = new object[2] { nNewTurnState, nSavedSerializedTargetSelection };
            break;

        case (int)ContTurns.STATETURN.TURNEND:

            //Reset any old stored ability selection
            ResetSavedAbilitySelection();
            break;

        default:

            arAdditionalInfo = new object[1] { nNewTurnState };
            break;
        }

        //Let each player know that they can actually progress to that phase
        NetworkConnectionManager.SendEventToClients(evtCMoveToNewTurnPhase, arAdditionalInfo);

        //Stop the timeout timer
        timeoutcontroller.EndTimeoutTimer();
    }

    public void MoveToNextPhase(int nPlayerID, int nCurTurnPhase) {

        int nNextTurnPhase;

        switch(nCurTurnPhase) {

        case (int)ContTurns.STATETURN.CHOOSEACTIONS:
            //If no character is set to act, then we jump directly ahead to TurnEnd
            if(ContTurns.Get().GetNextActingChr() == null) {
                nNextTurnPhase = (int)ContTurns.STATETURN.TURNEND;
            } else {
                //Otherwise, execute whatever action the active player has chosen
                nNextTurnPhase = (int)ContTurns.STATETURN.EXECUTEACTIONS;
            }
            break;

        case (int)ContTurns.STATETURN.EXECUTEACTIONS:
            //If we've finished executing an action, then we can move back to
            //  selecting an action for the character that is next set to act this turn
            nNextTurnPhase = (int)ContTurns.STATETURN.CHOOSEACTIONS;

            break;

        default:
            //By default, just advance to the next sequential phase (wrapping around if reaching the end)
            nNextTurnPhase = (nCurTurnPhase + 1) % ((int)ContTurns.STATETURN.ENDFLAG);

            break;
        }


        MoveToPhase(nPlayerID, nNextTurnPhase);
    }

    public void OnPlayerFinishedPhase(int nPlayerID, int nCurTurnPhase, int nSerializedInfo = 0) {

        //Double check that the phase they claim to have ended is the one we expect them to be on
        Debug.Assert(arnPlayerExpectedPhase[nPlayerID] == nCurTurnPhase);

        //Check if we're in any weird phases that need us to do something special
        if(nCurTurnPhase == (int)ContTurns.STATETURN.CHOOSEACTIONS) {

            //We only need to do something special if we received this end-phase signal from a character
            //  who has actually submitted an ability selection for his active character
            if(ContTurns.Get().GetNextActingChr() != null && ContTurns.Get().GetNextActingChr().plyrOwner.id == nPlayerID) {

                //We need to move ahead with finalizing an ability selection.  Check if their selection was valid, and store it.
                // If it's not valid, then reset it to a rest which will always be fine
                if(nSerializedInfo == 0) {

                    //If no additional info was passed, then interpret this as a rest action
                    ResetSavedAbilitySelection();

                } else if(SelectionSerializer.Deserialize(ContTurns.Get().GetNextActingChr(), nSerializedInfo).CanActivate()) {

                    //If the ability selection and targetting passed can be used, then save that selection and try to move to the next phase

                    SaveAbilitySelection(nSerializedInfo);

                } else {
                    //Otherwise, if we were passed an invalid ability selection 

                    Debug.LogError("MASTER: Invalid ability selection of " + nSerializedInfo);

                    //Then we'll override that ability selection and just assign them a rest (they can locally fail to select a correct ability as many times as they 
                    // want, but they should only ever submit to the master if they're sure they have a finallized good selection)

                    ResetSavedAbilitySelection();

                    //TODO - put a signal in here to let the current player know that they somehow submitted an invalid ability

                }

            }
        }

        //If we're done any special actions for this phase, then we can just progress this player to the next phase
        MoveToNextPhase(nPlayerID, nCurTurnPhase);

    }

    public void ForceAllPlayersEndPhase(ContTurns.STATETURN stateTurn) {

        for(int i = 0; i < Player.MAXPLAYERS; i++) {
            if(arnPlayerExpectedPhase[i] == (int)stateTurn) {
                //Then this is one of the players we have to manually nudge to end their phase

                OnPlayerFinishedPhase(i, (int)stateTurn);
            }

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
