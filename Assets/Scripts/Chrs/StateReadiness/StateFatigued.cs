using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateFatigued : StateReadiness {

    public int nStartingFatigue;

    public StateFatigued(Chr _chrOwner, int _nStartingFatigue) : base(_chrOwner) {

        nStartingFatigue = _nStartingFatigue;
        
    }

    public override void OnEnter() {

        ContAbilityEngine.Get().AddExec(new ExecChangeFatigue {
            chrSource = null, //Since no character is actually the source of this effect - it's just the game rules
            chrTarget = chrOwner,

            nAmount = nStartingFatigue
        });

    }

}
