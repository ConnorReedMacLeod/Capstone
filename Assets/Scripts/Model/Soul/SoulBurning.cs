using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulBurning : Soul {

    public Damage dmg;
    public int nBaseDamage;

    public SoulBurning(Chr _chrSource, Chr _chrTarget, Action _actSource) : base(_chrSource, _chrTarget, _actSource){

        sName = "Test";

        nBaseDamage = 5;

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(4);

        nBaseDamage = 30;
        //Create a base Damage object that this action will apply
        dmg = new Damage(this.chrSource, null, nBaseDamage, true);

        lstTriggers = new List<TriggerEffect>() {
            new TriggerEffect() {
                sub = ExecTurnEndTurn.subAllPostTrigger,
                cb = (target, args) => 
                {
                    Debug.Log("We have been triggered at the end of turn to add a burn damage exec");

                    //Make a copy of the damage object to give to the executable
                    Damage dmgToApply = new Damage(dmg);
                    //Give the damage object its target
                    dmgToApply.SetChrTarget(this.chrTarget);

                    ContAbilityEngine.Get().AddExec(new ExecDealDamage(this.chrSource, this.chrTarget, new Damage(dmgToApply)) {
                        sLabel = "Taking damage from Burn effect"
                    });

                 }
            }
        };
    }

    public SoulBurning(SoulBurning other, Chr _chrTarget=null) : base(other) {
        if(_chrTarget != null) {
            //If a Target was provided, then we'll use that
            chrTarget = _chrTarget;
        } else {
            //Otherwise, just copy from the other object
            chrTarget = other.chrTarget;
        }
     
        nBaseDamage = other.nBaseDamage;
        dmg = new Damage(other.dmg);
    }

}
