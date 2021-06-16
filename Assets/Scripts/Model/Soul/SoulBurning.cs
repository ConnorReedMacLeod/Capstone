using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulBurning : Soul {

    public Damage dmg;
    public int nBaseDamage;

    public SoulBurning(Chr _chrSource, Chr _chrTarget, Skill _skillSource) : base(_chrSource, _chrTarget, _skillSource) {

        sName = "Test";

        nBaseDamage = 5;

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(4);

        //Create a base Damage object that this skill will apply
        dmg = new Damage(this.chrSource, null, nBaseDamage, true);

        InitTriggers();
    }

    public override void InitTriggers() {

        lstTriggers = new List<TriggerEffect>() {
            new TriggerEffect() {
                sub = ExecTurnEndTurn.subAllPostTrigger,
                cb = cbOnEndTurn
            }
        };
    }

    public void cbOnEndTurn(Object target, object[] args) {
        Debug.Log("We have been triggered at the end of turn to add a burn damage exec");

        ContSkillEngine.Get().AddExec(new ExecDealDamage(this.chrSource, this.chrTarget, new Damage(dmg)) {
            sLabel = "Taking damage from Burn effect"
        });
    }

    public SoulBurning(SoulBurning other, Chr _chrTarget = null) : base(other) {
        if(_chrTarget != null) {
            //If a Target was provided, then we'll use that
            chrTarget = _chrTarget;
        } else {
            //Otherwise, just copy from the other object
            chrTarget = other.chrTarget;
        }

        nBaseDamage = other.nBaseDamage;
        dmg = new Damage(other.dmg);

        InitTriggers();
    }

}
