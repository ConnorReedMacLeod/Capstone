using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used for targgeting a specific character
public class StateTargetPos : StateTarget {

	TargetArgPos tarArg;

	override public void OnEnter(){

		Debug.Assert(contTarg.selected != null);
		tarArg = (TargetArgPos)contTarg.selected.arActions [contTarg.selected.nUsingAction].arArgs[contTarg.nTarCount];

	}

	override public void OnLeave(){

	}

	//TODO:: set up some general 'back' method for targetting

	override public void OnClickArena(Vector3 pos){

		if (tarArg.setTar (pos)) {

			//move to next target
			contTarg.IncTar ();

			contTarg.SetTargetArgState ();
		} else {
			
		}
	}

	override public void OnClickChr(Character chr, Vector3 pos){
		//TODO:: this was just copied from above - prolly don't do that...
		if (tarArg.setTar (pos)) {

			//move to next target
			contTarg.IncTar ();

			contTarg.SetTargetArgState ();
		} else {
			
		}


	}

	override public void OnClickAct(Character chr, int idAct){
		// shouldn't be possible since no actions should be out
		Debug.LogError("SOMEHOW CLICKED AN ACTION WHILE TRYING TO TARGET A POS");
	}

	public StateTargetPos(ContTarget _contTarg): base(_contTarg){

	}
}
