using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used for targgeting a specific character
public class StateTargetPos : StateTarget {

	TargetArgPos tarArg;

	public override void UpdateObs(string eventType, Object target, params object[] args){

		switch (eventType) {
		case Notification.ClickChr: // If you click on a character, then you still want the location under them
		case Notification.ClickArena:
			// Try targetting under the mouse
			if (tarArg.setTar (LibView.GetMouseLocation ())) {
				// If the target location is acceptable

				//move to next target
				contTarg.IncTar ();

				contTarg.SetTargetArgState ();
			} else {
				// The location is not valid
				Debug.Log("You can't target that position");
			}
			break;
		}
	}

	override public void OnEnter(){

		Debug.Assert(contTarg.selected != null);
		tarArg = (TargetArgPos)contTarg.selected.arActions [contTarg.selected.nUsingAction].arArgs[contTarg.nTarCount];

	}

	override public void OnLeave(){

	}

	//TODO:: set up some general 'back' method for targetting

	public StateTargetPos(ContTarget _contTarg): base(_contTarg){

	}
}
