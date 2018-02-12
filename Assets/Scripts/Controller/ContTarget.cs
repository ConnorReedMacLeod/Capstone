using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContTarget : Controller {

	public StateTarget curState;

	public Character selected;

	override public void OnNotification(string eventType, Object target, params object[] args){

		if (curState == null) {
			Debug.LogError ("CONTTARGET HAS NO STATE");
			Debug.Assert (false);
		}

		switch (eventType) {
		case Notification.ClickAct:
			curState.OnClickAct(((ViewAction)target).mod, (int)args[0]);
			break;
		case Notification.ClickArena:
			Vector3 clickPos = (Vector3)args [0];
			curState.OnClickArena (clickPos);
			break;

		case Notification.ClickChr:
			curState.OnClickChr (((ViewChr)target).mod);
			break;

		default:
			//Assume this notification isn't relevent for this controller
			break;
		}

	}

	public void SetState (StateTarget newState){
		if (curState != null) {
			curState.OnLeave ();
		}

		curState = newState;

		if (curState != null) {
			curState.OnEnter ();
		}
	}

	public ContTarget(){
		SetState( new StateTargetIdle (this));
	}
}
