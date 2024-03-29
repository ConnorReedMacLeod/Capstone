﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulEnvenomed : SoulChr {

    public int nLifeLoss;

    public SoulEnvenomed(Chr _chrSource, Chr _chrTarget, Skill _skillSource) : base(_chrSource, _chrTarget, _skillSource) {

        sName = "Envenomed";

        nLifeLoss = 5;

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(3);


    }

    public override void InitTriggers() {
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

        ContSkillEngine.Get().AddExec(new ExecLoseLife(chrSource, chrTarget, nLifeLoss) {
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

        //Change the Max duration to be one higher
        pnMaxDuration.SetBase(1 + pnMaxDuration.GetBase()());

        //Let the SoulContainer know to update the visuals (it updates everything cause it's a bit clueless)
        chrTarget.soulContainer.subVisibleSoulUpdate.NotifyObs();
    }

    public SoulEnvenomed(SoulEnvenomed other, Chr _chrTarget = null) : base(other, _chrTarget) {

        nLifeLoss = other.nLifeLoss;


    }

}
