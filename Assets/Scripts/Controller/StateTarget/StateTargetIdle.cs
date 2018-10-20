using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO:: Far in the future, do a pass for where I do 'new' allocations,
//       and see if those occurrences can be avoided with moving
//       around existing elements

//Nothing selected - ready to select a new character
public class StateTargetIdle : StateTarget {

    public void cbClickChar(Object target, params object[] args) {
        contTarg.selected = ((ViewChr)target).mod;

        contTarg.SetState(new StateTargetSelected(contTarg));
    }

	override public void OnEnter(){
        Debug.Log("Entering Idle");
		if (contTarg.selected != null) {
			contTarg.selected.Idle ();
		}		
		contTarg.selected = null;
        ViewChr.subAllClick.Subscribe(cbClickChar);
    }

    public override void OnLeave() {
        ViewChr.subAllClick.UnSubscribe(cbClickChar);
    }

    public StateTargetIdle(ContTarget _contTarg): base(_contTarg){

	}

}
