using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulSadism : Soul {

    public Healing heal;
    public int nBaseHealing;

    public void GainLife() {

        ContAbilityEngine.Get().AddExec(new ExecHeal(chrTarget, chrTarget, heal) {

            arSoundEffects = new SoundEffect[] { new SoundEffect("PitBeast/sndSadism", 1.067f) },
            sLabel = "ooh it hurts so good"
        });
    }

    public SoulSadism(Chr _chrSource, Chr _chrTarget, Action _actSource) : base(_chrSource, _chrTarget, _actSource) {

        sName = "Sadism";

        bVisible = false;
        bDuration = false;
        bRecoilWhenApplied = false;

        nBaseHealing = 5;

        //Create a base Healing object that this action will apply 
        heal = new Healing(this.chrSource, this.chrSource, nBaseHealing);

        lstTriggers = new List<TriggerEffect>() {

            new TriggerEffect() {
                sub = ExecDealDamage.subAllPreTrigger,
                cb = cbOnDealDamage
            }
        };
    }

    public void cbOnDealDamage(Object target, object[] args) {
        //Check which character is about to be dealing damage
        Chr dmgSource = ((ExecDealDamage)args[0]).chrSource;

        //Check which character is about to be taking damage
        Chr dmgTarget = ((ExecDealDamage)args[0]).chrTarget;

        //If the source of the damage is the chr this buff is on
        // and if we're dealing damage to an enemy
        if (dmgSource == this.chrTarget && this.chrTarget.plyrOwner != dmgTarget.plyrOwner) {

            //Then check if the chr this buff is on has lower health than
            //who they are attacking
            if (this.chrTarget.nCurHealth < dmgTarget.nCurHealth) {
                GainLife();
            }
        }
    }
}
