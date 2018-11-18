using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecTurnEndTurn : Executable {

    public static Subject subAllTurnEnd = new Subject();

    public void EndTurn() {

        subAllTurnEnd.NotifyObs(null);

    }

    public override void Execute() {

        EndTurn();

        ContTurns.Get().SetTurnState(ContTurns.STATETURN.GIVEMANA);

        sLabel = "End of Turn";
        fDelay = 0.5f;

        base.Execute();
    }
}
