using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO:: Far in the future, do a pass for where I do 'new' allocations,
//       and see if those occurrences can be avoided with moving
//       around existing elements

//Nothing selected - ready to select a new character
public class StateTargetIdle : StateTarget {

    public void cbClickChar(Object target, params object[] args) {

        ContCharacterSelection.Get().chrSelected = ((ViewChr)target).mod;

        ContCharacterSelection.Get().SetState(new StateTargetSelected());
    }

	override public void OnEnter(){
		if (ContCharacterSelection.Get().chrSelected != null) {
            ContCharacterSelection.Get().chrSelected.Idle();
		}
        ContCharacterSelection.Get().chrSelected = null;

        ViewChr.subAllClick.Subscribe(cbClickChar);
    }

    public override void OnLeave() {
        ViewChr.subAllClick.UnSubscribe(cbClickChar);
    }


}
