using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContTurns : MonoBehaviour {

    public bool bStarted = false;
    public bool bAutoTurns = false;

    public enum STATETURN {GIVEMANA, REDUCECOOLDOWNS, CHOOSEACTIONS, TURNSTART, EXECUTEACTIONS, TURNEND };
    public STATETURN curStateTurn;

    //Delays to wait before we enter the next phase (duration of this phase)
    public float fDelayGiveMana = 0.5f;
    public float fDelayReduceCooldowns = 0.5f;
    public float fDelayChooseActions = 0.0f;
    public float fDelayTurnStart = 0.5f;
    public float fDelayExecuteActions = 0.5f; //Delay between each action
    public float fDelayTurnEnd = 1.0f;

    public static ContTurns instance;

    public Chr []arChrPriority = new Chr[6];
    public static Subject subAllPriorityChange = new Subject();

    public float fDelayChrFirst = 5.0f;
    public float fDelayChrAdditional = 1.0f;

    public static Subject subAllTurnStart = new Subject();
    public static Subject subAllTurnEnd = new Subject();

    public GameObject pfTimer;
    
    //TODO CHANGE ALL .Get() calls in other classes to use properties
    //     so the syntax isn't as gross

    public static ContTurns Get() {
        if (instance == null) {
            GameObject go = GameObject.FindGameObjectWithTag("ContTurns");
            if (go == null) {
                Debug.LogError("ERROR! NO OBJECT HAS A CONTTURNS TAG!");
            }
            instance = go.GetComponent<ContTurns>();
            if (instance == null) {
                Debug.LogError("ERROR! ContTurns TAGGED OBJECT DOES NOT HAVE A ContTurns COMPONENT!");
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

        //Debug.Log("I am " + chr.sName + " and I'm in position " + i + " with fatigue " + chr.nFatigue);

        //First try to move ahead the character
        //If there is some character ahead and we go on a earlier turn
        while (i > 0 && arChrPriority[i - 1].nFatigue > chr.nFatigue) {
            //Swap these characters
            arChrPriority[i] = arChrPriority[i - 1];
            arChrPriority[i - 1] = chr;
            //And move to the next possible slot
            i--;
        }

        //if(i!=0)
        //Debug.Log("I stopped moving left since the previous character is " + arChrPriority[i - 1].sName + " at position " + (i - 1) + " with fatigue " +
        //    arChrPriority[i - 1].nFatigue);

        //Next try to move the character back in the list
        //If there is a character after us, and we go on the same turn or later
        while (i < (6 - 1) && chr.nFatigue >= arChrPriority[i + 1].nFatigue) {
            //Swap these character
            arChrPriority[i] = arChrPriority[i + 1];
            arChrPriority[i + 1] = chr;
            //And move to the next possible slot
            i++;
        }

        //if (i == 5) Debug.Log("I stopped moving right since I have the highest fatigue");

        //if (i != 5) 
        //Debug.Log("I stopped moving right since the next character is " + arChrPriority[i + 1].sName + " at position " + (i + 1) + " with fatigue " +
        //    arChrPriority[i + 1].nFatigue);

        subAllPriorityChange.NotifyObs(this);
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


    public void GiveMana() {
        //TODO::Make this only semi-random
        Mana.MANATYPE manaGen = (Mana.MANATYPE)Random.Range(0, Mana.nManaTypes - 1);

        //Give the mana to each player
        for (int i = 0; i < Match.Get().nPlayers; i++) {
            Match.Get().arPlayers[i].mana.AddMana(manaGen);
        }
    }

    public void ReduceCooldowns() {

        for (int i = 0; i < Match.Get().nPlayers; i++) {
            for (int j = 0; j < Player.MAXCHRS; j++) {
                if (Match.Get().arChrs[i][j] == null) {
                    continue; // A character isn't actually here (extra space for characters)
                }

                //Reduce the character's recharge
                Match.Get().arChrs[i][j].TimeTick();
                //Reduce the cd of that character's actions
                Match.Get().arChrs[i][j].RechargeActions();

            }
        }
    }


    public void StartTurn() {

        subAllTurnStart.NotifyObs(this);
        //TODO - MAKE CHARACTERS OBSERVE THIS AND LOCK THEIR ABILITY SELECTION

    }

    public void EndTurn() {

        subAllTurnEnd.NotifyObs(this);
        //TODO - MAKE CHARACTERS OBSERVE THIS AND UNLOCK THEIR ABILITY SELECTION

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
                Match.Get().arChrs[i][j].ChangeFatigue(2 * j + i + 1);
            }
        }

    }

    public void cbAutoExecuteEvent(Object target, params object[] args) {
        if (bAutoTurns == true) return; //If the button is already pressed
        bAutoTurns = true;

        if (bAutoTurns) {
            Debug.Log("Going to next event in " + 2.0f);

            Invoke("AutoExecuteEvent", 2.0f);
        }
    }
    public void AutoExecuteEvent() {

        if (!bAutoTurns) {
            //Then we must have switched to manual turns while waiting for this event,
            //so don't actually execute anything automatically
            return;
        }

        HandleTurnPhase();
    }

    public void cbManualExecuteEvent(Object target, params object[] args) {
        bAutoTurns = false;

        HandleTurnPhase();
    }

    public void SpawnTimer(float fDelay, string sLabel) {
        GameObject goTimer = Instantiate(pfTimer, Match.Get().transform);
        ViewTimer viewTimer = goTimer.GetComponent<ViewTimer>();
        if (viewTimer == null) {
            Debug.LogError("ERROR - pfTimer doesn't have a viewTimer component");
        }
        viewTimer.InitTimer(fDelay, sLabel);
    }

    public void HandleTurnPhase() {

        float fDelay = 0.0f;
        string sLabel = "";

        switch (curStateTurn) {
            case STATETURN.GIVEMANA:
                GiveMana();

                curStateTurn = STATETURN.REDUCECOOLDOWNS;
                fDelay = fDelayGiveMana;
                sLabel = "Giving Mana";
                break;
            case STATETURN.REDUCECOOLDOWNS:
                ReduceCooldowns();

                curStateTurn = STATETURN.CHOOSEACTIONS;
                fDelay = fDelayReduceCooldowns;
                sLabel = "Reducing Cooldowns";
                break;
            case STATETURN.CHOOSEACTIONS:
                fDelayChooseActions = GetTimeForActing();

                curStateTurn = STATETURN.TURNSTART;
                fDelay = fDelayChooseActions;
                sLabel = "Select Your Actions";
                break;
            case STATETURN.TURNSTART:
                StartTurn();

                curStateTurn = STATETURN.EXECUTEACTIONS;
                fDelay = fDelayTurnEnd;
                sLabel = "Beginning of Turn";
                break;
            case STATETURN.EXECUTEACTIONS:

                if(GetNumAllActingChrs() == 0) {
                    //If no more characters are set to act this turn
                    curStateTurn = STATETURN.TURNEND;
                    sLabel = "All Characters Have Finished Acting";
                } else {
                    //Then at least one character still has to go

                    if (arChrPriority[0].nUsingAction != -1) {
                        sLabel = arChrPriority[0].sName + " is using " + arChrPriority[0].arActions[arChrPriority[0].nUsingAction].sName;
                    } else {
                        sLabel = arChrPriority[0].sName + " has no valid action prepped";
                    }

                    arChrPriority[0].ExecuteAction();
                    
                }

                fDelay = fDelayExecuteActions;
                break;
            case STATETURN.TURNEND:
                EndTurn();

                curStateTurn = STATETURN.GIVEMANA;
                fDelay = fDelayTurnEnd;
                sLabel = "End of Turn";
                break;
        }


        if (bAutoTurns) {

            if(fDelay > 0) {
                //Check if we need to spawn a timer

                SpawnTimer(fDelay, sLabel);
            }

            Invoke("HandleTurnPhase", fDelay);
        } else {
            //Then we're doing manual turns - still spawn a quick timer to show what phase of the turn we're in
            SpawnTimer(1.0f, sLabel);
        }
    }


    // Use this for initialization
    void Start () {
        if (bStarted) return;

        bStarted = true;
        InitChrPriority();
        InitChrTurns();


        ViewAutoTurnsButton.subAllAutoExecuteEvent.Subscribe(cbAutoExecuteEvent);
        ViewManualTurnsButton.subAllManualExecuteEvent.Subscribe(cbManualExecuteEvent);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
