﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LocalInputHuman : LocalInputType {

    public StateTarget curState;
    public bool bCurrentlySelectingSkill;

    public static Subject subAllHumanStartSelection = new Subject(Subject.SubType.ALL);
    public static Subject subAllHumanEndSelection = new Subject(Subject.SubType.ALL);

    public override InputType GetInputType() {
        return InputType.HUMAN;
    }

    public override bool CanProceedWithSkillSelection() {

        //We can only proceed with selecting a skill if it's our turn to actually
        // be using a skill - we will have had our StartSelection method called to let us
        // know we are allowed to select skills

        //Bug - if we have swapped to a newly created humaninput while skill selection has already started, then 
        //  we don't currently initilize bCurrentlySelectingSkill to the correct value

        if(bCurrentlySelectingSkill == false) {
            Debug.Log("Error - can't select a skill when we're not currently locally asked to select targets for a skill");
            return false;
        }
        return true;
    }

    public override void StartSelection(MatchInput matchinputToFill) {

        Debug.Log(LibDebug.AddColor("Starting Input Selection for " + ContTurns.Get().chrNextReady, LibDebug.Col.RED));

        matchinputToFill.StartManualInputProcess(this);

        subAllHumanStartSelection.NotifyObs(plyrOwner);

    }

    public override void EndSelection(MatchInput matchinputFilled) {

        Debug.Log(LibDebug.AddColor("Finished Input Selection for " + ContTurns.Get().chrNextReady, LibDebug.Col.RED));

        matchinputFilled.EndManualInputProcess(this);

        ContTurns.Get().chrNextReady.subEndsActiveForHumans.NotifyObs();
        subAllHumanEndSelection.NotifyObs(plyrOwner);

    }


    public LocalInputHuman() {

    }
}
