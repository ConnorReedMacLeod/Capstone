using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateStunned : StateReadiness {

    public int nStunAmount;
    Replacement repStun; //A reference to the stunning replacement

    public StateStunned(Chr _chrOwner, int _nStunAmount) : base(_chrOwner) {

        nStunAmount = _nStunAmount;

    }

    public override TYPE Type() {
        return TYPE.STUNNED;
    }


    //Same implementation as Fatigued
    public override void Ready() {
        if (chrOwner.bDead) {
            Debug.Log("Tried to ready, but " + chrOwner.sName + " is dead");
            return;
        }

        if (chrOwner.nFatigue == 0) {
            //Then transition to the ready state


            //Leave the source as null since it's just the game rules causing the readying
            ContSkillEngine.Get().AddExec(new ExecReadyChar (null, chrOwner) { 

                fDelay = ContTurns.fDelayStandard,
                sLabel = chrOwner.sName + " is Readying"
            });

        }
    }


    public override void OnEnter() {

        //First, increase the fatigue value of the character
        ContSkillEngine.Get().AddExec(new ExecChangeFatigue (null, chrOwner, nStunAmount, false));

        //Then, add a replacement effect to cancel out any further stuns that we would take
        repStun = new Replacement() {

            //The list of replacement effects we'll include ourselves in
            lstExecReplacements = ExecStun.lstAllFullReplacements,

            //Note that the parameter type is the generic Executable
            // - should cast to the proper type if further checking is required
            shouldReplace = (Executable exec) => {
                Debug.Assert(typeof(ExecStun) == exec.GetType());

                //replace only if the stunned character will be the character this effect is on
                return ((ExecStun)exec).chrTarget == this.chrOwner;
            },

            //Just replace the executable with a completely new null executable
            execReplace = (Executable exec) => new ExecNull(exec.chrSource)

        };

        //Register this replacement effect so that it will take effect
        Replacement.Register(repStun);

        //Let observers know to start paying attention to the fatigue value now
        // and to clear out any channeling time (if applicable)
        chrOwner.subFatigueChange.NotifyObs();
        chrOwner.subChannelTimeChange.NotifyObs();
    }


    public override void OnLeave() {

        //Since we're leaving the stun state, we no longer need to cancel out stuns
        Replacement.Unregister(repStun);

    }
}
