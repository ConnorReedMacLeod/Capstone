using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecTurnExecuteAction : Executable {


    public override void Execute() {

        //We assume that we have just come from choosing an action, so get that character
        Chr chrNextToAct = ContTurns.Get().GetNextActingChr();

        ContTarget.Get().CancelTar(); // If you're in the middle of targetting an ability, cancel that targetting
        chrNextToAct.LockTargetting(); // Lock that character so they can't change ability selection

        if (chrNextToAct.bSetAction == true) {
            sLabel = chrNextToAct.sName + " is using " + chrNextToAct.arActions[chrNextToAct.nUsingAction].sName;
            chrNextToAct.ExecuteAction();

        } else {
            sLabel = chrNextToAct.sName + " has not set an Action";

            if (chrNextToAct.nQueuedFatigue == 0) {
                //If the character has not used any abilities that would increase their fatigue
                //(cantrips with +0 fatigue), then we set the character to use a rest action

                chrNextToAct.SetRestAction();
                chrNextToAct.ExecuteAction();
            }

            // Finished any cleanup we need to do for a character ending their ability selection
            chrNextToAct.FinishSelectionPhase();

        }

        fDelay = 2.0f;

        if(ContTurns.Get().GetNextActingChr() == null) {
            // Then there are no more characters that need to go after us this turn
            ContTurns.Get().SetTurnState(ContTurns.STATETURN.TURNEND);
        } else {
            // Otherwise, we have a character we need to select actions for
            ContTurns.Get().SetTurnState(ContTurns.STATETURN.CHOOSEACTIONS);
        }

        base.Execute();
    }
}
