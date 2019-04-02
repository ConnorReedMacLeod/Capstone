using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulEnvenomed : Soul {

    public int nLifeLoss;

    public void IncreaseDuration() {

        nCurDuration++;

        //Change the Max duration to be one higher
        pnMaxDuration.SetBase(1 + pnMaxDuration.GetBase()());

        //Let the SoulContainer know to update the visuals (it updates everything cause it's a bit clueless)
        chrTarget.soulContainer.subVisibleSoulUpdate.NotifyObs();
    }

    public SoulEnvenomed(Chr _chrSource, Chr _chrTarget, Action _actSource) : base(_chrSource, _chrTarget, _actSource) {

        sName = "Envenomed";

        nLifeLoss = 5;

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(3);


        lstTriggers = new List<TriggerEffect>() {
            new TriggerEffect() {
                sub = ExecTurnEndTurn.subAllPostTrigger,
                cb = (target, args) =>
                {
                    ContAbilityEngine.Get().AddExec(new ExecLoseLife() {
                        chrSource = this.chrSource,
                        chrTarget = this.chrTarget,
                        nLifeLoss = this.nLifeLoss,

                        fDelay = ContTurns.fDelayStandard,
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

}
