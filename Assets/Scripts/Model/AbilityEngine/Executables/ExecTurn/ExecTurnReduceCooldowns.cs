using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecTurnReduceCooldowns : Executable {

    public void ReduceCooldowns() {

        //TODO:: Consider breaking this into smaller Executables

        for (int i = 0; i < Match.Get().nPlayers; i++) {
            for (int j = 0; j < Player.MAXCHRS; j++) {
                if (Match.Get().arChrs[i][j] == null) {
                    continue; // A character isn't actually here (extra space for characters)
                }

                //Reduce the character's recharge
                Match.Get().arChrs[i][j].TimeTick();
                //Reduce the cd of that character's actions
                Match.Get().arChrs[i][j].RechargeActions();

            }
        }
    }

    public override void Execute() {

        ReduceCooldowns();

        ContTurns.Get().SetTurnState(ContTurns.STATETURN.CHOOSEACTIONS);

        sLabel = "Reducing Cooldowns";
        fDelay = 0.5f;

        base.Execute();
    }
}
