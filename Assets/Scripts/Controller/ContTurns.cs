using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContTurns : Singleton<ContTurns> {

    public enum STATETURN { RECHARGE, READY, REDUCECOOLDOWNS, GIVEMANA, TURNSTART, CHOOSESKILL, TURNEND };
    public STATETURN curStateTurn;

    public List<Chr> lstChrPriority;
    public Chr chrNextReady; //Stores the currently acting character this turn (or null if none are acting)

    public int nTurnNumber;



    public Subject subNextActingChrChange = new Subject();
    public Subject subAllPriorityChange = new Subject();
    public Subject subTurnChange = new Subject();
    public Subject subChrAddedPriority = new Subject();
    public Subject subChrRemovedPriority = new Subject();

    public void NextTurn() {
        nTurnNumber++;

        subTurnChange.NotifyObs();
    }

    public void FixSortedPriority(Chr chr) {

        //Find the referenced character
        int i = 0;
        while(lstChrPriority[i] != chr) {
            i++;

            if(i == lstChrPriority.Count) {
                //Debug.LogErrorFormat("Tried to find {0} in the priority list, but they didn't exist", chr);
                return;
            }
        }

        //First try to move ahead the character
        //If there is some character ahead and we go on a earlier turn
        while (i > 0 && lstChrPriority[i - 1].GetPriority() > chr.GetPriority()) {
            //Swap these characters
            lstChrPriority[i] = lstChrPriority[i - 1];
            lstChrPriority[i - 1] = chr;
            //And move to the next possible slot
            i--;
        }

        //Next try to move the character back in the list
        //If there is a character after us, and we go on the same turn or later
        while(i < lstChrPriority.Count - 1 &&
            chr.GetPriority() >= lstChrPriority[i + 1].GetPriority()) {
            //Swap these character
            lstChrPriority[i] = lstChrPriority[i + 1];
            lstChrPriority[i + 1] = chr;
            //And move to the next possible slot
            i++;
        }
        

        subAllPriorityChange.NotifyObs(this);
    }

    public void AddChrToPriorityList(Chr chr) {

        lstChrPriority.Add(chr);

        subChrAddedPriority.NotifyObs(chr);

        //Nudge the new character into the appropriate position
        FixSortedPriority(chr);

    }

    public void RemoveChrFromPriorityList(Chr chr) {

        //Find the referenced character
        int i = 0;
        while(lstChrPriority[i] != chr) {
            i++;

            if(i == lstChrPriority.Count) {
                Debug.LogErrorFormat("Tried to find {0} in the priority list, but they didn't exist", chr);
                return;
            }
        }

        //Now that we've found the character, swap them to the very end of the priority list
        while(i < lstChrPriority.Count - 1) {
            //Swap this character with the next
            lstChrPriority[i] = lstChrPriority[i + 1];
            lstChrPriority[i + 1] = chr;
            //And move to the next possible slot
            i++;
        }

        //Now that we've reached the end of the priority list and this character is guaranteed to be at the very end,
        //   we're safe to remove them
        lstChrPriority.RemoveAt(lstChrPriority.Count - 1);
        
        subChrRemovedPriority.NotifyObs(chr);
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

        while(i < lstChrPriority.Count) {
            //Just skip this character if they don't have their readiness state created yet
            if(lstChrPriority[i].curStateReadiness == null) {
                i++;
                continue;
            }

            if(lstChrPriority[i].curStateReadiness.Type() == StateReadiness.TYPE.READY) {
                //If we find a character in the ready state, then they will become our new chrNextReady
                chrNextReady = lstChrPriority[i];

                Debug.Assert(chrNextReady.bDead == false);
                break;
            }

            i++;
        }

        //Whether we've found a ready character or not, just return whatever's stored in chrNextReady
        return chrNextReady;

    }

    //This is called if we've cleared out processing the current part of the turn, 
    // so we should send a signal to the master letting them know that this player is done that
    // phase of the turn.
    public void FinishedTurnPhase() {

        if(ContSkillEngine.Get().AreStacksEmpty() == false) {
            Debug.LogError("There's still more to evaluate on the stacks, so we can't finish the turn yet");
            return;
        }
        if(ContSkillEngine.bDEBUGENGINE) Debug.Log("Finished the turn phase: " + curStateTurn);

        //Move to the next phase of the turn
        SetTurnState(GetNextPhase(curStateTurn));
    }

    public STATETURN GetNextPhase(STATETURN turnphase) {

        //Loop around to the recharge phase if we've reached the end of a turn
        if(turnphase == STATETURN.TURNEND) {
            return STATETURN.RECHARGE;

        } else if(turnphase == STATETURN.TURNSTART) {
            //If we'd normally move to choosing a skill for a character, but no character is set to move this turn, then we can directly skip to the end of turn
            if(GetNextActingChr() == null) {
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

    public void InitializePriorityList() {

        lstChrPriority = new List<Chr>();

        nTurnNumber = 0;

        ViewPriorityList.Get().InitViewPriorityList();
    }

    public void InitStartingTurnPhase() {
        curStateTurn = STATETURN.TURNEND;
    }

    public override void Init() {

        InitStartingTurnPhase();

    }



}
