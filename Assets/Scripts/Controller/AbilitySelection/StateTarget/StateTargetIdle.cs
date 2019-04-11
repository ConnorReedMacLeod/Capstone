using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO:: Far in the future, do a pass for where I do 'new' allocations,
//       and see if those occurrences can be avoided with moving
//       around existing elements

//Nothing selected - ready to select a new character
public class StateTargetIdle : StateTarget {

    public void cbClickChar(Object target, params object[] args) {

        if(InputHuman.nHumanCount == 2){

            if (ContTurns.Get().GetNextActingChr() == null) {
                //If there's two humans, and no character is acting next, then only allow selection by the input of the player
                //who owns this character
                if (inputHuman.plyrOwner.id != ((ViewChr)target).mod.plyrOwner.id) return;

            } else if (inputHuman.plyrOwner.id != ((ViewChr)target).mod.plyrOwner.id) {
                //If there are two players, then only allow selection by the player whose turn it is
                if (inputHuman.plyrOwner.id != ContTurns.Get().GetNextActingChr().plyrOwner.id) return;
            }
        }

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
