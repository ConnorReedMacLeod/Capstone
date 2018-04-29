﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used for targgeting a specific character
public class StateTargetChr : StateTarget {

	TargetArgChr tarArg;


	public override void UpdateObs(string eventType, Object target, params object[] args){

		switch (eventType) {
		case Notification.ClickArena:
			StopTargetting ();

			break;

		case Notification.ChrStopHold:
		case Notification.ClickChr:
			if (tarArg.setTar (((ViewChr)target).mod)) {
				Debug.Log ("Target successfully set to " + ((ViewChr)target).mod.sName);

				//move to next target
				contTarg.IncTar ();

				contTarg.SetTargetArgState ();
			} else {
				//BUG:: When releasing intially to select a characters ability, it tries to target the abililty immediately
				// Cause: when you release over an action it passes a actionrelease and characterrelease notification
				Debug.Log (((ViewChr)target).mod + " is not a valid character target");
			}
			break;
		case Notification.GlobalRightUp:
			contTarg.CancelTar ();
			break;
		}

	}

	override public void OnEnter(){

		Debug.Assert(contTarg.selected != null);
		tarArg = (TargetArgChr)contTarg.selected.arActions [contTarg.selected.nUsingAction].arArgs[contTarg.nTarCount];

	}

	override public void OnLeave(){
	}

	public void StopTargetting(){
		//clear any targetting 
		//TODO:: maybe only reset the targets to whatever was selected before?
		contTarg.selected.arActions [contTarg.selected.nUsingAction].Reset ();

		contTarg.SetState (new StateTargetIdle (contTarg));
	}

		
	public StateTargetChr(ContTarget _contTarg): base(_contTarg){

	}
}
