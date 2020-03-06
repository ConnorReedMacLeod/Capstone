﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulCheerleader : Soul {

    public int nPowerGain;

    public void ApplyBuff(Chr chrAlly) {

        //Make sure we are buffing an ally and not ourselves
        if (chrAlly == this.chrTarget) return;

        //Don't target dead characters
        if (chrAlly.bDead) {
            Debug.LogError("Attempting to buff a dead character - shouldn't have been supplied as an active character");
            return;
        }

        //So we're sure we're buffing a valid character at this point

        ContAbilityEngine.Get().AddExec(new ExecApplySoul(chrSource, chrAlly,
            new SoulChangePower(chrSource, chrAlly, actSource, nPowerGain, 1) {
                //Set up the hidden soul effect that's buffing the ally's power
                bRemoveOnChrSourceDeath = true
            }) {
            //Set up the properties of the soul application executable
            sLabel = chrAlly.sName + " is inspired by " + this.chrSource.sName

        });
    }
    public SoulCheerleader(Chr _chrSource, Chr _chrTarget, Action _actSource) : base(_chrSource, _chrTarget, _actSource) {

        sName = "Cheerleader";

        bVisible = false;
        bDuration = false;
        bRecoilWhenApplied = false;

        bRemoveOnChrSourceDeath = true;


        nPowerGain = 5;


        lstTriggers = new List<TriggerEffect>() {

            new TriggerEffect() {
                sub = ExecReadyChar.subAllPostTrigger,
                cb = cbOnReady
            }
        };
    }

    public void cbOnReady(Object target, object[] args) {
        //Check which character is about to be taking damage
        Chr chrReadied = ((ExecReadyChar)args[0]).chrTarget;

        //Only trigger if the readied character is our target
        if (chrTarget != chrReadied) return;

        //Loop through each of the characters on this character's team, and let ApplyBuff decide
        // if they should recieve a buff
        foreach (Chr chrAlly in chrTarget.plyrOwner.GetActiveChrs()) {
            ApplyBuff(chrAlly);
        }
    }

}
