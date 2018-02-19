﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action { //This should probably be made abstract

	public int id;

	public int nArgs;
	public TargetArg [] arArgs;
	public int nCd;
	public int nCurCD;
	public int nRecharge;
	public Character chrOwner;

	public int[] arCost;

	public Action(int _nArgs, Character _chrOwner){
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

	public void TimeTick(){
		ChangeCD (-1);
	}

	// Should call VerifyLegal() before calling this
	public virtual void Execute(){

		Debug.Assert (VerifyLegal ());
		
		nCurCD = nCd;
		chrOwner.ChangeRecharge(nRecharge);


		if (chrOwner.playOwner.mana.SpendMana (arCost)) {
			//Then the mana was paid properly
		} else {
			Debug.LogError ("YOU DIDN'T ACTUALLY HAVE ENOUGH MANA");
		}

		Reset ();
	}

	public virtual bool VerifyLegal(){// Maybe this doesn't need to be virtual

		//Check if you have enough mana
		if (!chrOwner.playOwner.mana.HasMana (arCost)) {
			return false;
		}

		//Check that the ability isn't on cooldown
		if (nCd != 0) {
			return false;
		}

		for (int i = 0; i < nArgs; i++) {
			arArgs[i].VerifyLegal ();
		}
		return true;
	}
		
}
