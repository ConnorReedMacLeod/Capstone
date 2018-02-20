using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRest : Action {



	public ActionRest(Character _chrOwner): base(0, _chrOwner){//number of target arguments

		arCost = new int[]{0,0,0,0,0};

		nCd = 0;
		nRecharge = 2;

	}

	override public void Execute(){

		Debug.Log ("Just resting lol");

		base.Execute ();
	}

}
