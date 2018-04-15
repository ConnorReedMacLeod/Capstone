using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionFireball : Action {



	public ActionFireball(Chr _chrOwner): base(1, _chrOwner){//number of target arguments

		//Since the base constructor initializes this array, we can start filling it
		arArgs [0] = new TargetArgChr ((own, tar) => own.plyrOwner != tar.plyrOwner);

		sName = "Fireball";
		type = ActionType.ACTIVE;

		arCost = new int[]{2,1,1,4,3};

		nCd = 6;
		nRecharge = 4;

		sDescription = "Throw a fireball at a target";

		SetArgOwners ();
	}

	override public void Execute(){
		//It's a bit awkward that you have to do this typecasting, 
		// but at least it's eliminated from the targetting lambda
		Chr tar = ((TargetArgChr)arArgs [0]).chrTar;

		Debug.Log (tar + " has been targetted");

		//NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;


		base.Execute ();
	}

}
