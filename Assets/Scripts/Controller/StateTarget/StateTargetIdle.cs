using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO:: Far in the future, do a pass for where I do 'new' allocations,
//       and see if those occurrences can be avoided with moving
//       around existing elements

//Nothing selected - ready to target a new character
public class StateTargetIdle : StateTarget {


	public override void UpdateObs(string eventType, Object target, params object[] args){

		switch (eventType) {
		case Notification.ClickChr:
			// TODO:: Select the character so that you can see debuffs and stuff, measure distances
			contTarg.selected = ((ViewChr)target).mod;

			contTarg.SetState (new StateTargetSelected (contTarg));
			break;

		case Notification.ChrStartHold:
			// Then we've started to hold/drag this character in preparation for choosing an action
			contTarg.selected = ((ViewChr)target).mod;

			contTarg.SetState (new StateTargetChooseAction (contTarg));
			break;
		}
	}

	override public void OnEnter(){
		contTarg.selected.Idle ();
		contTarg.selected = null;
	}

	public StateTargetIdle(ContTarget _contTarg): base(_contTarg){

	}

}
