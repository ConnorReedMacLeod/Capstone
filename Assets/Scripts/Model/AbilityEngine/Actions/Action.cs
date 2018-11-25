using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action { //This should probably be made abstract

	public enum ActionType {ACTIVE, PASSIVE, CHANNEL, CANTRIP};

	public int id;

	public int nArgs; // Note that this should only ever be 0 or 1
	public TargetArg [] arArgs;
	public string sName;
	public ActionType type; 

	public int nCd;
	public int nCurCD;
	public int nFatigue;

    public int nActionCost; // How many action 'points' this ability uses - cantrips would cost 0

	public Chr chrOwner;

	public bool bCharges;
	public int nCharges;
	public int nCurCharges;

	public string sDescription;
	public string sExtraDescription;

	public int[] arCost;

    public Stack<Clause> stackClauses = new Stack<Clause>();

	public Action(int _nArgs, Chr _chrOwner){
		nArgs = _nArgs;
		chrOwner = _chrOwner;


		arArgs = new TargetArg[nArgs];

	}

	public void SetArgOwners(){
		for (int i = 0; i < nArgs; i++) {
			arArgs [i].setOwner(chrOwner);
		}
	}

	public void Reset(){
		for (int i = 0; i < nArgs; i++) {
			arArgs [i].Reset ();
		}
	}

	public void ChangeCD(int _nChange){
		if (_nChange + nCurCD < 0) {
			// Don't let reductions go negative
			nCurCD = 0;
		} else {
			nCurCD += _nChange;
		}
	}

	public void Recharge(){
		ChangeCD (-1);
	}

	// Should call VerifyLegal() before calling this
	public virtual void Execute(){

		Debug.Assert (VerifyLegal ());
		
        //TODO:: Consider if cooldowns and fatigue should be set before or 
        //       after the ability finishes resolving

		nCurCD = nCd;
		chrOwner.QueueFatigue(nFatigue);

        Debug.Assert(chrOwner.nCurActionsLeft >= nActionCost);
        chrOwner.nCurActionsLeft -= nActionCost;

		if (chrOwner.plyrOwner.mana.SpendMana (arCost)) {
            //Then the mana was paid properly

            while (stackClauses.Count != 0) {
                //Add each clause in this ability to the stack
                ContAbilityEngine.Get().AddClause(stackClauses.Pop());
            }

        } else {
			Debug.LogError ("YOU DIDN'T ACTUALLY HAVE ENOUGH MANA");
		}



		Reset ();
	}

	public virtual bool VerifyLegal(){// Maybe this doesn't need to be virtual

		//Check if you have enough mana
		if (!chrOwner.plyrOwner.mana.HasMana (arCost)) {
			Debug.Log ("Not enough mana");
			return false;
		}

		//Check that the ability isn't on cooldown
		if (nCurCD != 0) {
			Debug.Log ("Ability on cd");
			return false;
		}

        if (nActionCost > chrOwner.nMaxActionsLeft) {
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
