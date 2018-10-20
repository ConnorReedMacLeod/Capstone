using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(ViewChr))]
public class Chr : MonoBehaviour {

	bool bStarted;

	public enum CHARTYPE {          //CHARTYPE's possible values include all characters in the game
		LANCER, KATARA, SKELCOWBOY
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
    public int nRecharge;           //Number of turns a character must wait before their next action

	public int nCurHealth;          //The character's current health
	public int nMaxHealth;          //The character's max health

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

    public Subject subHealthChange = new Subject();
    public Subject subStatusChange = new Subject();

    //Changes the character's recharge by a given value
    public void ChangeRecharge(int _nChange){
		if (_nChange + nRecharge < 0) {
			nRecharge = 0;
		} else {
			nRecharge += _nChange;
		}
	}


	public void NotifyNewRecharge(){
		Timeline.Get ().AddEventChr (this, nRecharge, Timeline.PRIORITY.NONE); 
	}

  //Counts down the character's recharge with the timeline
	public void TimeTick(){
		ChangeRecharge (-1);
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
		Debug.Assert (ValidAction ());
		arActions [nUsingAction].Execute ();
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

			arActions = new Action[nActions];
			nUsingAction = -1;

			stateSelect = STATESELECT.IDLE;
		}

	}
}
