using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContTurns : MonoBehaviour {

    public bool bStarted = false;

    public enum STATETURN {RECHARGE, READY, REDUCECOOLDOWNS, GIVEMANA, TURNSTART, CHOOSEACTIONS, EXECUTEACTIONS, TURNEND };
    public STATETURN curStateTurn;

    public static ContTurns instance;

    public Chr []arChrPriority = new Chr[6];
    public static Subject subAllPriorityChange = new Subject();

    public float fDelayChrFirst = 5.0f;
    public float fDelayChrAdditional = 1.0f;

    
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
        while (i > 0 && arChrPriority[i - 1].nFatigue > chr.nFatigue) {
            //Swap these characters
            arChrPriority[i] = arChrPriority[i - 1];
            arChrPriority[i - 1] = chr;
            //And move to the next possible slot
            i--;
        }

        //Next try to move the character back in the list
        //If there is a character after us, and we go on the same turn or later
        while (i < (6 - 1) && chr.nFatigue >= arChrPriority[i + 1].nFatigue) {
            //Swap these character
            arChrPriority[i] = arChrPriority[i + 1];
            arChrPriority[i + 1] = chr;
            //And move to the next possible slot
            i++;
        }

        subAllPriorityChange.NotifyObs(this);
    }
    
    public Chr GetNextActingChr() {

        if (arChrPriority[0].nFatigue == 0) {
            //TODO:: Consider adding another check here
            //       for if the character started the turn at 0 fatigue
            //       so we don't have characters reducing their fatigue to go immediately
            return arChrPriority[0];
        }

        //Then no more characters are going this turn
        return null;
    }


    public int GetNumActingChrs(int nOwnerId) {
        int nActingChrs = 0;
        for (int i=0; i<6; i++) {
            if(arChrPriority[i].plyrOwner.id == nOwnerId) {
                if(arChrPriority[i].nFatigue == 0) {
                    nActingChrs++;
                }
            }
        }
        return nActingChrs;
    }

    public int GetNumAllActingChrs() {
        return GetNumActingChrs(0) + GetNumActingChrs(1);
    }

    public float GetTimeForActing() {
        int nMaxChrsActing = Mathf.Max(GetNumActingChrs(0), GetNumActingChrs(1));

        float fDelay = 0.0f;
        if(nMaxChrsActing >= 1) {
            fDelay += fDelayChrFirst;
            if(nMaxChrsActing > 1) {
                fDelay += fDelayChrAdditional * (nMaxChrsActing - 1);
            }
        }

        return fDelay;
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
                //Initially start each character off in a fatigued state with 1/2/3 fatigue
                StateFatigued newState = new StateFatigued(Match.Get().arChrs[i][j], 2 * j + i + 1);

                Match.Get().arChrs[i][j].SetStateReadiness(newState);
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
