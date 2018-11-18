using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecTurnExecuteAction : Executable {


    public override void Execute() {

        //We assume that we have just come from choosing an action, so get that character
        Chr chrNextToAct = ContTurns.Get().GetNextActingChr();

        ContTarget.Get().CancelTar(); // If you're in the middle of targetting an ability, cancel that targetting
        chrNextToAct.LockTargetting(); // Lock that character so they can't change ability selection


        if (chrNextToAct.nUsingAction != -1) {
            sLabel = chrNextToAct.sName + " is using " + chrNextToAct.arActions[chrNextToAct.nUsingAction].sName;
        } else {
            sLabel = chrNextToAct.sName + " has no valid action prepped";
        }

        chrNextToAct.ExecuteAction();
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
