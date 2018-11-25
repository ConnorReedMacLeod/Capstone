using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecTurnReduceCooldowns : Executable {




    //Note:: This section should be copy and pasted for each type of executable
    //       We could do a gross thing like 
    //        this.GetType().GetMember("subAllPreTrigger", BindingFlags.Public |BindingFlags.Static);
    //       in a single base implementation of GetPreTrigger, but this should be slower and less reliable
    public static Subject subAllPreTrigger = new Subject();
    public static Subject subAllPostTrigger = new Subject();

    public override Subject GetPreTrigger() {
        return subAllPreTrigger; //Note this auto-resolves to the static member
    }
    public override Subject GetPostTrigger() {
        return subAllPostTrigger;
    }
    // This is the end of the section that should be copied and pasted



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

        ContTurns.Get().SetTurnState(ContTurns.STATETURN.TURNSTART);

        sLabel = "Reducing Cooldowns";
        fDelay = 0.5f;

        base.Execute();
    }
}
