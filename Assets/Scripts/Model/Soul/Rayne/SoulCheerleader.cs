using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulCheerleader : Soul {

    int nPowerGain;

    public void ApplyBuff(Chr chrAlly) {

        //Make sure we are buffing an ally
        if (chrAlly.plyrOwner != this.chrTarget) return;

        //Make sure we're not buffing ourselves

        ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
            chrSource = this.chrSource,
            chrTarget = this.chrSource,

            funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {
                return new SoulChangePower(_chrSource, _chrTarget, nPowerGain, 1);
            },

            fDelay = 1.0f,
            sLabel = this.chrSource.sName + " is inspired by " + this.chrTarget.sName
        });
    }
    public SoulCheerleader(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget) {

        sName = "Cheerleader";

        bVisible = false;
        bDuration = false;

        nPowerGain = 5;

        lstTriggers = new List<TriggerEffect>() {

            new TriggerEffect() {
                sub = ExecTurnStartTurn.subAllPostTrigger,
                cb = (target, args) => {
                    
                    //Loop through each of the characters on this character's team, and let ApplyBuff decide
                    // if they should recieve a buff
                    foreach (Chr chrAlly in chrSource.plyrOwner.arChr) {
                        ApplyBuff(chrAlly);
                    }

                }
            }
        };
    }

}
