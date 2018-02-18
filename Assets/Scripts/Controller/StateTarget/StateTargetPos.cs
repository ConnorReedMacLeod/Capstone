using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used for targgeting a specific character
public class StateTargetPos : StateTarget {

	TargetArgChr tarArg;

	override public void OnEnter(){

		Debug.Assert(contTarg.selected != null);
		tarArg = (TargetArgChr)contTarg.selected.arActions [contTarg.selected.nUsingAction].arArgs[contTarg.nTarCount];

	}

	override public void OnLeave(){

	}

	//TODO:: set up some general 'back' method for targetting

	override public void OnClickArena(Vector3 pos){

		Debug.Log (pos + " was selected");

		/*
		//clear any targetting 
		//TODO:: maybe only reset the targets to whatever was selected before?
		contTarg.selected.arActions [contTarg.selected.nUsingAction].Reset ();

		contTarg.selected.Deselect();

		contTarg.selected = null;

		contTarg.SetState (new StateTargetIdle (contTarg));*/
	}

	override public void OnClickChr(Character chr){

		if (tarArg.setTar (chr)) {
			Debug.Log ("Target successfully set to " + chr);

			//move to next target
			contTarg.IncTar ();

			contTarg.SetTargetArgState ();
		} else {
			Debug.Log (chr + " is not a valid target");
		}


	}

	override public void OnClickAct(Character chr, int idAct){
		// shouldn't be possible since no actions should be out
		Debug.LogError("SOMEHOW CLICKED AN ACTION WHILE TRYING TO TARGET A CHR");
	}

	public StateTargetPos(ContTarget _contTarg): base(_contTarg){

	}
}
