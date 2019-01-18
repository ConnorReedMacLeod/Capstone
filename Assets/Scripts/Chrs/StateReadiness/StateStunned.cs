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

        //First, increase the fatigue value of the character
        ContAbilityEngine.Get().AddExec(new ExecChangeFatigue {
            chrSource = null,  //No character is the cause of the increase of fatigue - 
                               //it's just a direct consequence of some character stunning us (which causes fatigue increase)
            chrTarget = chrOwner,

            nAmount = nStunAmount
        });

        //Then, add a replacement effect to cancel out any further stuns that we would take
        repStun = new Replacement() {

            //The list of replacement effects we'll include ourselves in
            lstExecReplacements = ExecStun.lstAllFullReplacements,

            //Note that the parameter type is the generic Executable
            // - should cast to the proper type if further checking is required
            shouldReplace = (Executable exec) => {
                Debug.Assert(typeof(ExecStun) == exec.GetType());

                //replace only if the stunned character will be the character this effect is on
                return ((ExecDealDamage)exec).chrTarget == this.chrOwner;
            },

            //Just replace the executable with a completely new null executable
            execReplace = (Executable exec) => new ExecNull()

        };

        //Register this replacement effect so that it will take effect
        Replacement.Register(repStun);

    }

    public override void OnLeave() {

        //Since we're leaving the stun state, we no longer need to cancel out stuns
        Replacement.Unregister(repStun);

    }
}
