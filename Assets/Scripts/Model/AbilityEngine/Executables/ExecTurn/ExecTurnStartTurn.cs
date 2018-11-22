using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecTurnStartTurn : Executable {

    public static Subject subAllTurnStart = new Subject();

    public void StartTurn() {

        //Controller.Get().contTarget.LockTargetting();

        subAllTurnStart.NotifyObs(null);
        //TODO - MAKE CHARACTERS OBSERVE THIS AND LOCK THEIR ABILITY SELECTION
        // Confirm that all characters should be locked in between abilities 
        //   - don't want weird ability swapping mid-ability selection

    }

    public override void Execute() {

        StartTurn();

        if (ContTurns.Get().GetNextActingChr() == null) {
            //If there are no characters set to go this turn, then jump to end of turn directly
            ContTurns.Get().SetTurnState(ContTurns.STATETURN.TURNEND);

        } else {
            //If there is a character set to act, then let the player choose their actions
            ContTurns.Get().SetTurnState(ContTurns.STATETURN.CHOOSEACTIONS);
        }

        sLabel = "Beginning of Turn";
        fDelay = 0.5f;

        base.Execute();
    }
}
