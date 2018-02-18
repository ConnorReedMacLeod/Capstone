using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action { //This should probably be made abstract

	public int id;

	public int nArgs;
	public TargetArg [] arArgs;
	public int nCd;
	public int nRecharge;
	public Character chrOwner;

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

	// Should call VerifyLegal() before calling this
	public virtual void Execute(){
		// TODO: stuff with cd + recharge
		// TODO: pay mana
	}

	public virtual bool VerifyLegal(){// Maybe this doesn't need to be virtual

		for (int i = 0; i < nArgs; i++) {
			arArgs[i].VerifyLegal ();
		}
		return true;
	}
		
}
