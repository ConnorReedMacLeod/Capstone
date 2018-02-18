﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContTarget : Controller {

	public StateTarget curState;

	public Character selected;

	public int nTarCount;

	//TODO:: Set up helper methods to make retrieving the current action and current target arg easy
	//       Need to coinsider what happens if they don't exist though

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

	// Move to selecting the next target
	public void IncTar(){
		nTarCount++;
	}

	// Move to selecting the previous target
	public void DecTar(){
		nTarCount--;
	}

	// Start a new round of targetting
	public void ResetTar(){
		nTarCount = 0;
	}

	// Create the necessary state for selecting the needed type
	public void SetTargetArgState(){
		//Before this is called, assume that IncTar/DecTar/ResetTar has been appropriately called

		if (nTarCount < 0) {
			//Then we've cancelled the targetting action so go back to... idle?
			selected.nUsingAction = -1;
			selected.Deselect ();

			SetState (new StateTargetIdle (this));
		} else if (nTarCount == selected.arActions [selected.nUsingAction].nArgs) {
			//Then we've filled of the targetting arguments

			// TODO: Let the timeline know that the action is filled
			selected.Deselect ();

			Debug.Log (selected.sName + " just finished targetting their ability");

			// Can now go back idle and wait for the next targetting
			SetState (new StateTargetIdle (this));
		} else {

			// Get the type of the target arg that we need to handle
			string sArgType = selected.arActions[selected.nUsingAction].arArgs[nTarCount].GetType().ToString();

			StateTarget newState;

			switch (sArgType) {
			case "TargetArgChr":
				newState = new StateTargetChr (this);
				break;

			default:
				newState = new StateTarget(this);
				Debug.LogError(sArgType + " is not a recognized ArgType!");
				return;
			}

			// Transition to the appropriate state
			SetState(newState);

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
		nTarCount = 0;
	}
}
