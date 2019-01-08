using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulCheerleader : Soul {

    int nPowerGain;

    public void ApplyBuff(Chr chrAlly) {

        //Make sure we are buffing an ally and not ourselves
        if (chrAlly == this.chrTarget) return;

        //So we're sure we're buffing a valid character at this point
        Debug.Log(chrAlly.sName + " is being buffed");

        ContAbilityEngine.Get().AddExec(new ExecApplySoul() {
            chrSource = this.chrSource,
            chrTarget = chrAlly,

            funcCreateSoul = (Chr _chrSource, Chr _chrTarget) => {
                return new SoulChangePower(_chrSource, _chrTarget, nPowerGain, 1);
            },

            fDelay = 1.0f,
            sLabel = this.chrSource.sName + " is inspired by " + chrAlly.sName
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
                    Debug.Log("beginning of turn");

                    //Only trigger if Rayna will act this turn
                    if (chrTarget.nFatigue > 0) return;
                    Debug.Log("rayna's gonna act this turn");

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
