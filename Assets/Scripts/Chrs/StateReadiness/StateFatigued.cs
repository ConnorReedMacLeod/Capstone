using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateFatigued : StateReadiness {

    public StateFatigued(Chr _chrOwner) : base(_chrOwner) {
        
    }

    public override void Ready() {
        if(chrOwner.nFatigue == 0) {
            //Then transition to the ready state

            ContAbilityEngine.Get().AddExec(new ExecReadyChar {
                chrSource = null, //Since no character is actually the source of this effect - it's just the game rules
                chrTarget = chrOwner,

            });

        }
    }

}
