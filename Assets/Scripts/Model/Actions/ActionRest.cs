using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRest : Action {



	public ActionRest(Chr _chrOwner): base(0, _chrOwner){//number of target arguments

		sName = "Rest";
		type = ActionType.ACTIVE;

		arCost = new int[]{0,0,0,0,0};

		nCd = 0;
        nFatigue = 2;

		sDescription = "Just wait around for a little";

	}

	override public void Execute(){

		Debug.Log ("Just resting lol");

		base.Execute ();
	}

}
