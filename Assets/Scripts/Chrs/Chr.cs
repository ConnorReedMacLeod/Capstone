using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(ViewChr))]
public class Chr : MonoBehaviour {

	bool bStarted;

	public enum CHARTYPE {          //CHARTYPE's possible values include all characters in the game
		LANCER, KATARA, SKELCOWBOY, SNEKGIRL
	};

	public enum STATESELECT{
		SELECTED,                   //Selected a character (to see status effects, actions)
		TARGGETING,                 //Targetting of character actions
		IDLE                 		//Default character state
	};

	public enum SIZE{
		SMALL,
		MEDIUM,
		LARGE,
		GIANT
	};

	Arena arena;                    //The field of play

	public string sName;			//The name of the character
	public Player plyrOwner;        //The player who controls the character


    public int id;                  //The character's unique identifier
    public int nFatigue;            //Number of turns a character must wait before their next action
    public int nQueuedFatigue;      //The amount of fatigue that will be added to the characters fatigue when they're done acting for the turn

    public int nMaxActionsLeft;     //The total maximum number of actions a character can use in a turn (usually 1, cantrips cost 0)
    public int nCurActionsLeft;     //The number of actions left in a turn that the character can use (cantrips cost 0)

	public int nCurHealth;          //The character's current health
	public int nMaxHealth;          //The character's max health

    public bool bLockedTargetting;  //Whether or not the character can select their action
    public Action[] arActions;      //The characters actions
    public static int nActions = 8; //Number of actions the character can perform
	public int nUsingAction;        //The currently selected action for the character, either targetting or having been queued
	public bool bSetAction;         //Whether or not the character has an action queued

	public ViewChr view;

	public STATESELECT stateSelect; //The character's state

    public Subject subStartSelect = new Subject();
    public static Subject subAllStartSelect = new Subject();
    public Subject subStartTargetting = new Subject();
    public static Subject subAllStartTargetting = new Subject();
    public Subject subStartIdle = new Subject();
    public static Subject subAllStartIdle = new Subject();

    public Subject subFatigueChange = new Subject();
    public static Subject subAllFatigueChange = new Subject();

    public Subject subHealthChange = new Subject();
    public Subject subStatusChange = new Subject();


    // Prepare a certain amount of fatigue to be applied to this character
    public void QueueFatigue(int _nChange) {

        nQueuedFatigue += _nChange;

    }

    // Apply this amount of fatigue to the character
    public void ChangeFatigue(int _nChange, bool bBeginningTurn = false){
		if (_nChange + nFatigue < 0) {
			nFatigue = 0;
		} else {
			nFatigue += _nChange;
		}

        subFatigueChange.NotifyObs(this);
        subAllFatigueChange.NotifyObs(this);

        if (!bBeginningTurn) {
            //Then this is a stun or an actions used
            ContTurns.Get().FixSortedPriority(this);
            //So make sure we're in the right place in the priority list
        }
    }

    public void FinishSelectionPhase() {
        // Note - this  queued amount will always be greater than 0, so we will never come back to the same character
        //        twice in a row on the same turn.  
        ChangeFatigue(nQueuedFatigue);
        nQueuedFatigue = 0;
        nCurActionsLeft = nMaxActionsLeft;
    }

    public void UnlockTargetting() {
        bLockedTargetting = false;
    }

    public void LockTargetting() {
        bLockedTargetting = true;
    }

    public void RechargeActions() {

        for (int i = 0; i < Chr.nActions; i++) {
            arActions[i].Recharge();
        }
    }

    public void ChangeHealth(int nChange) {
        nCurHealth += nChange;

        subHealthChange.NotifyObs();
    }

  //Counts down the character's recharge with the timeline
	public void TimeTick(){
		ChangeFatigue (-1, true);
	}

    public void ChangeState(STATESELECT _stateSelect) {
        stateSelect = _stateSelect;

        subStatusChange.NotifyObs(this);
    }

    //Sets character state to selected
	public void Select(){
        ChangeState(STATESELECT.SELECTED);

        subStartSelect.NotifyObs(this);
        subAllStartSelect.NotifyObs(this);
    }

    //Sets character state to targetting
	public void Targetting(){
		ChangeState(STATESELECT.TARGGETING);

        subStartTargetting.NotifyObs(this);
        subAllStartTargetting.NotifyObs(this);
    }

    //Set character state to unselected
	public void Idle (){
		ChangeState(STATESELECT.IDLE);

        subStartIdle.NotifyObs(this);
        subAllStartIdle.NotifyObs(this);
    }

    //Performs the character's queued action
	public void ExecuteAction(){
        if (!ValidAction()) {
            Debug.LogError("ERROR! This ability was targetted, but is no longer a valid action");
            SetRestAction();
        }

		arActions [nUsingAction].Execute ();
        bSetAction = false;
		nUsingAction = 7;//TODO:: Make this consistent
	}

    //Checks if the character's selected action is ready and able to be performed
	public bool ValidAction(){
		//Debug.Log (bSetAction + " is the setaction");
		return (bSetAction && arActions [nUsingAction].VerifyLegal ());
	}

    //Sets character's selected action to Rest
	public void SetRestAction(){
		Debug.Log ("Had to reset to a rest action");
		if (nUsingAction != -1) {
			arActions [nUsingAction].Reset ();
		}
		bSetAction = true;
		nUsingAction = 7;//TODO::Make this consistent
	}

    //By default, set all character actions to resting
    public virtual void SetDefaultActions() {//TODO:: probably add some parameter for this at some point like an array of ids

        for (int i = 0; i < nActions; i++) {
            arActions[i] = new ActionRest(this);
        }
    }

    // Used to initiallize information fields of the Chr
    // Call this after creating to set information
    public void InitChr(Player _plyrOwner, int _id, BaseChr baseChr){
		plyrOwner = _plyrOwner;
		id = _id;

        SetDefaultActions();

        baseChr.SetName();
        baseChr.SetActions();

		view.Init ();
	}

    // Sets up fundamental class connections for the Chr
	public void Start(){
		if (bStarted == false) {
			bStarted = true;

			view = GetComponent<ViewChr> ();
			view.Start ();
            // Should let the view initialize itself first
            // so that it'll be safe for us to update in our Start method

            nMaxActionsLeft = 1;
            nCurActionsLeft = nMaxActionsLeft;

            arActions = new Action[nActions];
			nUsingAction = -1;

			stateSelect = STATESELECT.IDLE;

            bLockedTargetting = true;
		}

	}
}
