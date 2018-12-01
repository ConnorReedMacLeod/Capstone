using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRest : Action {



	public ActionRest(Chr _chrOwner): base(0, _chrOwner){//number of target arguments

		sName = "Rest";
		type = ActionType.ACTIVE;

        chrSource = _chrOwner;

		arCost = new int[]{0,0,0,0,0};

		nCd = 0;
        nFatigue = 0;

		sDescription = _chrOwner + " has finished selecting abilities for the turn";

	}

	override public void Execute(){

		Debug.Log ("Just resting lol");

		base.Execute ();
	}

}
