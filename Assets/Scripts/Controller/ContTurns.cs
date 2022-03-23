using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContTurns : Singleton<ContTurns> {

    public enum STATETURN { RECHARGE, READY, REDUCECOOLDOWNS, GIVEMANA, TURNSTART, CHOOSESKILL, TURNEND };
    public STATETURN curStateTurn;

    public Chr[] arChrPriority = new Chr[Player.MAXCHRS];
    public Chr chrNextReady; //Stores the currently acting character this turn (or null if none are acting)

    public int nLiveCharacters;
    public int nTurnNumber;



    public Subject subNextActingChrChange = new Subject();
    public Subject subAllPriorityChange = new Subject();
    public Subject subTurnChange = new Subject();

    public void NextTurn() {
        nTurnNumber++;

        subTurnChange.NotifyObs();
    }

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
        if (ContSkillEngine.bDEBUGENGINE) Debug.Log("Finished the turn phase: " + curStateTurn);
        
        //Move to the next phase of the turn
        SetTurnState(GetNextPhase(curStateTurn));
    }

    public STATETURN GetNextPhase(STATETURN turnphase) {

        //Loop around to the recharge phase if we've reached the end of a turn
        if (turnphase == STATETURN.TURNEND) {
            return STATETURN.RECHARGE;

        }else if(turnphase == STATETURN.TURNSTART) {
            //If we'd normally move to choosing a skill for a character, but no character is set to move this turn, then we can directly skip to the end of turn
            if (GetNextActingChr() == null) {
                return STATETURN.TURNEND;
            } else {
                return STATETURN.CHOOSESKILL;
            }
        } else if(turnphase == STATETURN.CHOOSESKILL) {
            //If we have finished choosing and executing a skill, then let's check if we want to stay in the choose-skills phase of the turn,
            // or we can move to the end of turn if no one is left to act
            if(GetNextActingChr() == null) {
                return STATETURN.TURNEND;
            } else {
                return STATETURN.CHOOSESKILL;
            }
        }

        //Generally, just move to the next sequential turn phase
        return turnphase + 1;
    }

    public void OnLeavingState(object oAdditionalInfo) {
        

    }

    public void SetTurnState(STATETURN _curStateTurn) {
        
        OnLeavingState(curStateTurn);

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

            ContSkillEngine.Get().AddExec(new ExecTurnGiveMana(_chrSource: null));

            break;


        case STATETURN.TURNSTART:

            ContSkillEngine.Get().AddExec(new ExecTurnStartTurn(_chrSource: null));

            break;

        case STATETURN.CHOOSESKILL:

            ContSkillEngine.Get().AddExec(new ExecTurnChooseSkills(_chrSource: null));

            break;

        case STATETURN.TURNEND:

            ContSkillEngine.Get().AddExec(new ExecTurnEndTurn(_chrSource: null));

            break;
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
