﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO:: Far in the future, do a pass for where I do 'new' allocations,
//       and see if those occurrences can be avoided with moving
//       around existing elements

//Nothing selected - ready to select a new character
public class StateTargetIdle : StateTarget {


	public override void UpdateObs(string eventType, Object target, params object[] args){

		switch (eventType) {
            case Notification.ChrStartHold:
            case Notification.ClickChr:
			contTarg.selected = ((ViewChr)target).mod;

			contTarg.SetState (new StateTargetSelected (contTarg));
			break;
		}
	}

	override public void OnEnter(){
		if (contTarg.selected != null) {
			contTarg.selected.Idle ();
		}		
		contTarg.selected = null;
	}

	public StateTargetIdle(ContTarget _contTarg): base(_contTarg){

	}

}
