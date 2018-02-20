using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionFireball : Action {



	public ActionFireball(Character _chrOwner): base(1, _chrOwner){//number of target arguments

		//Since the base constructor initializes this array, we can start filling it
		arArgs [0] = new TargetArgChr ((own, tar) => own.plyrOwner != tar.plyrOwner);

		arCost = new int[]{0,0,1,0,0};

		nCd = 6;
		nRecharge = 4;

		SetArgOwners ();
	}

	override public void Execute(){
		//It's a bit awkward that you have to do this typecasting, 
		// but at least it's eliminated from the targetting lambda
		Character tar = ((TargetArgChr)arArgs [0]).chrTar;

		Debug.Log (tar + " has been targetted");

		//NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;


		base.Execute ();
	}

}
