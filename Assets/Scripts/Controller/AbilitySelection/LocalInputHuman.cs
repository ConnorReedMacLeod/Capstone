using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LocalInputHuman : LocalInputType {

    public StateTarget curState;

    public static Subject subAllHumanStartSelection = new Subject(Subject.SubType.ALL);
    public static Subject subAllHumanEndSelection = new Subject(Subject.SubType.ALL);

    public override bool CanProceedWithSkillSelection() {

        //We can only proceed with slecting an ability if it's our turn to actually
        // be using an ability

        if(bCurrentlySelectingSkill == false) {
            Debug.Log("Error - can't select a skill to be used when it's not the local player's turn");
            return false;
        }
        return true;
    }

    public override void StartSelection() {
        base.StartSelection();

        ContTurns.Get().chrNextReady.subBecomesActiveForHumans.NotifyObs();
        subAllHumanStartSelection.NotifyObs(this);

    }

    public override void EndSelection() {
        base.EndSelection();

        ContTurns.Get().chrNextReady.subEndsActiveForHumans.NotifyObs();
        subAllHumanEndSelection.NotifyObs(this);

    }


    public LocalInputHuman() {

    }
}
