using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulParry : Soul {

    public int nDamage;

    public void OnDamaged(Chr chrDamager) {

        //First remove this soul effect
        soulContainer.RemoveSoul(this);

        //Then retaliate with damage
        ContAbilityEngine.Get().AddClause(new Clause() {
            fExecute = () => {
                Damage dmgToDeal = new Damage(chrSource, chrDamager, 15);

                ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                    chrSource = this.chrTarget,
                    chrTarget = chrDamager,

                    dmg = dmgToDeal
                });
            }
        });
       
    }

    public SoulParry(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget) {

        sName = "Parry";

        bVisible = true;
        bDuration = true;
        nMaxDuration = 4;


        lstTriggers = new List<TriggerEffect>() {

            new TriggerEffect() {
                sub = ExecDealDamage.subAllPostTrigger,
                cb = (target, args) => {
                    //Check which character is about to be dealing damage
                    Chr chrSource = ((ExecDealDamage)args[0]).chrSource;

                    //Check which character is about to be taking damage
                    Chr chrTarget = ((ExecDealDamage)args[0]).chrTarget;

                    // If the source of the damage is owned by a different player
                    // AND if the target of the damage is the target of this soul
                    if(chrSource.plyrOwner.id != this.chrSource.plyrOwner.id && chrTarget == this.chrTarget) {
                        //Then perform a parry action
                        OnDamaged(chrSource);
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
