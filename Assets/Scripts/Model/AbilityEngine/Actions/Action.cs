using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action { //This should probably be made abstract

    public enum ActionType { ACTIVE, PASSIVE, CHANNEL, CANTRIP };

    public int id;

    public int nArgs; // Note that this should only ever be 0 or 1
    public TargetArg[] arArgs;
    public string sName;
    public ActionType type;

    public int nCd;
    public int nCurCD;
    public int nFatigue;

    public int nActionCost; // How many action 'points' this ability uses - cantrips would cost 0

    public Chr chrSource;

    public bool bCharges;
    public int nCharges;
    public int nCurCharges;

    public string sDescription;
    public string sExtraDescription;
    
    public Property<int[]> parCost;

    public Stack<Clause> stackClauses = new Stack<Clause>();

    public Subject subAbilityChange = new Subject();

    public Action(int _nArgs, Chr _chrOwner) {
        nArgs = _nArgs;
        chrSource = _chrOwner;


        arArgs = new TargetArg[nArgs];

    }

    public void SetArgOwners() {
        for (int i = 0; i < nArgs; i++) {
            arArgs[i].setOwner(chrSource);
        }
    }

    public void ResetTargettingArgs() {
        for (int i = 0; i < nArgs; i++) {
            arArgs[i].Reset();
        }
    }

    public void ChangeCD(int _nChange) {
        if (_nChange + nCurCD < 0) {
            // Don't let reductions go negative
            nCurCD = 0;
        } else {
            nCurCD += _nChange;
            subAbilityChange.NotifyObs();

        }
    }

    public void Recharge() {
        ChangeCD(-1);
    }

    //What should happen when this action is added to the list of abilities
    public virtual void OnEquip() {

        while (stackClauses.Count != 0) {
            //Add each clause in this ability to the stack
            ContAbilityEngine.Get().AddClause(stackClauses.Pop());
        }

    }

    //What should happen when this action is remove from the list of abilities
    public virtual void OnUnequip() {

        while (stackClauses.Count != 0) {
            //Add each clause in this ability to the stack
            ContAbilityEngine.Get().AddClause(stackClauses.Pop());
        }

    }

	// Should call VerifyLegal() before calling this
	public virtual void Execute(){

		Debug.Assert (VerifyLegal ());
		
        //TODO:: Consider if cooldowns and fatigue should be set before or 
        //       after the ability finishes resolving

		nCurCD = nCd;
		chrSource.QueueFatigue(nFatigue);

        Debug.Assert(chrSource.nCurActionsLeft >= nActionCost);
        chrSource.nCurActionsLeft -= nActionCost;

		if (chrSource.plyrOwner.mana.SpendMana (parCost.Get())) {
            //Then the mana was paid properly

            while (stackClauses.Count != 0) {
                //Add each clause in this ability to the stack
                ContAbilityEngine.Get().AddClause(stackClauses.Pop());
            }

        } else {
			Debug.LogError ("YOU DIDN'T ACTUALLY HAVE ENOUGH MANA");
		}
        
		ResetTargettingArgs ();
        subAbilityChange.NotifyObs();
    }

	public virtual bool VerifyLegal(){// Maybe this doesn't need to be virtual

		//Check if you have enough mana
		if (!chrSource.plyrOwner.mana.HasMana (parCost.Get())) {
			Debug.Log ("Not enough mana");
			return false;
		}

		//Check that the ability isn't on cooldown
		if (nCurCD != 0) {
			Debug.Log ("Ability on cd");
			return false;
		}

        if (nActionCost > chrSource.nMaxActionsLeft) {
            Debug.Log("We have already used all non-cantrip actions for the turn");
            return false;
        }

		for (int i = 0; i < nArgs; i++) {
			if (!arArgs [i].VerifyLegal ()) {
				Debug.Log ("Argument " + i + " was invalid");
				return false;
			}
		}
		return true;
	}
		
}
