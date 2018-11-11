using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecTurnChooseActions : Executable {

    public override void Execute() {

        ContTurns.Get().SetTurnState(ContTurns.STATETURN.TURNSTART);

        sLabel = "Select Your Actions";
        fDelay = ContTurns.Get().GetTimeForActing();

        base.Execute();
    }
}
