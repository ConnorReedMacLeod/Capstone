using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulCheerleader : Soul {

    int nPowerGain;

    public void ApplyBuff(Chr chrAlly) {

        //Make sure we are buffing an ally and not ourselves
        if (chrAlly == this.chrTarget) return;

        //Don't target dead characters
        if (chrAlly.bDead) return;

        //So we're sure we're buffing a valid character at this point

        ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
            chrSource = this.chrSource,
            chrTarget = chrAlly,

            funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {
                SoulChangePower soulPowerBuff = new SoulChangePower(_chrSource, _chrTarget, nPowerGain, 1);

                soulPowerBuff.bRemoveOnChrSourceDeath = true;

                return soulPowerBuff;
            },

            fDelay = ContTurns.fDelayStandard,
            sLabel = chrAlly.sName + " is inspired by " + this.chrSource.sName
        });
    }
    public SoulCheerleader(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget) {

        sName = "Cheerleader";

        bVisible = false;
        bDuration = false;
        bRecoilWhenApplied = false;

        bRemoveOnChrSourceDeath = true;


        nPowerGain = 5;


        lstTriggers = new List<TriggerEffect>() {

            new TriggerEffect() {
                sub = ExecTurnStartTurn.subAllPostTrigger,
                cb = (target, args) => {

                    //Only trigger if Rayna will act this turn
                    if (chrTarget.nFatigue > 0) return;

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
