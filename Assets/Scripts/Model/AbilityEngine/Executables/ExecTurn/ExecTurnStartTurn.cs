using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecTurnStartTurn : Executable {


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


    public override void Execute() {

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
