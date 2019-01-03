﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(ViewChr))]
public class Chr : MonoBehaviour {

	bool bStarted;

	public enum CHARTYPE {          //CHARTYPE's possible values include all characters in the game
		FISCHER, KATARINA, SKELCOWBOY, SOHPIDIA, PITBEAST, RAYNE, SAIKO
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
	public Property<int> pnMaxHealth;          //The character's max health

    public Property<int> pnPower;              //The character's current power
    public Property<int> pnDefense;            //The character's current defense

    public Property<int> pnArmour;          //The character's current armour
    public int nAbsorbedArmour;             //The amount of damage currently taken by armour

    public bool bLockedTargetting;  //Whether or not the character can select their action
    public Action[] arActions;      //The characters actions
    public static int nActions = 9; //Number of actions the character can perform
    public static int nCharacterActions = 8; // Number of non-standard actions
    public int nUsingAction;        //The currently selected action for the character, either targetting or having been queued
	public bool bSetAction;         //Whether or not the character has an action queued

    public const int idResting = 7;  //id for the resting action
    public const int idBlocking = 8; //id for the blocking action

    public bool bBlocker;           //Whether or not the character is the assigned blocker
    public Property<bool> pbCanBlock;          //Whether the character is capable or not of blocking

    public SoulContainer soulContainer; //A reference to the characters list of soul effects

	public ViewChr view;

	public STATESELECT stateSelect; //The character's state

    public Subject subStartSelect = new Subject();
    public static Subject subAllStartSelect = new Subject();
    public Subject subStartTargetting = new Subject();
    public static Subject subAllStartTargetting = new Subject();
    public Subject subStartIdle = new Subject();
    public static Subject subAllStartIdle = new Subject();

    public Subject subPreExecuteAbility = new Subject();
    public static Subject subAllPreExecuteAbility = new Subject();
    public Subject subPostExecuteAbility = new Subject();
    public static Subject subAllPostExecuteAbility = new Subject();

    public Subject subLifeChange = new Subject();
    public Subject subArmourCleared = new Subject();
    public Subject subFatigueChange = new Subject();
    public static Subject subAllFatigueChange = new Subject();
    public Subject subBlockerChanged = new Subject();

    public Subject subStatusChange = new Subject();
    public static Subject subAllStatusChange = new Subject();

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

    //If we have lowered our armour (either by taking damage, or by
    // having an armour buff expire), then check if we have no armour left
    public void CheckNoArmour() {

        if (pnArmour.Get() < nAbsorbedArmour) {
            Debug.LogError("ERROR - " + sName + "'s armour is at " + pnArmour.Get() + " but we've absorbed " + nAbsorbedArmour);
        } else if (pnArmour.Get() == 0) {
            //Then we have used up all of our armour

            //Reset the armour absorbed amount to 0
            nAbsorbedArmour = 0;

            //So clear out the armour modifiers and reset armour to 0
            pnArmour = new Property<int>(0);

            //Notify all Armour buffs on this character that they should be dispelled
            subArmourCleared.NotifyObs();
        }

    }

    public void AddArmour(int nChange) {

        //TODO:: Just make a nAbsorbedByArmour value that keeps track of damage sustained by armour
        //       If this ever goes negative, or reachs 0, clear the pnArmour modifiers
        //       If a Armour Modifier ever expires, then it should also decrease the nAbsorbedByArmour
        //       by the same amount

        pnArmour.AddModifier((nBelow) => nBelow += nChange);
        
    }

    public void DamageArmour(int nDamage) {

        //Increase the current amount of damage absorbed by armour value
        nAbsorbedArmour += nDamage;

        //Then check if we've destroyed all of the armour currently on the character
        CheckNoArmour();

    }



    public void TakeDamage(Damage dmgToTake) {

        //Fetch the amount of damage we're going to take
        int nDamageToTake = dmgToTake.GetDamage();

        //If the damage isn't piercing, then reduce it by the defense amount
        if (dmgToTake.bPiercing == false) {
            //Reduce the damage to take by our defense (but ensure it doesn't go below 0)
            nDamageToTake = Mathf.Max(0, nDamageToTake - pnDefense.Get());
        }

        int nArmouredDamage = 0;

        if (dmgToTake.bPiercing == false) {
            //Deal as much damage as we can (but not more than how much armour we have)
            nArmouredDamage = Mathf.Min(nDamageToTake, pnArmour.Get());

            //If there's actually damage that needs to be dealt to armour
            if (nArmouredDamage > 0) {
                DamageArmour(nArmouredDamage);
            }
        }

        //Calculate how much damage still needs to be done after armour
        int nAfterArmourDamage = nDamageToTake - nArmouredDamage;

        //If there's damage to be done, then deal it to health
        if (nAfterArmourDamage > 0) {
            ChangeHealth(-nAfterArmourDamage);
        }
    }

    public void ChangeHealth(int nChange) {
        nCurHealth += nChange;

        subLifeChange.NotifyObs();
    }

    public void ChangeBlocker(bool _bBlocker) {
        bBlocker = _bBlocker;

        subBlockerChanged.NotifyObs();
    }

    public bool CanBlock() {
        return !bBlocker && pbCanBlock.Get();
    }

  //Counts down the character's recharge with the timeline
	public void TimeTick(){
		ChangeFatigue (-1, true);
	}

    public void ChangeState(STATESELECT _stateSelect) {
        stateSelect = _stateSelect;

        subStatusChange.NotifyObs(this);
        subAllStatusChange.NotifyObs(this);
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

        Action actToUse = arActions[nUsingAction];

        subPreExecuteAbility.NotifyObs(this, actToUse);
        subAllPreExecuteAbility.NotifyObs(this, actToUse);

        arActions [nUsingAction].Execute ();
        bSetAction = false;
		nUsingAction = 7;//TODO:: Make this consistent

        subPostExecuteAbility.NotifyObs(this, actToUse);
        subAllPostExecuteAbility.NotifyObs(this, actToUse);
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
		nUsingAction = idResting;
	}

    //By default, set all character actions to resting
    public virtual void SetDefaultActions() {//TODO:: probably add some parameter for this at some point like an array of ids

        for (int i = 0; i < nActions; i++) {
            arActions[i] = new ActionRest(this);
        }
    }

    //Get a refernce to the enemy player
    public Player GetEnemyPlayer() {
        if (plyrOwner.id == 0) {
            return Match.Get().arPlayers[1];
        } else {
            return Match.Get().arPlayers[0];
        }
    }

    public void SetAction(int i, Action actNew) {
        //If there is an action already in this slot
        if (arActions[i] != null) {
            //Then call it's unequip method since it's leaving
            arActions[i].OnUnequip();
        }

        arActions[i] = actNew;
        actNew.id = i;

        //If we've set this to be a non-null action
        if(arActions[i] != null) {
            arActions[i].OnEquip();
        }
    }

    public void SetBaseActions() {
        //Sets the basic generic actions like resting and blocking

        SetAction(7, new ActionRest(this));
        SetAction(8, new ActionBlock(this));

    }

    // Used to initiallize information fields of the Chr
    // Call this after creating to set information
    public void InitChr(Player _plyrOwner, int _id, BaseChr baseChr){
		plyrOwner = _plyrOwner;
		id = _id;

        SetDefaultActions();

        baseChr.SetName();
        SetBaseActions();
        baseChr.SetActions();
        baseChr.SetMaxHealth();

		view.Init ();
	}

    // Sets up fundamental class connections for the Chr
	public void Start(){
		if (bStarted == false) {
			bStarted = true;

            nMaxActionsLeft = 1;
            nCurActionsLeft = nMaxActionsLeft;

            arActions = new Action[nActions];
            nUsingAction = -1;

            stateSelect = STATESELECT.IDLE;

            bLockedTargetting = true;

            pbCanBlock = new Property<bool>(true);

            pnMaxHealth = new Property<int>(100);
            pnArmour = new Property<int>(0);

            pnPower = new Property<int>(0);
            pnDefense = new Property<int>(0);

            view = GetComponent<ViewChr>();
            view.Start ();
        }

	}
}


    //Add a max health initializer in each instance of a character - add an 
    // initializer in the base chr that sets curhealth to max health