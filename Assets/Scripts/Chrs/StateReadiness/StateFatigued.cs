using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateFatigued : StateReadiness {

    public StateFatigued(Chr _chrOwner) : base(_chrOwner) {
        
    }

    public override TYPE Type() {
        return TYPE.FATIGUED;
    }

    public override void Ready() {
        if (chrOwner.bDead) {
            Debug.Log("Tried to Ready, but " + chrOwner.sName + " is dead");
            return;
        }

        if (chrOwner.nFatigue == 0) {
            //Then transition to the ready state

            //Leave the source as null since it's just the game rules causing the readying
            ContAbilityEngine.Get().AddExec(new ExecReadyChar(null, chrOwner){

                fDelay = ContTurns.fDelayStandard,
                sLabel = chrOwner.sName + " is Readying"
            });

        }
    }

    public override void OnEnter() {
        //Let observers know to start paying attention to the fatigue value now
        chrOwner.subFatigueChange.NotifyObs();
        chrOwner.subChannelTimeChange.NotifyObs();
    }
}
