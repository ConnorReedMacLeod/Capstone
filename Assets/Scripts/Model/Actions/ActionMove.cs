using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionMove : Action {



	public ActionMove(Chr _chrOwner): base(1, _chrOwner){//number of target arguments

		//Since the base constructor initializes this array, we can start filling it
		arArgs [0] = new TargetArgPos ((own, pos) => true);// TODO:: Make a standard "can't move inside other people or outside of map" function

		arCost = new int[]{ 0, 0, 0, 0, 0 };

		nCd = 3;
		nRecharge = 3;

		SetArgOwners ();
	}

	override public void Execute(){
		//It's a bit awkward that you have to do this typecasting, 
		// but at least it's eliminated from the targetting lambda
		Vector3 tar = ((TargetArgPos)arArgs [0]).v3Tar;

		chrOwner.SetPosition (new Vector3(tar.x, tar.y, chrOwner.v3Pos.z));

		//NOTE:: Every Execute extension should begin with a typecast and end with a base.Execute call;


		base.Execute ();
	}

}
