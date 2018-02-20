using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Subject {

	public enum CHARTYPE {          //CHARTYPE's possible values include all characters in the game
		LANCER, KATARA, SKELCOWBOY
	};

	public enum STATESELECT{
		SELECTED,                   //Initial selection of the character
		TARGGETING,                 //Targetting of character actions
		UNSELECTED                  //Default character state
	}

	Arena arena;                    //The field of play

    public string sName;            //The character's name
    public Player plyrOwner;        //The player who controls the character
    public Vector3 v3Pos;           //The character's position

    // TODO:: Reconsider making this pos range from -0.5 to 0.5
    //        it's nice for avoiding some standard units of measurement,
    //        but it means there's only one map size

    public int id;                  //The character's unique identifier
    public int nRecharge;           //Number of turns a character must wait before their next acion

	public int nCurHealth;          //The character's current health
	public int nMaxHealth;          //The character's max health

    public Action[] arActions;      //The characters actions
    public static int nActions = 8; //Number of actions the character can perform
	public int nUsingAction;        //The currently selected action for the character, either targetting or having been queued
	public bool bSetAction;         //Whether or not the character has an action queued

	public STATESELECT stateSelect; //The character's state

    
    //Changes the character's recharge by a given value
	public void ChangeRecharge(int _nChange){
		if (_nChange + nRecharge < 0) {
			nRecharge = 0;
		} else {
			nRecharge += _nChange;
		}
	}

    //Counts down the character's recharge with the timeline
	public void TimeTick(){
		ChangeRecharge (-1);
	}

    //Sets the character's position
    public void SetPosition(Vector3 _v3Pos){
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
	}

    //Checks if the character's selected action is ready and able to be performed
	public bool ValidAction(){
		return (bSetAction && arActions [nUsingAction].VerifyLegal ());
	}

    //Sets character's selected action to Rest
	public void SetRestAction(){
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

    //Defines the character's controller and ID
	public Character(Player _playOwner, int _id){
		plyrOwner = _playOwner;
		id = _id;

		arActions = new Action[nActions];
		nUsingAction = -1;

		stateSelect = STATESELECT.UNSELECTED;

		SetActions ();//TODO:: move this somewhere else at some point
	}
		
}
