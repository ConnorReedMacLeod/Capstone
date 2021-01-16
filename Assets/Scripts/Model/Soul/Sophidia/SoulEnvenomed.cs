﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulEnvenomed : Soul {

    public int nLifeLoss;

    public SoulEnvenomed(Chr _chrSource, Chr _chrTarget, Action _actSource) : base(_chrSource, _chrTarget, _actSource) {

        sName = "Envenomed";

        nLifeLoss = 5;

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(3);


        lstTriggers = new List<TriggerEffect>() {
            new TriggerEffect() {
                sub = ExecTurnEndTurn.subAllPostTrigger,
                cb = cbOnEndTurn
            },

            new TriggerEffect() {
                sub = ExecDealDamage.subAllPostTrigger,
                cb = cbOnDamaged
            }
        };


    }

    public void cbOnEndTurn(Object target, object[] args) {

        Debug.Log("End of turn for envenomed chr, chrTarget=" + chrTarget);
        ContAbilityEngine.Get().AddExec(new ExecLoseLife(chrSource, chrTarget, nLifeLoss) {
            sLabel = "Get me a cleanser booster!"
        });

    }

    public void cbOnDamaged(Object target, object[] args) {
        //Check which character just took damage
        Chr chrDamaged = ((ExecDealDamage)args[0]).chrTarget;

        //If that character is the person who this Soul is applied to
        if(chrDamaged == this.chrTarget) {
            //Then increase the duration
            IncreaseDuration();
        }
    }

    public void IncreaseDuration() {

        nCurDuration++;

        Debug.Log("Increasing envenomed duration to " + nCurDuration);

        //Change the Max duration to be one higher
        pnMaxDuration.SetBase(1 + pnMaxDuration.GetBase()());

        //Let the SoulContainer know to update the visuals (it updates everything cause it's a bit clueless)
        chrTarget.soulContainer.subVisibleSoulUpdate.NotifyObs();
    }

    public SoulEnvenomed(SoulEnvenomed other, Chr _chrTarget = null) : base(other/*TODONOW - add ", _chrTarget" here*/) {
        Debug.Log("Calling SoulEnvenomed Copy Constructor at " + Time.time);
        if(_chrTarget != null) {
            //If a Target was provided, then we'll use that
            chrTarget = _chrTarget;
        } else {
            //Otherwise, just copy from the other object
            chrTarget = other.chrTarget;
        }

        nLifeLoss = other.nLifeLoss;

        Debug.Log("Soulenvenomed created with chrTarget = " + chrTarget.sName);
        Debug.Log("Test calling cbOnEndTurn");
        cbOnEndTurn(null, null);
        Debug.Log("Done calling cbOnEndTurn");
        Debug.Log("Calling from trigger list");
        lstTriggers[0].cb(null, null);
        Debug.Log("Done calling lsttriggers");
    }

}
