using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO:: Far in the future, do a pass for where I do 'new' allocations,
//       and see if those occurrences can be avoided with moving
//       around existing elements

//Nothing selected - ready to select a new character
public class StateTargetIdle : StateTarget {

    public void cbClickChar(Object target, params object[] args) {

        inputHuman.selected = ((ViewChr)target).mod;

        inputHuman.SetState(new StateTargetSelected(inputHuman));
    }

	override public void OnEnter(){
		if (inputHuman.selected != null) {
			inputHuman.selected.Idle ();
		}		
		inputHuman.selected = null;
        ViewChr.subAllClick.Subscribe(cbClickChar);
    }

    public override void OnLeave() {
        ViewChr.subAllClick.UnSubscribe(cbClickChar);
    }

    public StateTargetIdle(InputHuman _inputHuman) : base(_inputHuman) {

	}

}
