using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRest : Action {



	public ActionRest(Chr _chrOwner): base(0, _chrOwner){//number of target arguments

		arCost = new int[]{0,0,0,0,0};

		nCd = 0;
		nRecharge = 1;

	}

	override public void Execute(){

		Debug.Log ("Just resting lol");

		base.Execute ();
	}

}
