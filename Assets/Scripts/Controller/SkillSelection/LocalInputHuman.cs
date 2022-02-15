using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LocalInputHuman : LocalInputType {

    public StateTarget curState;

    public static Subject subAllHumanStartSelection = new Subject(Subject.SubType.ALL);
    public static Subject subAllHumanEndSelection = new Subject(Subject.SubType.ALL);

    public override bool CanProceedWithSkillSelection() {

        //We can only proceed with selecting a skill if it's our turn to actually
        // be using a skill - we will have had our StartSelection method called to let us
        // know we are allowed to select skills

        if(bCurrentlySelectingSkill == false) {
            Debug.Log("Error - can't select a skill to be used when it's not the local player's turn");
            return false;
        }
        return true;
    }

    public override void StartSelection() {
        base.StartSelection();

        Debug.Log(LibDebug.AddColor("Starting Input Selection for " + ContTurns.Get().chrNextReady, LibDebug.Col.RED));

        ContTurns.Get().chrNextReady.subBecomesActiveForHumans.NotifyObs();
        subAllHumanStartSelection.NotifyObs(this);

    }

    public override void EndSelection() {
        base.EndSelection();

        Debug.Log(LibDebug.AddColor("Finished Input Selection for " + ContTurns.Get().chrNextReady, LibDebug.Col.RED));

        ContTurns.Get().chrNextReady.subEndsActiveForHumans.NotifyObs();
        subAllHumanEndSelection.NotifyObs(this);

    }


    public LocalInputHuman() {

    }
}
