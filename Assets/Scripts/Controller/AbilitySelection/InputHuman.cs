using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputHuman : InputAbilitySelection {

	public StateTarget curState;

    public static Subject subAllStartTargetting = new Subject();
    public static Subject subAllFinishTargetting = new Subject();

    public static Subject subAllHumanStartSelection = new Subject();
    public static Subject subAllHumanEndSelection = new Subject();

    public override void StartSelection() {

        ResetTar();

        //TODO - hookup character highlighting to this
        subAllHumanStartSelection.NotifyObs(this);

    }

    public override void GaveInvalidTarget() {
        //If we somehow gave an invalid target, make an error message, then reset our targetting
        Debug.Log("The human-input for player " + plyrOwner.id + " gave an invalid target");

        ResetTar();
    }

    public void ResetTar(){

        //Clear any previous targetting information we had
        nSelectedAbility = -1;
        arTargetIndices = null;

    }


    public InputHuman(){
        ResetTar();
	}
}
