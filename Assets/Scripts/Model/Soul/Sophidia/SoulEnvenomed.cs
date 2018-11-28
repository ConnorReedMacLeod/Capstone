using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulEnvenomed : Soul {

    public int nLifeLoss;

    public void IncreaseDuration() {
        nCurDuration++;

        //Let the SoulContainer know to update the visuals (it updates everything cause it's a bit clueless)
        chrTarget.soulContainer.subVisibleSoulUpdate.NotifyObs();
    }

    public SoulEnvenomed(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget) {

        sName = "Envenomed";

        nLifeLoss = 5;

        bVisible = true;
        bDuration = true;
        nMaxDuration = 3;


        lstTriggers = new List<TriggerEffect>() {
            new TriggerEffect() {
                sub = ExecTurnEndTurn.subAllPostTrigger,
                cb = (target, args) =>
                {
                    Debug.Log("We have been triggered at the end of turn to add a venom damage exec");
                    ContAbilityEngine.Get().AddExec(new ExecLoseLife() {
                        chrSource = this.chrSource,
                        chrTarget = this.chrTarget,
                        nLifeLoss = this.nLifeLoss,

                        fDelay = 1.0f,
                        sLabel = this.chrTarget.sName + " is Poisoned"
                    });

                 }
            },

            new TriggerEffect() {
                sub = ExecDealDamage.subAllPostTrigger,
                cb = (target, args) => {
                    //Check which character just took damage
                    Chr chrDamaged = ((ExecDealDamage)args[0]).chrTarget;

                    //If that character is the person who this Soul is applied to
                    if(chrDamaged == this.chrTarget) {
                        //Then increase the duration
                        IncreaseDuration();
                    }

                }
            }
        };
    }

    public override void funcOnApplication() {
        Debug.Log(sName + " has been applied");
    }

    public override void funcOnRemoval() {
        Debug.Log(sName + " has been removed");
    }

    public override void funcOnExpiration() {
        Debug.Log(sName + " has expired");
    }
}
