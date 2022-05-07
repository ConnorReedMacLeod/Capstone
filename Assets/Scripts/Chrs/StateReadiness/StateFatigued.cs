using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateFatigued : StateReadiness {

    public StateFatigued(Chr _chrOwner) : base(_chrOwner) {

    }

    public override TYPE Type() {
        return TYPE.FATIGUED;
    }

    public override void ReadyIfNoFatigue() {
        if(chrOwner.bDead) {
            Debug.LogFormat("Tried to Ready, but {0} is dead", chrOwner.sName);
            return;
        }

        if(chrOwner.nFatigue == 0) {
            if (chrOwner.position.positiontype == Position.POSITIONTYPE.BENCH) {
                Debug.LogFormat("Tried to Ready, but {0} is on the bench", chrOwner.sName);
                return;
            }

            //Then transition to the ready state

            //Leave the source as null since it's just the game rules causing the readying
            ContSkillEngine.Get().AddExec(new ExecReadyChar(null, chrOwner) {

                fDelay = ContTime.fDelayStandard,
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
