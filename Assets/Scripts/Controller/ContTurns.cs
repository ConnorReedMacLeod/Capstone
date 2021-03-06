﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContTurns : Singleton<ContTurns> {

    public enum STATETURN { DRAFT, BAN, LOADOUTSETUP, RECHARGE, READY, REDUCECOOLDOWNS, GIVEMANA, TURNSTART, CHOOSESKILL, EXECUTESKILL, TURNEND };
    public STATETURN curStateTurn;

    public Chr[] arChrPriority = new Chr[Player.MAXCHRS];
    public Chr chrNextReady; //Stores the currently acting character this turn (or null if none are acting)

    public int nLiveCharacters;
    public int nTurnNumber;

    public Subject subNextActingChrChange = new Subject();
    public static Subject subAllPriorityChange = new Subject(Subject.SubType.ALL);

    public const float fDelayGameEffects = 0.5f;
    public const float fDelayTurnSkill = 0.5f;
    public const float fDelayMinorSkill = 0.5f;
    public const float fDelayStandard = 1.25f;
    public const float fDelayBan = 20f;
    public const float fDelayDraftPick = 20f;
    public const float fDelayLoadoutSetup = 120f;


    public void FixSortedPriority(Chr chr) {

        //Find the referenced character
        int i = 0;
        while(arChrPriority[i] != chr) {
            i++;
        }


        //First try to move ahead the character
        //If there is some character ahead and we go on a earlier turn
        while(i > 0 && arChrPriority[i - 1].GetPriority() > chr.GetPriority()) {
            //Swap these characters
            arChrPriority[i] = arChrPriority[i - 1];
            arChrPriority[i - 1] = chr;
            //And move to the next possible slot
            i--;
        }

        //Next try to move the character back in the list
        //If there is a character after us, and we go on the same turn or later
        while(i < (nLiveCharacters - 1) &&
            chr.GetPriority() >= arChrPriority[i + 1].GetPriority()) {
            //Swap these character
            arChrPriority[i] = arChrPriority[i + 1];
            arChrPriority[i + 1] = chr;
            //And move to the next possible slot
            i++;
        }

        subAllPriorityChange.NotifyObs(this);
    }

    public Chr GetNextActingChr() {

        if(chrNextReady != null && chrNextReady.curStateReadiness.Type() == StateReadiness.TYPE.READY) {
            //If we've already got a reference to the currently acting character, 
            //  and that character is still ready and not dead, then they are the correct next acting character
            return chrNextReady;
        } else if(chrNextReady != null) {
            //If we have a reference to a non-ready character, then reset that reference to null
            //Debug.Log("Resetting chrNextReady since " + chrNextReady.sName + " is in " + chrNextReady.curStateReadiness);

            chrNextReady = null;
        }

        //Now we should look for the first character in our priority queue in a ready state
        int i = 0;

        while(i < nLiveCharacters) {
            //Just skip this character if they don't have their readiness state created yet
            if(arChrPriority[i].curStateReadiness == null) {
                i++;
                continue;
            }

            if(arChrPriority[i].curStateReadiness.Type() == StateReadiness.TYPE.READY) {
                //If we find a character in the ready state, then they will become our new chrNextReady
                chrNextReady = arChrPriority[i];

                Debug.Assert(chrNextReady.bDead == false);
                break;
            }

            i++;
        }

        //Whether we've found a ready character or not, just return whatever's stored in chrNextReady
        return chrNextReady;

    }

    //Fetch the character owned by plyr who will act next
    public Chr GetNextToActOwnedBy(Player plyr) {
        int i = 0;

        while(i < nLiveCharacters) {
            if(arChrPriority[i].plyrOwner == plyr) {
                return arChrPriority[i];
            }

            i++;
        }

        Debug.LogError("Error: Player " + plyr.id + " does not have a live character");
        return null;
    }


    //Copy the array of characters so we have references we can sort by priority
    public void InitChrPriority() {
        for(int i = 0; i < Match.Get().nPlayers; i++) {
            for(int j = 0; j < Match.Get().arPlayers[i].nChrs; j++) {

                arChrPriority[i + 2 * j] = Match.Get().arChrs[i][j];
            }
        }
    }

    //Initially assign Fatigue values to each character
    public void InitChrTurns() {

        for(int i = 0; i < Match.Get().nPlayers; i++) {
            for(int j = 0; j < Match.Get().arPlayers[i].nChrs; j++) {
                //TODO:: Consider putting this on the stack
                //Initially start each character off in a fatigued state with 1/2/3/4/5/6 fatigue
                Match.Get().arChrs[i][j].ChangeFatigue(2 * j + i + 1);

            }
        }

    }


    //This is called if we've cleared out processing the current part of the turn, 
    // so we should send a signal to the master letting them know that this player is done that
    // phase of the turn.
    public void FinishedTurnPhase() {

        if(ContSkillEngine.Get().AreStacksEmpty() == false) {
            Debug.LogError("There's still more to evaluate on the stacks, so we can't finish the turn yet");
            return;
        }

        if(curStateTurn == STATETURN.CHOOSESKILL) {
            //If the current phase of the turn is for choosing skills, then just let
            // the selection controller send its result for when the phase should end
            // (since this may require waiting for AI calculation or human input)
            //They might send it immediately if it's not their turn or if there's no
            // character acting this turn.

            return;
        } else if(curStateTurn == STATETURN.EXECUTESKILL) {
            //After we're done executing the skill (and processing all associated effects) that was passed to us,
            //  then we can clear out the selection info that the master passed to us since we're done with it
            ContSkillSelection.Get().ResetStoredSelection();
        }


        if(ContSkillEngine.bDEBUGENGINE) Debug.Log("Finished the turn phase: " + curStateTurn);

        ClientNetworkController.Get().SendTurnPhaseFinished();

        //We then wait til we get back a signal from the master saying that we can progress to the next phase of the turn

    }

    public void OnLeavingState(object oAdditionalInfo) {

        switch(curStateTurn) {
        case STATETURN.BAN:

            //Interpret the passed info as a selected character that was banned in the just-finished drafting step
            DraftController.Get().BanChr((Chr.CHARTYPE)oAdditionalInfo);

            //End this drafting step and move on to the next one
            DraftController.Get().FinishDraftPhaseStep();

            break;

        case STATETURN.DRAFT:

            //Interpret the passed info as a selected character that was drafted in the just-finished drafting step
            DraftController.Get().DraftChr(DraftController.Get().GetActivePlayerForNextDraftPhaseStep(), (Chr.CHARTYPE)oAdditionalInfo);

            //End this drafting step and move on to the next one
            DraftController.Get().FinishDraftPhaseStep();

            break;
        }

    }

    public void SetTurnState(STATETURN _curStateTurn, object oAdditionalInfo = null) {

        OnLeavingState(oAdditionalInfo);

        curStateTurn = _curStateTurn;

        switch(curStateTurn) {

        case STATETURN.RECHARGE:

            ContSkillEngine.Get().AddExec(new ExecTurnRecharge(_chrSource: null));

            break;

        case STATETURN.READY:

            ContSkillEngine.Get().AddExec(new ExecTurnReady(_chrSource: null));

            break;

        case STATETURN.REDUCECOOLDOWNS:

            ContSkillEngine.Get().AddExec(new ExecTurnReduceCooldowns(_chrSource: null));

            break;

        case STATETURN.GIVEMANA:

            //Interpret the additional info as an array of the types of mana given to each player,
            // and pass this along to the ExecTurnGiveMana that's put on the stack
            Debug.Assert(oAdditionalInfo != null);

            ContSkillEngine.Get().AddExec(new ExecTurnGiveMana(_chrSource: null) { arManaToGive = (int[])oAdditionalInfo });

            break;


        case STATETURN.TURNSTART:

            ContSkillEngine.Get().AddExec(new ExecTurnStartTurn(_chrSource: null));

            break;

        case STATETURN.CHOOSESKILL:

            ContSkillEngine.Get().AddExec(new ExecTurnChooseSkills(_chrSource: null));

            break;

        case STATETURN.EXECUTESKILL:

            //Interpret the additional info passed from Master as skill selection info from the active player
            int nSerializedSelectionInfo = (int)oAdditionalInfo;
            ContSkillSelection.Get().ReceiveSelectionFromMaster(nSerializedSelectionInfo);

            ContSkillEngine.Get().AddExec(new ExecTurnExecuteSkill(_chrSource: null));

            break;

        case STATETURN.TURNEND:

            ContSkillEngine.Get().AddExec(new ExecTurnEndTurn(_chrSource: null));

            break;
        }

        if(ContSkillEngine.Get().bAutoTurns) {
            //Now that the appropriate ExecTurn as been added, we can resume processing the stack
            ContSkillEngine.Get().ProcessStacks();
        }
    }

    public void InitializePriorities() {

        InitChrPriority();
        InitChrTurns();


        nTurnNumber = 1;

        nLiveCharacters = Player.MAXPLAYERS * Player.MAXCHRS;

        ViewPriorityList.Get().InitViewPriorityList();
    }

    public void InitStartingTurnPhase() {
        curStateTurn = STATETURN.TURNEND;
    }

    public override void Init() {

        InitStartingTurnPhase();

    }



}
