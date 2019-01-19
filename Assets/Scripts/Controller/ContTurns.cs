using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContTurns : MonoBehaviour {

    public bool bStarted = false;

    public enum STATETURN {RECHARGE, READY, REDUCECOOLDOWNS, GIVEMANA, TURNSTART, CHOOSEACTIONS, EXECUTEACTIONS, TURNEND };
    public STATETURN curStateTurn;

    public static ContTurns instance;

    public Chr []arChrPriority = new Chr[6];
    public Chr chrNextReady; //Stores the currently acting character this turn (or null if none are acting)
    public static Subject subAllPriorityChange = new Subject();

    public const float fDelayChooseAction = 5.0f;

    
    //TODO CHANGE ALL .Get() calls in other classes to use properties
    //     so the syntax isn't as gross

    public static ContTurns Get() {
        if (instance == null) {
            GameObject go = GameObject.FindGameObjectWithTag("Controller");
            if (go == null) {
                Debug.LogError("ERROR! NO OBJECT HAS A Controller TAG!");
            }
            instance = go.GetComponent<ContTurns>();
            if (instance == null) {
                Debug.LogError("ERROR! Controller TAGGED OBJECT DOES NOT HAVE A ContTurns COMPONENT!");
            }
            instance.Start();
        }
        return instance;
    }

    public void FixSortedPriority(Chr chr) {
        //Find the referenced character
        int i = 0;
        while (arChrPriority[i] != chr) {
            i++;
        }

        //First try to move ahead the character
        //If there is some character ahead and we go on a earlier turn
        while (i > 0 && arChrPriority[i - 1].GetPriority() > chr.GetPriority()) {
            //Swap these characters
            arChrPriority[i] = arChrPriority[i - 1];
            arChrPriority[i - 1] = chr;
            //And move to the next possible slot
            i--;
        }

        //Next try to move the character back in the list
        //If there is a character after us, and we go on the same turn or later
        while (i < (6 - 1) && chr.GetPriority() >= arChrPriority[i + 1].GetPriority()) {
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
            //  and that character is still ready, then they are the correct next acting character
            return chrNextReady;
        } else if (chrNextReady != null) {
            //If we have a reference to a non-ready character, then reset that reference to null
            //Debug.Log("Resetting chrNextReady since " + chrNextReady.sName + " is in " + chrNextReady.curStateReadiness);

            chrNextReady = null;
            
        }

        //Now we should look for the first character in our priority queue in a ready state
        int i = 0;

        while (i < 6) {
            //Just skip this character if they don't have their readiness state created yet
            if (arChrPriority[i].curStateReadiness == null) {
                i++;
                continue;
            }

            if (arChrPriority[i].curStateReadiness.Type() == StateReadiness.TYPE.READY) {
                //If we find a character in the ready state, then they will become our new chrNextReady
                chrNextReady = arChrPriority[i];
                break;
            }

            i++;
        }
        //Whether we've found a ready character or not, just return whatever's stored in chrNextReady
        return chrNextReady;

    }


    //Copy the array of characters so we have references we can sort by priority
    public void InitChrPriority() {
        for (int i = 0; i < Match.Get().nPlayers; i++) {
            for (int j = 0; j < Match.Get().arPlayers[i].nChrs; j++) {

                arChrPriority[i + 2*j] = Match.Get().arChrs[i][j];
            }
        }
    }

    //Initially assign Fatigue values to each character
    public void InitChrTurns() {

        for (int i = 0; i < Match.Get().nPlayers; i++) {
            for (int j = 0; j < Match.Get().arPlayers[i].nChrs; j++) {
                //TODO:: Consider putting this on the stack
                //Initially start each character off in a fatigued state with 1/2/3/4/5/6 fatigue
                Match.Get().arChrs[i][j].ChangeFatigue(2 * j + i + 1);

            }
        }

    }

    
    //This is called if we've cleared out processing the current part of the turn, 
    // so we should add an ExecTurn Executable to next be processed
    public void HandleTurnPhase() {

        if (ContAbilityEngine.bDEBUGENGINE) Debug.Log("Handling the turn for phase: " + curStateTurn);

        switch (curStateTurn) {
            case STATETURN.RECHARGE:

                ContAbilityEngine.Get().AddExec(new ExecTurnRecharge());

                break;

            case STATETURN.READY:

                ContAbilityEngine.Get().AddExec(new ExecTurnReady());

                break;

            case STATETURN.REDUCECOOLDOWNS:

                ContAbilityEngine.Get().AddExec(new ExecTurnReduceCooldowns());

                break;

            case STATETURN.GIVEMANA:

                ContAbilityEngine.Get().AddExec(new ExecTurnGiveMana());

                break;
            
            
            case STATETURN.TURNSTART:

                ContAbilityEngine.Get().AddExec(new ExecTurnStartTurn());

                break;

            case STATETURN.CHOOSEACTIONS:

                ContAbilityEngine.Get().AddExec(new ExecTurnChooseActions());

                break;

            case STATETURN.EXECUTEACTIONS:

                ContAbilityEngine.Get().AddExec(new ExecTurnExecuteAction());

                break;

            case STATETURN.TURNEND:

                ContAbilityEngine.Get().AddExec(new ExecTurnEndTurn());

                break;
        }
    }


    public void SetTurnState(STATETURN _curStateTurn) {
        curStateTurn = _curStateTurn;
    }

    // Use this for initialization
    void Start () {
        if (bStarted) return;

        bStarted = true;
        InitChrPriority();
        InitChrTurns();

    }
}
