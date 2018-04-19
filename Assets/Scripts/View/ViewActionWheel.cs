using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewActionWheel : Observer {

	//Have a reference to the character so we know what abilities to represent
	public Chr mod;

	public static int MAXACTIONS = 8;
	public ViewAction[] arViewAction;

	public ViewActionWheel(){
		arViewAction = new ViewAction[MAXACTIONS];
	}

	public void setModel(Chr _mod){
		mod = _mod;
		mod.Subscribe (this);

		//TODO:: Register each of the actions this contains
		// currently, all of the registration is done in the inspector
		// I'm not sure how I feel about the sturdiness of that...

		for (int i = 0; i < MAXACTIONS; i++) {
			arViewAction [i].SetActionMaterial ("Default");
			arViewAction [i].SetModel (mod.arActions[i]);
			//arViewAction [i].SetActionMaterial (mod.arActions[i].sName); //TODO: replace when ready
		}
	}

	public void Start(){
        
	}
}
