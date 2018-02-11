using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTargetSelected : StateTarget {

	override public void OnEnter(){
		//TODO:: bring up action wheel
	}

	override public void OnLeave(){
		//TODO:: remove action wheel
	}

	override public void OnClickArena(Vector3 pos){
		//requires the position that was clicked
		contTarg.selected.UnSelect();

		contTarg.selected = null;

		contTarg.SetState (new StateTargetIdle (contTarg));
	}

	override public void OnClickChr(Character chr){
		if (contTarg.selected == chr)
			return; //nothing should be done here I don't think

		//requires the selected character
		contTarg.selected.UnSelect();

		contTarg.selected = chr;
		chr.Select();

		contTarg.SetState (new StateTargetSelected (contTarg));
		//perhaps a bit silly since we're already in this state,
		// but we want to reinitiallize the action wheel
	}

	public StateTargetSelected(ContTarget _contTarg): base(_contTarg){

	}
}
