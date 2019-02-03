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

            ContAbilityEngine.Get().AddExec(new ExecReadyChar {
                chrSource = null, //Since no character is actually the source of this effect - it's just the game rules
                chrTarget = chrOwner,

                fDelay = 1.0f,
                sLabel = chrOwner.sName + " is Readying"
            });

        }
    }

    public override void OnEnter() {
        //Let observers know to start paying attention to the fatigue value now
        chrOwner.subFatigueChange.NotifyObs();
    }
}
