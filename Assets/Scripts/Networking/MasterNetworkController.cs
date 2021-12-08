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
    public const byte evtCStartDraftWithParams = TOCLIENTEVENTBASE + 3;
    public const byte evtCStartLoadoutWithParams = TOCLIENTEVENTBASE + 4;
    public const byte evtCStartMatchWithParams = TOCLIENTEVENTBASE + 5;

    public const byte evtCMoveToNewTurnPhase = TOCLIENTEVENTBASE + 6;

    public const byte TOMASTEREVENTBASE = 100;

    public const byte evtMStartDraft = TOMASTEREVENTBASE + 0;
    public const byte evtMSubmitMatchParamsAndDirectlyStartLoadout = TOMASTEREVENTBASE + 1;
    public const byte evtMSubmitMatchParamsAndDirectlyStartMatch = TOMASTEREVENTBASE + 2;
    public const byte evtMFinishedTurnPhase = TOMASTEREVENTBASE + 3;

    public Text txtMasterDisplay;

    public const int nStartingPhase = 0;

    public static bool bIsMaster {
        get { return PhotonNetwork.IsMasterClient; }
    }

    int nTime;
    

    //Once we're passed this by the active player picking their skill, save the selection so it
    // can be disseminated to all players once the Execute skill phase starts
    int[] arnSavedSerializedInfo;

    //Keep a stored compilation of all match-setup parameters that we've been passed.  When ready,
    // we can re-serialize it and distribute it to all clients to start a match
    public MatchSetup.MatchParams matchparamsPrepped;

    //TODO - look at making this into a dynamic list of connected players
    public Dictionary<int, int> dictClientExpectedPhase;

    public ContManaDistributer manadistributer;
    public MasterTimeoutController timeoutcontroller;


    public override void Init() {

        // Construct a default matchparams that we can mutate when a client sends in changes
        matchparamsPrepped = new MatchSetup.MatchParams();
        Debug.LogError("Master's initial matchparams has " + matchparamsPrepped);
    }

    public void OnEnable() {

        Debug.Log("When MasterNetworkController is enabled, we have " + PhotonNetwork.CurrentRoom.PlayerCount + " players in the room");

        manadistributer = GetComponent<ContManaDistributer>();
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
            

            case MasterNetworkController.evtMSubmitMatchParamsAndDirectlyStartLoadout:
                Debug.Assert(nClientID == PhotonNetwork.MasterClient.ActorNumber, 
                    "ERROR - Master received directly start loadout signal from a non-master client (" + nClientID + ")");

                //Deserialize the passed match parameters which we'll copy starting and chrselections fields from and fill the loadout-responsible
                //  fields while in the loadout phase
                matchparamsPrepped.CopyForLoadoutStart(MatchSetup.UnserializeMatchParams(arContent));

                Debug.Log("Master received the matchparams (and will now directly move to the loadout phase): " + matchparamsPrepped);

                BroadcastLoadoutStart();

                break;

            case MasterNetworkController.evtMSubmitMatchParamsAndDirectlyStartMatch:
                Debug.Assert(nClientID == PhotonNetwork.MasterClient.ActorNumber, 
                    "ERROR - Master received directly start match signal from a non-master client (" + nClientID + ")");

                //Deserialize the passed match params and store it locally.  We're assuming it is fully filled out
                matchparamsPrepped.CopyForMatchStart(MatchSetup.UnserializeMatchParams(arContent));

                Debug.Log("Master received the matchparams (and will now directly start the match): " + matchparamsPrepped);

                BroadcastMatchStart();

                break;

            case MasterNetworkController.evtMFinishedTurnPhase:
                int nTurnState = (int)arContent[0];
                Debug.Log("Recieved signal that client " + nClientID + " has finished phase " + ((ContTurns.STATETURN)nTurnState).ToString());

                OnClientFinishedPhase(nClientID, nTurnState, arContent[1]);

                break;

            default:
                //Debug.Log(name + " shouldn't handle event code " + eventCode);
                break;
            }

    }

    public void BroadcastDraftStart() {

        Debug.Log("Transferring to the draft scene");
        PhotonNetwork.LoadLevel("_DRAFT");

        Debug.Log("Sending out matchparams to start a draft: " + matchparamsPrepped);

        //Serialize and distribute our matchparams to all players to let them know how to start the draft
        NetworkConnectionManager.SendEventToClients(evtCStartDraftWithParams, MatchSetup.SerializeMatchParams(matchparamsPrepped));
    }

    public void BroadcastLoadoutStart() {

        Debug.Log("Transferring to the loadout scene");
        PhotonNetwork.LoadLevel("_LOADOUT");

        Debug.Log("Sending out matchparams to start the loadout phase: " + matchparamsPrepped);

        //Serialize and distribute our matchparams to all players to let them know how to start the draft
        NetworkConnectionManager.SendEventToClients(evtCStartLoadoutWithParams, MatchSetup.SerializeMatchParams(matchparamsPrepped));
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

    public void MoveToPhase(int nClientID, int nNewTurnState, int nPrevTurnState) {

        //First set that client's expected turn phase to the passed state
        dictClientExpectedPhase[nClientID] = nNewTurnState;

        //Then, check if everyone has actually reached this expected phase - if not, we don't need to officially do anything yet
        if(CheckIfAllPlayersFinishedPhase(nNewTurnState) == false) return;

        //At this point, everyone agrees on what turnstate we should be moving to, so prep it
        //  and let everyone know they can progress to it

        //Stop the timeout timer
        timeoutcontroller.EndTimeoutTimer();

        object[] arAdditionalInfo = new object[2] { nNewTurnState, null };//Pass null for default extra info

        switch((ContTurns.STATETURN)nNewTurnState) {


            case ContTurns.STATETURN.GIVEMANA:

                //Ask our MasterManaDistributer component to determine what mana should be given to each player
                arAdditionalInfo[1] = manadistributer.TakeNextMana();

                break;

            case ContTurns.STATETURN.CHOOSESKILL:

                //Reset any old stored skillselection
                ResetSavedSkillSelection();

                break;

            case ContTurns.STATETURN.TURNEND:

                //Reset any old stored skill selection
                ResetSavedSkillSelection();

                break;


            default:



                break;
        }
        

        //Let each client know that they can actually progress to that phase
        NetworkConnectionManager.SendEventToClients(evtCMoveToNewTurnPhase, arAdditionalInfo);

    }

    

    public void OnClientFinishedPhase(int nClientID, int nCurTurnPhase, int nSerializedInfo) {
        OnClientFinishedPhase(nClientID, nCurTurnPhase, new int[1] { nSerializedInfo });
    }

    //Note, the arSerializedInfo is passed as an object so it can be cast into whatever type we expect to receive based
    //  on the turn phase we're finishing
    public void OnClientFinishedPhase(int nClientID, int nCurTurnPhase, object arSerializedInfo = null) {

        //Double check that the phase they claim to have ended is the one we expect them to be on
        Debug.Assert(dictClientExpectedPhase[nClientID] == nCurTurnPhase, "Client " + nClientID + " is expected to be in " +
            (ContTurns.STATETURN)dictClientExpectedPhase[nClientID] + " but received the signal that they finished " + (ContTurns.STATETURN)nCurTurnPhase);

        //Check if we're in any weird phases that need us to do something special (primarily storing information passed to us from the client

        switch ((ContTurns.STATETURN)nCurTurnPhase) {

            //If we're expecting an skill selection
            case ContTurns.STATETURN.CHOOSESKILL:

                //We only need to do something special if we received this end-phase signal from a client
                //  who has actually submitted a skill selection for his active character
                if (ContTurns.Get().GetNextActingChr() != null &&
                    MatchSetup.Get().curMatchParams.arnPlayersOwners[ContTurns.Get().GetNextActingChr().plyrOwner.id] == nClientID) {

                    Selections selectionsFromClient;

                    //We need to move ahead with finalizing a skill selection.  If no serialized info for selections
                    // was passed along, then just treat the selection as a rest skill
                    if (arSerializedInfo == null) {

                        Debug.Log("MASTER: Received no selection from the client - issuing a Rest skill");

                        selectionsFromClient = Selections.GetRestSelection(ContTurns.Get().GetNextActingChr());

                        //If we did get a serialized selection, then we can deserialize it to see what it's targetting
                    } else {

                        //Deserialize by constructing a new Selections from the serialized array
                        selectionsFromClient = new Selections((int[])arSerializedInfo);

                        //If the selections we got correspond to a set of invalid targets, then reset that selection to a rest skill
                        if (selectionsFromClient.IsValidSelection() == false) {

                            Debug.LogError("MASTER: Received an invalid skill selection of " + selectionsFromClient.ToString());

                            //Reset to a rest action and reserialize the selectionsFromClient back into arnSerializedInfo
                            selectionsFromClient.ResetToRestSelection();
                            arSerializedInfo = selectionsFromClient.GetSerialization();

                            //TODO - Consider putting a signal in here to let the current player know that they somehow submitted an invalid skill
                            //       Likely this would only occur if there was some sort of programming/sync error though


                        } else {

                            Debug.Log("MASTER: Received valid selection of " + selectionsFromClient.ToString());
                        }
                    }

                    //By this point, we have either received a valid skill, or we have just wrote-in a rest selection.
                    //  Either way, we can save this as a valid selection to be distributed to all players once we are ready
                    //  to progress to the next phase of the turn
                    SaveSerializedSelection((int[])arSerializedInfo);


                } else {
                    //Don't need to do anything special if we recieved the signal from the non-active player
                    // since they're not selecting any skill right now
                }
             break;


        }


        //If we're done any special actions for this phase, then we can just progress this player to the next phase
        int nNextTurnState = GetNextPhase(nClientID, nCurTurnPhase);

        MoveToPhase(nClientID, nNextTurnState, nCurTurnPhase);

    }

    public void ForceAllClientsEndPhase(ContTurns.STATETURN stateTurn) {

        foreach(int i in dictClientExpectedPhase.Keys) {
            if(dictClientExpectedPhase[i] == (int)stateTurn) {
                //Then this is one of the clients we have to manually nudge to end their phase


                if(stateTurn == ContTurns.STATETURN.CHOOSESKILL) {
                    //Pass no selections for the skill we want the character to use (will be reset to a rest)
                    OnClientFinishedPhase(i, (int)stateTurn, null);

                } else {

                    OnClientFinishedPhase(i, (int)stateTurn);

                }
            }
        }
    }


    public void Update() {
        //Remaain inactive if we're not the master
        if(bIsMaster == false) return;

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
