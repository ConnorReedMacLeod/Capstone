using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecTurnExecuteAction : Executable {


    public override void Execute() {
        Chr chrNextToAct = ContTurns.Get().GetNextActingChr();

        if (chrNextToAct == null) {
            //If no more characters are set to act this turn
            ContTurns.Get().SetTurnState(ContTurns.STATETURN.TURNEND);

            fDelay = 1.0f;
            sLabel = "All Characters Have Finished Acting";
        } else {
            //Then at least one character still has to go

            if (chrNextToAct.nUsingAction != -1) {
                sLabel = chrNextToAct.sName + " is using " + chrNextToAct.arActions[chrNextToAct.nUsingAction].sName;
            } else {
                sLabel = chrNextToAct.sName + " has no valid action prepped";
            }

            chrNextToAct.ExecuteAction();

            fDelay = 2.0f;
        }

        base.Execute();
    }
}
