using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContTurns : MonoBehaviour {

    public bool bStarted = false;

    public static ContTurns instance;

    public Chr []arChrPriority = new Chr[6];
    public static Subject subAllPriorityChange = new Subject();

    public float fDelayChrFirst = 30.0f;
    public float fDelayChrAdditional = 15.0f;

    public static Subject subAllTurnStart = new Subject();
    public static Subject subAllTurnEnd = new Subject();
    
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

        Debug.Log("I am " + chr.sName + " and I'm in position " + i + " with fatigue " + chr.nFatigue);

        //First try to move ahead the character
        //If there is some character ahead and we go on a earlier turn
        while (i > 0 && arChrPriority[i - 1].nFatigue > chr.nFatigue) {
            //Swap these characters
            arChrPriority[i] = arChrPriority[i - 1];
            arChrPriority[i - 1] = chr;
            //And move to the next possible slot
            i--;
        }

        if(i!=0)
        Debug.Log("I stopped moving left since the previous character is " + arChrPriority[i - 1].sName + " at position " + (i - 1) + " with fatigue " +
            arChrPriority[i - 1].nFatigue);

        //Next try to move the character back in the list
        //If there is a character after us, and we go on the same turn or later
        while (i < (6 - 1) && chr.nFatigue >= arChrPriority[i + 1].nFatigue) {
            //Swap these character
            arChrPriority[i] = arChrPriority[i + 1];
            arChrPriority[i + 1] = chr;
            //And move to the next possible slot
            i++;
        }

        if (i == 5) Debug.Log("I stopped moving right since I have the highest fatigue");

        if (i != 5) 
        Debug.Log("I stopped moving right since the next character is " + arChrPriority[i + 1].sName + " at position " + (i + 1) + " with fatigue " +
            arChrPriority[i + 1].nFatigue);

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
        for (int i = 0; i < Timeline.Get().match.nPlayers; i++) {
            Timeline.Get().match.arPlayers[i].mana.AddMana(manaGen);
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

    }

    public void EndTurn() {

        subAllTurnEnd.NotifyObs(this);

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
                Debug.Log("Changing fatigue of " + Match.Get().arChrs[i][j] + " to " + (2 * j + i + 1));
                Match.Get().arChrs[i][j].ChangeFatigue(2 * j + i + 1);
            }
        }

    }

    // Use this for initialization
    void Start () {
        if (bStarted) return;

        bStarted = true;
        InitChrPriority();
        InitChrTurns();

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
