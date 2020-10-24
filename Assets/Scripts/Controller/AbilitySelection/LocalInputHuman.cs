using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LocalInputHuman : LocalInputType {

    public StateTarget curState;

    public static Subject subAllHumanStartSelection = new Subject(Subject.SubType.ALL);
    public static Subject subAllHumanEndSelection = new Subject(Subject.SubType.ALL);

    public override bool CanProceedWithSkillSelection(Chr chrSelected) {
        //We're only okay with skill selections for characters owned by the local player
        if(chrSelected.plyrOwner.id != plyrOwner.id) {
            Debug.Log("Error - can't select skills for characters the local player doesn't control");
            return false;
        }
        if(bCurrentlySelectingSkill == false) {
            Debug.Log("Error - can't select a skill to be used when it's not the local player's turn");
            return false;
        }
        return true;
    }

    public override void StartSelection() {
        base.StartSelection();

        //TODO - hookup character highlighting to this
        subAllHumanStartSelection.NotifyObs(this);

    }

    public override void EndSelection() {
        base.EndSelection();

        //TODO - should close any mid-selection UI for targetting skills
        subAllHumanEndSelection.NotifyObs(this);

    }


    public LocalInputHuman() {

    }
}
