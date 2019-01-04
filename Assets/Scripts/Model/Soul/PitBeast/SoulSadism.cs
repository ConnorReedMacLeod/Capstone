﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulSadism : Soul {

    public int nLifeGain;

    public void GainLife() {

        ContAbilityEngine.Get().AddExec(new ExecHeal() {
            chrSource = this.chrSource,
            chrTarget = this.chrSource,

            nAmount = this.nLifeGain,

            fDelay = 1.0f,
            sLabel = this.chrSource.sName + " is revelling in the pain"
        });
    }

    public SoulSadism(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget) {

        sName = "Sadism";

        bVisible = false;
        bDuration = false;

        nLifeGain = 5;

        lstTriggers = new List<TriggerEffect>() {

            new TriggerEffect() {
                sub = ExecDealDamage.subAllPreTrigger,
                cb = (target, args) => {
                    //Check which character is about to be dealing damage
                    Chr dmgSource = ((ExecDealDamage)args[0]).chrSource;

                    //Check which character is about to be taking damage
                    Chr dmgTarget = ((ExecDealDamage)args[0]).chrTarget;

                    //If the source of the damage is the chr this buff is on
                    // and if we're dealing damage to an enemy
                    if(dmgSource == this.chrTarget && this.chrTarget.plyrOwner != dmgTarget.plyrOwner) {
                        
                        //Then check if the chr this buff is on has higher health than
                        //who they are attacking
                        if(this.chrTarget.nCurHealth < dmgTarget.nCurHealth) {
                            GainLife();
                        }
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