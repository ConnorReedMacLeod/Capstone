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

	public void StopTargetting(){
		//clear any targetting 
		//TODO:: maybe only reset the targets to whatever was selected before?
		contTarg.selected.arActions [contTarg.selected.nUsingAction].Reset ();

		contTarg.selected.Deselect();

		contTarg.selected = null;

		contTarg.SetState (new StateTargetIdle (contTarg));
	}

	override public void OnReleaseChrOverNone(){

		StopTargetting ();

	}

	override public void OnClickArena(Vector3 pos){
	
		StopTargetting ();
	
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
		
	public StateTargetChr(ContTarget _contTarg): base(_contTarg){

	}
}
