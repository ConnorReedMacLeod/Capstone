using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulBurning : Soul {

    public SoulBurning() : base(){

        lstTriggers = new List<TriggerEffect>() {
            new TriggerEffect() {
                sub = ExecTurnEndTurn.subAllPostTrigger,
                cb = (target, args) => 
                {
                    Debug.Log("We have been triggered at the end of turn to add a burn damage exec");
                    ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                        chrOwner = this.chrOwner,
                        chrTarget = tar,
                        nDamage = 5,
                        fDelay = 1.0f,
                        sLabel = tar.sName + " is Burning"
                    });

                 }
            }
        };
    }

    public override void funcOnApplication() {
        Debug.Log("Burning has been applied");
    }

    public override void funcOnRemoval() {
        Debug.Log("Burning has been removed");
    }

    public override void funcOnExpiration() {
        Debug.Log("Burning has expired");
    }
}
