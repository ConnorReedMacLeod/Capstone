using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:: Make a static instance of this

public class ContTarget : MonoBehaviour {

	public StateTarget curState;

	public Chr selected;

	public int nTarCount;

    public bool bLocked;

    public static ContTarget instance;

    public static Subject subAllStartTargetting = new Subject();
    public static Subject subAllFinishTargetting = new Subject();

    //TODO CHANGE ALL .Get() calls in other classes to use properties
    //     so the syntax isn't as gross

    public static ContTarget Get() {
        if (instance == null) {
            GameObject go = GameObject.FindGameObjectWithTag("ContTarget");
            if (go == null) {
                Debug.LogError("ERROR! NO OBJECT HAS A ContTarget TAG!");
            }
            instance = go.GetComponent<ContTarget>();
            if (instance == null) {
                Debug.LogError("ERROR! ContTurns TAGGED OBJECT DOES NOT HAVE A ContTarget COMPONENT!");
            }
            instance.Start();
        }
        return instance;
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

    // Stop the player from targetting any abilities during a turn
    public void LockTargetting() {

        CancelTar();
        bLocked = true;
    }

    // Allow the player to start targetting abilities again
    public void UnlockTargetting() {

        bLocked = false;

    }

	// Ends targetting
	public void CancelTar(){
        //TODO:: Consider if resetting like this needs to back through the previously selected
        //       targets and clean them out for the future.

        if(curState.GetType() == typeof(StateTargetIdle) || curState.GetType() == typeof(StateTargetSelected)) {
            // If we're waiting to select a character, or aren't in the process of targetting
            // an ability with the selected character, then no resetting is needed
            return;
        }

		ResetTar();
		selected.bSetAction = false;
		selected.nUsingAction = -1;

		SetState (new StateTargetIdle (this));

		//Let everything know that targetting has ended
		subAllFinishTargetting.NotifyObs(this);
	}

	// Create the necessary state for selecting the needed type
	public void SetTargetArgState(){
		//Before this is called, assume that IncTar/DecTar/ResetTar has been appropriately called

		if (nTarCount < 0) {
			//Then we've cancelled the targetting action so go back to... idle?
			CancelTar();

		} else if (nTarCount == selected.arActions [selected.nUsingAction].nArgs) {
			//Then we've filled of the targetting arguments

			selected.bSetAction = true;

			// Can now go back idle and wait for the next targetting
			SetState (new StateTargetIdle (this));
			Debug.Log ("Targetting finished");

            //Let everything know that targetting has ended
            subAllFinishTargetting.NotifyObs(this);
		} else {

			if (nTarCount == 0) {
                //Then we should let things know that a new targetting has begun
                subAllStartTargetting.NotifyObs(selected, selected.nUsingAction);
			}

			// Get the type of the target arg that we need to handle
			string sArgType = selected.arActions[selected.nUsingAction].arArgs[nTarCount].GetType().ToString();

			StateTarget newState;

			switch (sArgType) { //TODO:: Maybe make this not rely on a string comparison... bleh
			case "TargetArgChr":
				newState = new StateTargetChr (this);
				break;

            case "TargetArgTeam":
                newState = new StateTargetTeam(this);
                break;

            case "TargetArgAlly":
                newState = new StateTargetChr(this);
                break;

			default:

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

    public void Start() {
        SetState(new StateTargetIdle(this));
    }

    public ContTarget(){
		
		nTarCount = 0;
	}
}
