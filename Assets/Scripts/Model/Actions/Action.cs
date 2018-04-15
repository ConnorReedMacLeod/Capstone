using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action { //This should probably be made abstract

	public enum ActionType {ACTIVE, PASSIVE, CHANNEL};

	public int id;

	public int nArgs;
	public TargetArg [] arArgs;
	public string sName;
	public ActionType type; 
	public int nCd;
	public int nCurCD;
	public int nRecharge;

	public Chr chrOwner;

	public bool bCharges;
	public int nCharges;
	public int nCurCharges;

	public string sDescription;
	public string sExtraDescription;

	public int[] arCost;

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
		
		nCurCD = nCd;
		chrOwner.ChangeRecharge(nRecharge);

		//Let the timeline know about the new slot
		chrOwner.NotifyNewRecharge ();

		if (chrOwner.plyrOwner.mana.SpendMana (arCost)) {
			//Then the mana was paid properly
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

		for (int i = 0; i < nArgs; i++) {
			if (!arArgs [i].VerifyLegal ()) {
				Debug.Log ("Argument " + i + " was invalid");
				return false;
			}
		}
		return true;
	}
		
}
