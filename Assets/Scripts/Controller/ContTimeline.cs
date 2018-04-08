using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContTimeline : Observer {

	override public void UpdateObs(string eventType, Object target, params object[] args){

		switch (eventType) {
		case Notification.ClickExecute:
			Timeline.Get ().EvaluateEvent ();

			break;
		default:

			break;
		}
	}

	public void Start(){

	}
}
