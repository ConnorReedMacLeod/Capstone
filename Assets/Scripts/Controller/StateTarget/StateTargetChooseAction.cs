using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTargetChooseAction : StateTarget {

	// Note: The only way to be in this state is if the mouse is currently held down
	//       So we only need to handle ways in which the mouse can be released
	public override void UpdateObs(string eventType, Object target, params object[] args){

		switch (eventType) {
		case Notification.ReleaseChrOverAct:
			// Then we've dragged from a Chr to an Action - start targetting with that action

			Debug.Assert (((ViewChr)target).mod == contTarg.selected);
			contTarg.selected.Targetting ();
			contTarg.selected.nUsingAction = ((ViewAction)args[0]).id;

			// TODO:: Save the current targets if there are any, so that you can 
			// revert to those targets if you've failed targetting
			contTarg.ResetTar ();
			contTarg.SetTargetArgState (); // Let the parent figure out what exact state we go to

			break;
		
		case Notification.ReleaseChrOverNone:
			// Then we've dragging from a Chr and ended on something that wasn't ourselves, or an action
			Debug.Assert (((ViewChr)target).mod == contTarg.selected);

			contTarg.SetState (new StateTargetIdle (contTarg));

			break;

		case Notification.ChrStopHold:
			// Then we've just dragged, but released over the original character - select them

			contTarg.selected = ((ViewChr)target).mod;

			contTarg.SetState (new StateTargetSelected (contTarg));

			break;
		}
	}
		
	override public void OnEnter(){
		contTarg.selected.ChoosingAction ();
	}

	public StateTargetChooseAction(ContTarget _contTarg): base(_contTarg){

	}
}
