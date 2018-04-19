using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used for targgeting a specific character
public class StateTargetChr : StateTarget {

	TargetArgChr tarArg;

	override public void OnEnter(){

		Debug.Assert(contTarg.selected != null);
		tarArg = (TargetArgChr)contTarg.selected.arActions [contTarg.selected.nUsingAction].arArgs[contTarg.nTarCount];

	}

	override public void OnLeave(){

	}

	override public void OnClickArena(Vector3 pos){

		//clear any targetting 
		//TODO:: maybe only reset the targets to whatever was selected before?
		contTarg.selected.arActions [contTarg.selected.nUsingAction].Reset ();

		contTarg.selected.Deselect();

		contTarg.selected = null;

		contTarg.SetState (new StateTargetIdle (contTarg));
	}

	override public void OnClickChr(Chr chr, Vector3 pos){

		if (tarArg.setTar (chr)) {
			Debug.Log ("Target successfully set to " + chr);

			//move to next target
			contTarg.IncTar ();

			contTarg.SetTargetArgState ();
		} else {
			Debug.Log (chr + " is not a valid target");
		}


	}

	override public void OnClickAct(Action act){
		// shouldn't be possible since no actions should be out
		Debug.LogError("SOMEHOW CLICKED AN ACTION WHILE TRYING TO TARGET A POS");
	}
		
	public StateTargetChr(ContTarget _contTarg): base(_contTarg){

	}
}
