using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulBurning : Soul {

    public Damage dmg;
    public int nBaseDamage;

    public SoulBurning(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget){

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

                    ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                        chrSource = this.chrSource,
                        chrTarget = this.chrTarget,

                        //Make a copy of the initially calculated damage when the soul was applied
                        dmg = new Damage(dmgToApply), 

                        fDelay = 1.0f,
                        sLabel = this.chrTarget.sName + " is Burning"
                    });

                 }
            }
        };
    }

}
