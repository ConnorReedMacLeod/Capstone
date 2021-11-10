using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

//Doesn't need to actually be networked - just assume this is only instantiated on 
// the master client - it can be in charge of dictating the flow of the game by receiving
// events from clients, processing them (mostly verifying that they're legal), then broadcasting the results to players

public class MasterNetworkController : SingletonPersistent<MasterNetworkController>, IOnEventCallback {

    public const byte TOCLIENTEVENTBASE = 0;

    public const byte evtCSkillSelected = TOCLIENTEVENTBASE + 1;
    public const byte evtCTimerTick = TOCLIENTEVENTBASE + 2;
    public const byte evtCStartMatchWithParams = TOCLIENTEVENTBASE + 3;



    public const byte evtCMoveToNewTurnPhase = TOCLIENTEVENTBASE + 7;
    public const byte evtCDraftCharacter = TOCLIENTEVENTBASE + 8;
    public const byte evtCBanCharacter = TOCLIENTEVENTBASE + 9;

    public const byte TOMASTEREVENTBASE = 100;

    public const byte evtMDraftCharacterSelected = TOMASTEREVENTBASE + 0;
    public const byte evtMBanCharacterSelected = TOMASTEREVENTBASE + 1;
    public const byte evtMSubmitMatchParams = TOMASTEREVENTBASE + 2;
    public const byte evtMSubmitMatchParamsAndStartMatch = TOMASTEREVENTBASE + 3;
    public const byte evtMFinishedTurnPhase = TOMASTEREVENTBASE + 4;

    public Text txtMasterDisplay;

    public const int nStartingPhase = 0;

    public static bool bIsMaster {
        get { return PhotonNetwork.IsMasterClient; }
    }

    int nTime;

    //Save a copy of whichever character was drafted/banned so we can pass it along
    // to all players when everyone's ready to progress
    int nSavedCharacterSelection;

    //Once we're passed this by the active player picking their skill, save the selection so it
    // can be disseminated to all players once the Execute skill phase starts
    int[] arnSavedSerializedInfo;

    //Keep a stored compilation of all match-setup parameters that we've been passed.  When ready,
    // we can re-serialize it and distribute it to all clients to start a match
    public MatchSetup.MatchParams matchparamsPrepped;

    //TODO - look at making this into a dynamic list of connected players
    public Dictionary<int, int> dictClientExpectedPhase;

    public MasterManaDistributer manadistributer;
    public MasterTimeoutController timeoutcontroller;


    public override void Init() {

        // Construct a default marchparams that we can mutate when a client sends in changes
        matchparamsPrepped = new MatchSetup.MatchParams();
        Debug.LogError("Master's initial matchparams has owner of " + matchparamsPrepped.arnPlayersOwners[0]);
    }

    public void OnEnable() {

        Debug.Log("When MasterNetworkController is enabled, we have " + PhotonNetwork.CurrentRoom.PlayerCount + " players in the room");

        manadistributer = GetComponent<MasterManaDistributer>();
        timeoutcontroller = GetComponent<MasterTimeoutController>();

        PhotonNetwork.AddCallbackTarget(this);

        txtMasterDisplay.text = "bIsMaster: " + bIsMaster;

        nTime = 0;

        dictClientExpectedPhase = new Dictionary<int, int>();
        //Initialize the starting phase of the game
        foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList) {
            Debug.Log("Adding client with ActorNumber " + player.ActorNumber + " to dictClientExpectedPhase");
            dictClientExpectedPhase[player.ActorNumber] = nStartingPhase;
        }

    }

    public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
        //Add that player to our tracked list of expected phases for each player
        Debug.Log("Adding new player " + newPlayer.ActorNumber + " to our starting expected phase");
        dictClientExpectedPhase[newPlayer.ActorNumber] = nStartingPhase;
    }

    public void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(ExitGames.Client.Photon.EventData photonEvent) {
        if(bIsMaster == false) return; //Only respond to Master events if we're the master

        byte eventCode = photonEvent.Code;
        if(eventCode >= 200) return; //Don't respond to built-in events

        int nClientID = photonEvent.Sender;
        object[] arContent = (object[])photonEvent.CustomData;

        //Debug.Log("Master Event Received: " + eventCode);

        //The master controller should only react to player-submitted input events (addressed to evtM...)
        switch(eventCode) {

        case MasterNetworkController.evtMSubmitMatchParams:
            //Deserialize the passed match params and store it locally, overwriting our current match params
            matchparamsPrepped = MatchSetup.UnserializeMatchParams(arContent);

            Debug.Log("Master received the matchparams: " + matchparamsPrepped);

            break;

        case MasterNetworkController.evtMSubmitMatchParamsAndStartMatch:
            //Deserialize the passed match params and store it locally, overwriting our current match params
            matchparamsPrepped = MatchSetup.UnserializeMatchParams(arContent);

            Debug.Log("Master received the matchparams (and will now start the match): " + matchparamsPrepped);

            BroadcastMatchStart();

            break;

        case MasterNetworkController.evtMFinishedTurnPhase:
            int nTurnState = (int)arContent[0];
            int[] arnSerializedInfo = (int[])arContent[1];
            Debug.Log("Recieved signal that client " + nClientID + " has finished phase " + ((ContTurns.STATETURN)nTurnState).ToString());

            OnClientFinishedPhase(nClientID, nTurnState, arnSerializedInfo);

            break;

        default:
            //Debug.Log(name + " shouldn't handle event code " + eventCode);
            break;
        }

    }


    public void BroadcastMatchStart() {

        Debug.Log("Transferring to the match scene");
        PhotonNetwork.LoadLevel("_MATCH");

        Debug.Log("Sending out matchparams to start a match: " + matchparamsPrepped);

        //Serialize and distribute our matchparams to all players to let them know how to start the match
        NetworkConnectionManager.SendEventToClients(evtCStartMatchWithParams, MatchSetup.SerializeMatchParams(matchparamsPrepped));
        
    }


    //Starts a timeout timer
    public bool CheckIfAllPlayersFinishedPhase(int nNewTurnState) {
        foreach(int i in dictClientExpectedPhase.Keys) {
            if(dictClientExpectedPhase[i] != nNewTurnState) {
                //At least one client hasn't reached the expected turn yet

                //Start the timeout timer to ensure the finished players aren't waiting forever on a stalled toaster
                timeoutcontroller.StartTimeoutTimer((ContTurns.STATETURN)dictClientExpectedPhase[i]);
                return false;
            }
        }

        return true;
    }

    public void MoveToPhase(int nClientID, int nNewTurnState) {

        //First set that client's expected turn phase to the passed state
        dictClientExpectedPhase[nClientID] = nNewTurnState;

        //Then, check if everyone has actually reached this expected phase - if not, we don't need to officially do anything yet
        if(CheckIfAllPlayersFinishedPhase(nNewTurnState) == false) return;

        //At this point, everyone agrees on what turnstate we should be moving to, so prep it
        //  and let everyone know they can progress to it

        object[] arAdditionalInfo = new object[2] { nNewTurnState, null };//Pass null for default extra info

        switch(nNewTurnState) {

        case (int)ContTurns.STATETURN.GIVEMANA:

            //Ask our MasterManaDistributer component to determine what mana should be given to each player
            arAdditionalInfo[1] = manadistributer.TakeNextMana();

            break;

        case (int)ContTurns.STATETURN.CHOOSESKILL:

            //Reset any old stored skillselection
            ResetSavedSkillSelection();

            break;

        case (int)ContTurns.STATETURN.EXECUTESKILL:

            //Fetch the saved skill selection info and pass it along so each client knows what skill is being used
            arAdditionalInfo[1] = arnSavedSerializedInfo;

            break;

        case (int)ContTurns.STATETURN.TURNEND:

            //Reset any old stored skill selection
            ResetSavedSkillSelection();

            break;

        case (int)ContTurns.STATETURN.BAN:

            //Send out a signal for which character just got banned
            NetworkConnectionManager.SendEventToClients(evtCBanCharacter, nSavedCharacterSelection);

            //Clear out the stored character selection
            ResetSavedChrSelection();

            break;

        case (int)ContTurns.STATETURN.DRAFT:

            //Send out a signal for which character just got drafted
            NetworkConnectionManager.SendEventToClients(evtCDraftCharacter, nSavedCharacterSelection);

            //Clear out the stored character selection
            ResetSavedChrSelection();

            break;

        default:



            break;
        }


        //Let each client know that they can actually progress to that phase
        NetworkConnectionManager.SendEventToClients(evtCMoveToNewTurnPhase, arAdditionalInfo);

        //Stop the timeout timer
        timeoutcontroller.EndTimeoutTimer();
    }

    public void MoveToNextPhase(int nPlayerID, int nCurTurnPhase) {

        int nNextTurnPhase;

        switch(nCurTurnPhase) {

        case (int)ContTurns.STATETURN.CHOOSESKILL:
            //If no character is set to act, then we jump directly ahead to TurnEnd
            if(ContTurns.Get().GetNextActingChr() == null) {
                nNextTurnPhase = (int)ContTurns.STATETURN.TURNEND;
            } else {
                //Otherwise, execute whatever skill the active player has chosen
                nNextTurnPhase = (int)ContTurns.STATETURN.EXECUTESKILL;
            }
            break;

        case (int)ContTurns.STATETURN.EXECUTESKILL:
            //If we've finished executing an skill, then we can move back to
            //  selecting a skill for the character that is next set to act this turn
            nNextTurnPhase = (int)ContTurns.STATETURN.CHOOSESKILL;

            break;

        case (int)ContTurns.STATETURN.TURNEND:
            //If we're at the end of turn, reset to the beginning
            nNextTurnPhase = (int)ContTurns.STATETURN.RECHARGE;

            break;

        default:

            //By default, just advance to the next sequential phase
            nNextTurnPhase = nCurTurnPhase + 1;

            break;
        }


        MoveToPhase(nPlayerID, nNextTurnPhase);
    }

    public void OnClientFinishedPhase(int nClientID, int nCurTurnPhase, int nSerializedInfo) {
        OnClientFinishedPhase(nClientID, nCurTurnPhase, new int[1] { nSerializedInfo });
    }

    public void OnClientFinishedPhase(int nClientID, int nCurTurnPhase, int[] arnSerializedInfo = null) {

        //Double check that the phase they claim to have ended is the one we expect them to be on
        Debug.Assert(dictClientExpectedPhase[nClientID] == nCurTurnPhase, "Client " + nClientID + " is expected to be in " +
            (ContTurns.STATETURN)dictClientExpectedPhase[nClientID] + " but received the signal that they finished " + (ContTurns.STATETURN)nCurTurnPhase);

        //Check if we're in any weird phases that need us to do something special

        //If we're expecting an skill selection
        if(nCurTurnPhase == (int)ContTurns.STATETURN.CHOOSESKILL) {

            //We only need to do something special if we received this end-phase signal from a client
            //  who has actually submitted a skill selection for his active character
            if(ContTurns.Get().GetNextActingChr() != null &&
                MatchSetup.Get().curMatchParams.arnPlayersOwners[ContTurns.Get().GetNextActingChr().plyrOwner.id] == nClientID) {

                Selections selectionsFromClient;

                //We need to move ahead with finalizing a skill selection.  If no serialized info for selections
                // was passed along, then just treat the selection as a rest skill
                if(arnSerializedInfo == null) {

                    Debug.Log("MASTER: Received no selection from the client - issuing a Rest skill");

                    selectionsFromClient = Selections.GetRestSelection(ContTurns.Get().GetNextActingChr());

                    //If we did get a serialized selection, then we can deserialize it to see what it's targetting
                } else {

                    //Deserialize by constructing a new Selections from the serialized array
                    selectionsFromClient = new Selections(arnSerializedInfo);

                    //If the selections we got correspond to a set of invalid targets, then reset that selection to a rest skill
                    if(selectionsFromClient.IsValidSelection() == false) {

                        Debug.LogError("MASTER: Received an invalid skill selection of " + selectionsFromClient.ToString());

                        //Reset to a rest action and reserialize the selectionsFromClient back into arnSerializedInfo
                        selectionsFromClient.ResetToRestSelection();
                        arnSerializedInfo = selectionsFromClient.GetSerialization();

                        //TODO - Consider putting a signal in here to let the current player know that they somehow submitted an invalid skill
                        //       Likely this would only occur if there was some sort of programming/sync error though


                    } else {

                        Debug.Log("MASTER: Received valid selection of " + selectionsFromClient.ToString());
                    }
                }

                //By this point, we have either received a valid skill, or we have just wrote-in a rest selection.
                //  Either way, we can save this as a valid selection to be distributed to all players once we are ready
                //  to progress to the next phase of the turn
                SaveSerializedSelection(arnSerializedInfo);


            } else {
                //Don't need to do anything special if we recieved the signal from the non-active player
                // since they're not drafting anything right now
            }

        } else if(nCurTurnPhase == (int)ContTurns.STATETURN.BAN) {
            //

        }

        //If we're done any special actions for this phase, then we can just progress this player to the next phase
        MoveToNextPhase(nClientID, nCurTurnPhase);

    }

    public void ForceAllClientsEndPhase(ContTurns.STATETURN stateTurn) {

        foreach(int i in dictClientExpectedPhase.Keys) {
            if(dictClientExpectedPhase[i] == (int)stateTurn) {
                //Then this is one of the clients we have to manually nudge to end their phase


                if(stateTurn == ContTurns.STATETURN.CHOOSESKILL) {
                    //Pass no selections for the skill we want the character to use (will be reset to a rest)
                    OnClientFinishedPhase(i, (int)stateTurn, null);

                } else if(stateTurn == ContTurns.STATETURN.BAN) {

                    //Simulate as though the player submitted a non-ban
                    OnClientFinishedPhase(i, (int)stateTurn, (int)CharType.CHARTYPE.LENGTH);
                } else if(stateTurn == ContTurns.STATETURN.DRAFT) {

                    //Simulate as though the player submitted a non-draft
                    OnClientFinishedPhase(i, (int)stateTurn, (int)CharType.CHARTYPE.LENGTH);
                } else {

                    OnClientFinishedPhase(i, (int)stateTurn);

                }

            }

        }

    }


    public void Update() {
        //Remaain inactive if we're not the master
        if(bIsMaster == false) return;

        int nNewTime = Mathf.FloorToInt(Time.time);
        if(nNewTime > nTime) {
            nTime = nNewTime;
            NetworkConnectionManager.SendEventToClients(evtCTimerTick, new object[1] { nTime });
            //PrintExpectedPhases();
        }
    }



    public void SaveCharacterSelection(int _iChr) {
        nSavedCharacterSelection = _iChr;
    }

    public void ResetSavedChrSelection() {
        nSavedCharacterSelection = (int)CharType.CHARTYPE.LENGTH;
    }

    public void SaveSerializedSelection(int[] _arnSavedSerializedInfo) {
        arnSavedSerializedInfo = _arnSavedSerializedInfo;
    }
    public void ResetSavedSkillSelection() {
        arnSavedSerializedInfo = null;
    }



    public void PrintExpectedPhases() {
        string sPrint = "[MASTER] Expected Phases:  ";
        foreach(int i in dictClientExpectedPhase.Keys) {
            sPrint += "Client " + i + ": " + ((ContTurns.STATETURN)dictClientExpectedPhase[i]).ToString() + " | ";
        }

        Debug.Log(sPrint);
    }

}
