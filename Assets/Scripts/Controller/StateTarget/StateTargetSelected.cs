using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTargetSelected : StateTarget {


	public override void UpdateObs(string eventType, Object target, params object[] args){

		switch (eventType) {
		case Notification.ArenaStartDrag:
		case Notification.ClickArena:
            // If we're clicking at all with the arena, then we can deselect our character
            Debug.Log("Clicked Arena, so stopping selection");

             contTarg.SetState (new StateTargetIdle (contTarg));

			break;

		case Notification.ClickChr:
             Debug.Log("Switching to select a new character instead");

            // If we now click on a different character, then we'll select them instead
            contTarg.selected.Idle (); // Need to deselect our current character first
			contTarg.selected = ((ViewChr)target).mod;

			contTarg.SetState (new StateTargetSelected (contTarg));

			break;

		case Notification.ClickAct:
            // When we've clicked an action, use that action

            contTarg.selected.Targetting();
            contTarg.selected.nUsingAction = ((ViewAction)target).id;

            // TODO:: Save the current targets if there are any, so that you can 
            // revert to those targets if you've failed targetting
            contTarg.ResetTar();
            contTarg.SetTargetArgState(); // Let the parent figure out what exact state we go to
            break;
        }
	}


	override public void OnEnter(){
		
		Debug.Assert(contTarg.selected != null);
		contTarg.selected.Select ();
        

	}

	override public void OnLeave(){

	}


	public StateTargetSelected(ContTarget _contTarg): base(_contTarg){

	}
}
