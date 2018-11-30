using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulHunted : Soul {

    public int nLifeLoss;

    public void ApplyDefenseDebuff() {

        ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
            chrSource = this.chrSource,
            chrTarget = this.chrTarget,

            funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {
                return new SoulClosingIn(_chrSource, _chrTarget);
            }
        });
        }

    public SoulHunted(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget) {

        sName = "Hunted";

        bVisible = true;
        bDuration = false;


        lstTriggers = new List<TriggerEffect>() {

            new TriggerEffect() {
                sub = ExecDealDamage.subAllPreTrigger,
                cb = (target, args) => {
                    //Check which character is about to be dealing damage
                    Chr chrSource = ((ExecDealDamage)args[0]).chrSource;

                    //Check which character is about to be taking damage
                    Chr chrTarget = ((ExecDealDamage)args[0]).chrTarget;

                    //If the source of the damage is the chr who applied this soul
                    // AND if the target of the damage is the target of this soul
                    if(chrSource == this.chrSource && chrTarget == this.chrTarget) {
                        //Then decrease the target's defense for the turn
                        ApplyDefenseDebuff();
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
