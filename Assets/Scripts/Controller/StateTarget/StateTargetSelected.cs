using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTargetSelected : StateTarget {

	override public void OnEnter(){
		
		Debug.Assert(contTarg.selected != null);
		contTarg.selected.Select ();

	}

	override public void OnLeave(){

	}

	override public void OnClickArena(Vector3 pos){
		//requires the position that was clicked
		contTarg.selected.Deselect();

		contTarg.selected = null;

		contTarg.SetState (new StateTargetIdle (contTarg));
	}

	override public void OnClickChr(Character chr, Vector3 pos){
		if (contTarg.selected == chr)
			return; //nothing should be done here I don't think

		//change the selected character
		contTarg.selected.Deselect();

		contTarg.selected = chr;
		chr.Select();

		contTarg.SetState (new StateTargetSelected (contTarg));
		//perhaps a bit silly since we're already in this state,
		// but we want to reinitiallize the action wheel
	}

	override public void OnClickAct(Character chr, int idAct){
		Debug.Log (chr + " is using action " + idAct);

		contTarg.selected.Targetting ();
		contTarg.selected.nUsingAction = idAct;

		contTarg.ResetTar ();
		contTarg.SetTargetArgState ();

	}

	public StateTargetSelected(ContTarget _contTarg): base(_contTarg){

	}
}
