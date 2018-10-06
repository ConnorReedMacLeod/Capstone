using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTargetChooseAction : StateTarget {

	public override void UpdateObs(string eventType, Object target, params object[] args){

		switch (eventType) {
		case Notification.ClickAct:
			// When we've clicked an action, use that action
            
			contTarg.selected.Targetting ();
			contTarg.selected.nUsingAction = ((ViewAction)args[0]).id;

			// TODO:: Save the current targets if there are any, so that you can 
			// revert to those targets if you've failed targetting
			contTarg.ResetTar ();
			contTarg.SetTargetArgState (); // Let the parent figure out what exact state we go to
			break;
		
		case Notification.ClickArena:
			// Then we've just clicked somewhere in the arena, so we can deselect the current Char

			contTarg.SetState (new StateTargetIdle (contTarg));

			break;

		case Notification.ClickChr:
			// Then we've clicked on a different character, so select them

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
