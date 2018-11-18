using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecTurnChooseActions : Executable {

    public override void Execute() {

        //Ensure only the currently acting character can select actions
        ContTurns.Get().GetNextActingChr().UnlockTargetting();

        ContTurns.Get().SetTurnState(ContTurns.STATETURN.EXECUTEACTIONS);

        sLabel = "Select Your Action for " + ContTurns.Get().GetNextActingChr().sName;
        fDelay = ContTurns.Get().GetTimeForActing();

        base.Execute();
    }
}
