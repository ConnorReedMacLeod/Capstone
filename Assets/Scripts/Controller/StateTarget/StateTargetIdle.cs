using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO:: Far in the future, do a pass for where I do new,
//       and see if those occurrences can be avoided with moving
//       around existing elements

//Nothing selected - ready to target a new character
public class StateTargetIdle : StateTarget {

	override public void OnEnter(){
		contTarg.selected = null;
	}

	override public void OnClickChr(Character chr, Vector3 pos){
		//requires the selected character
		contTarg.selected = chr;

		contTarg.SetState (new StateTargetSelected (contTarg));
	}

	public StateTargetIdle(ContTarget _contTarg): base(_contTarg){

	}

}
