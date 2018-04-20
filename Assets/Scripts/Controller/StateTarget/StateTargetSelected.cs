using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTargetSelected : StateTarget {


	public override void UpdateObs(string eventType, Object target, params object[] args){

		switch (eventType) {
		case Notification.ArenaStartDrag:
		case Notification.ClickArena:
			// If we're clicking at all with the arena, then we can deselect our character

			contTarg.SetState (new StateTargetIdle (contTarg));

			break;

		case Notification.ClickChr:
			// If we now click on a different character, then we'll select them instead
			contTarg.selected.Idle (); // Need to deselect our current character first
			contTarg.selected = ((ViewChr)target).mod;

			contTarg.SetState (new StateTargetSelected (contTarg));

			break;
		}
	}


	override public void OnEnter(){
		
		Debug.Assert(contTarg.selected != null);
		contTarg.selected.Select ();

	}


	public StateTargetSelected(ContTarget _contTarg): base(_contTarg){

	}
}
