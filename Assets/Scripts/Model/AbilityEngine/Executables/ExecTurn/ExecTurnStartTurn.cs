using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecTurnStartTurn : Executable {

    public static Subject subAllTurnStart = new Subject();

    public void StartTurn() {

        subAllTurnStart.NotifyObs(null);
        //TODO - MAKE CHARACTERS OBSERVE THIS AND LOCK THEIR ABILITY SELECTION

    }

    public override void Execute() {

        StartTurn();

        ContTurns.Get().SetTurnState(ContTurns.STATETURN.EXECUTEACTIONS);

        sLabel = "Beginning of Turn";
        fDelay = 0.5f;

        base.Execute();
    }
}
