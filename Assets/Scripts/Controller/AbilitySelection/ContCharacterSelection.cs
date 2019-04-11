﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ContCharacterSelection : MonoBehaviour {

    private static ContCharacterSelection inst;
    public static ContCharacterSelection Get() {
        return inst;
    }

    public StateTarget curState;

    public Chr chrSelected;

    public int nSelectedAbility;
    public int[] arTargetIndices;

    public int indexCurTarget;

    public static Subject subAllStartTargetting = new Subject();
    public static Subject subAllFinishTargetting = new Subject();

    private void Awake() {
        if (inst != null && inst != this) {
            Destroy(this.gameObject);
        } else {
            inst = this;
        }
    }

    // Start a new round of targetting
    public void ResetTar() {
        indexCurTarget = 0;
        //Clear any previous targetting information we had
        nSelectedAbility = -1;
        arTargetIndices = null;

    }

    public void StoreTargettingIndex(int ind) {
        //We assume the passed in index would be a legal target

        //Save a copy of the submitted targetting index
        arTargetIndices[indexCurTarget] = ind;

        //Then advance to look for the next target
        indexCurTarget++;


        //Now figure out and move to the next state required for the next target
        SetTargetArgState();
    }

    // Ends targetting
    public void CancelTar() {

        if (curState.GetType() == typeof(StateTargetIdle) || curState.GetType() == typeof(StateTargetSelected)) {
            // If we're waiting to select a character, or aren't in the process of targetting
            // an ability with the selected character, then no resetting is needed
            return;
        }

        ResetTar();

        SetState(new StateTargetIdle());

        //Let everything know that targetting has ended
        subAllFinishTargetting.NotifyObs(this);
    }

    // Create the necessary state for selecting the needed type
    public void SetTargetArgState() {
        //Before this is called, assume that IncTar/DecTar/ResetTar has been appropriately called

        if (indexCurTarget < 0) {
            //Then we've cancelled the targetting action so go back to... idle?
            CancelTar();

        } else if (indexCurTarget == 0) {
            //Then create the targetting array with the correct size
            arTargetIndices = new int[chrSelected.arActions[nSelectedAbility].nArgs];

            //Then we should let things know that a new targetting has begun
            subAllStartTargetting.NotifyObs(chrSelected, nSelectedAbility);
        }

        if (indexCurTarget == chrSelected.arActions[nSelectedAbility].nArgs) {
            //Then we've filled of the targetting arguments

            // Can now go back idle and wait for the next targetting
            SetState(new StateTargetIdle());
            //Debug.Log ("Targetting finished");

            //Submit our targetting selections to the InputAbilitySelection controller
            ContAbilitySelection.Get().SubmitAbility(chrSelected.globalid, nSelectedAbility, arTargetIndices);

            ResetTar();

            //Let everything know that targetting has ended
            subAllFinishTargetting.NotifyObs(this);
        } else {



            // Get the type of the target arg that we need to handle
            string sArgType = chrSelected.arActions[nSelectedAbility].arArgs[indexCurTarget].GetType().ToString();

            StateTarget newState;

            switch (sArgType) { //TODO:: Maybe make this not rely on a string comparison... bleh
                case "TargetArgChr":
                    newState = new StateTargetChr();
                    break;

                case "TargetArgTeam":
                    newState = new StateTargetTeam();
                    break;

                case "TargetArgAlly":
                    newState = new StateTargetChr();
                    break;

                default:

                    Debug.LogError(sArgType + " is not a recognized ArgType!");
                    return;
            }

            // Transition to the appropriate state
            SetState(newState);

        }
    }

    public void SetState(StateTarget newState) {

        if (curState != null) {
            //Debug.Log("Leaving State " + curState.ToString());
            curState.OnLeave();
        }

        curState = newState;

        if (curState != null) {
            curState.OnEnter();
            //Debug.Log("Entering State " + curState.ToString());
        }
    }

    public void Start() {
        Debug.Log("Starting an InputHuman");
        SetState(new StateTargetIdle());
    }

    public ContCharacterSelection() {

        indexCurTarget = 0;
    }
}
