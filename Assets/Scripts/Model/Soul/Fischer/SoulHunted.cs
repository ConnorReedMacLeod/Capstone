using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulHunted : SoulChr {

    public int nDefenseLoss;

    public SoulHunted(Chr _chrSource, Chr _chrTarget, Skill _skillSource) : base(_chrSource, _chrTarget, _skillSource) {

        sName = "Hunted";

        bVisible = true;
        bDuration = false;

        bRemoveOnChrSourceDeath = true;

        nDefenseLoss = -5;

    }

    public override void InitTriggers() {
        lstTriggers = new List<TriggerEffect>() {

            new TriggerEffect() {
                sub = ExecDealDamage.subAllPreTrigger,
                cb = cbOnDealDamage
            }
        };
    }


    public void cbOnDealDamage(Object target, object[] args) {
        //Check which character is about to be dealing damage
        Chr chrSource = ((ExecDealDamage)args[0]).chrSource;

        //Check which character is about to be taking damage
        Chr chrTarget = ((ExecDealDamage)args[0]).chrTarget;

        //If the source of the damage is the chr who applied this soul
        // AND if the target of the damage is the target of this soul
        if(chrSource == this.chrSource && chrTarget == this.chrTarget) {
            //Then decrease the target's defense for the turn
            ApplyDefenseDebuff();
        }
    }

    public void ApplyDefenseDebuff() {

        ContSkillEngine.PushSingleExecutable(new ExecApplySoul(chrSource, chrTarget,
            new SoulChangeDefense(chrTarget, chrTarget, this.skillSource, nDefenseLoss, 1)));

    }

    public SoulHunted(SoulHunted other, Chr _chrTarget = null) : base(other) {
        if(_chrTarget != null) {
            //If a Target was provided, then we'll use that
            chrTarget = _chrTarget;
        } else {
            //Otherwise, just copy from the other object
            chrTarget = other.chrTarget;
        }

        nDefenseLoss = other.nDefenseLoss;

    }

}
