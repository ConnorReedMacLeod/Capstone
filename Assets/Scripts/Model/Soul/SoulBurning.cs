using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulBurning : Soul {

    public int nBaseDamage;

    public SoulBurning(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget){

        sName = "Test";

        nBaseDamage = 5;

        bVisible = true;
        bDuration = true;
        nMaxDuration = 4;

        //Make an initial reference for how much damage should be dealt
        Damage dmgToDeal = new Damage(chrSource, chrTarget, nBaseDamage, true);

        lstTriggers = new List<TriggerEffect>() {
            new TriggerEffect() {
                sub = ExecTurnEndTurn.subAllPostTrigger,
                cb = (target, args) => 
                {
                    Debug.Log("We have been triggered at the end of turn to add a burn damage exec");
                    ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
                        chrSource = this.chrSource,
                        chrTarget = this.chrTarget,

                        //Make a copy of the initially calculated damage when the soul was applied
                        dmg = new Damage(dmgToDeal), 

                        fDelay = 1.0f,
                        sLabel = this.chrTarget.sName + " is Burning"
                    });

                 }
            }
        };
    }

    public override void funcOnApplication() {
        Debug.Log(sName + " has been applied");
    }

    public override void funcOnRemoval() {
        Debug.Log(sName + " has been removed");
    }

    public override void funcOnExpiration() {
        Debug.Log(sName + " has expired");
    }
}
