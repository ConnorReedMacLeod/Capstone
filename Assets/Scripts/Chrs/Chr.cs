using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(ViewChr))]
public class Chr : Subject {

	bool bStarted;

	public enum CHARTYPE {          //CHARTYPE's possible values include all characters in the game
		LANCER, KATARA, SKELCOWBOY
	};

	public enum STATESELECT{
		SELECTED,                   //Initial selection of the character
		TARGGETING,                 //Targetting of character actions
		UNSELECTED                  //Default character state
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
	public Vector3 v3Pos;           //The character's position

    // TODO:: Reconsider making this pos range from -0.5 to 0.5
    //        it's nice for avoiding some standard units of measurement,
    //        but it means there's only one map size

	// TODO:: Make a grid of possible positions

    public int id;                  //The character's unique identifier
    public int nRecharge;           //Number of turns a character must wait before their next action

	public int nCurHealth;          //The character's current health
	public int nMaxHealth;          //The character's max health

    public Action[] arActions;      //The characters actions
    public static int nActions = 8; //Number of actions the character can perform
	public int nUsingAction;        //The currently selected action for the character, either targetting or having been queued
	public bool bSetAction;         //Whether or not the character has an action queued

	public static float[] arfSize = { 0.5f, 0.75f, 1.25f, 2.0f };
	public SIZE size;
	public float fRad;

	public ViewChr view;

	public STATESELECT stateSelect; //The character's state

    
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

    //Sets the character's position
    public void SetPosition(Vector3 _v3Pos){
		Start ();
        v3Pos = _v3Pos;
        NotifyObs();
    }

    //Sets the character's position without need for depth (z)
    public void SetPosition(float _fX, float _fY){
		SetPosition (new Vector3 (_fX, _fY, 0));
	}

    //Sets character state to selected
	public void Select(){
		stateSelect = STATESELECT.SELECTED;
		NotifyObs ();
	}

    //Sets character state to targetting
	public void Targetting(){
		stateSelect = STATESELECT.TARGGETING;
		NotifyObs ();
	}

    //Set character state to unselected
	public void Deselect (){
		stateSelect = STATESELECT.UNSELECTED;
		NotifyObs ();
	}

    //Performs the character's queued action
	public void ExecuteAction(){
		Debug.Assert (ValidAction ());
		arActions [nUsingAction].Execute ();
		nUsingAction = 7;//TODO:: Make thie consistent
	}

    //Checks if the character's selected action is ready and able to be performed
	public bool ValidAction(){
		//Debug.Log (bSetAction + " is the setaction");
		return (bSetAction && arActions [nUsingAction].VerifyLegal ());
	}

	public void SetSize(SIZE _size){
		size = _size;
		fRad = arfSize [(int)size];
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

    //Defines all of a character's unique actions
	public void SetActions(){//TODO:: probably add some parameter for this at some point like an array of ids

		for (int i = 0; i < nActions; i+=2) {
			arActions [i] = new ActionFireball (this);
			arActions [i+1] = new ActionMove (this);
		}
		arActions [7] = new ActionRest (this);
	}


	// Used to initiallize information fields of the Chr
	// Call this after creating to set information
	public void InitChr(Player _plyrOwner, int _id, string _sName, SIZE _size = SIZE.MEDIUM){
		plyrOwner = _plyrOwner;
		id = _id;
		sName = _sName;
		SetSize (_size);

		SetActions ();//TODO:: move this somewhere else at some point

		view.Init ();
	}

    // Sets up fundamental class connections for the Chr
	public override void Start(){
		if (bStarted == false) {
			bStarted = true;

			base.Start ();
			// Call our base Subject's start method

			view = GetComponent<ViewChr> ();
			view.Start (); 
			// Should let the view initialize itself first
			// so that it'll be safe for us to update in our Start method

			arActions = new Action[nActions];
			nUsingAction = -1;

			stateSelect = STATESELECT.UNSELECTED;
		}

	}
		
}
