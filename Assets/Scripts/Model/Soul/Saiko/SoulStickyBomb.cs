using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulStickyBomb : Soul {

    public int nDetonationDamage;

    public void Detonate() {

        ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
            chrSource = this.chrSource,
            chrTarget = this.chrTarget,
            dmg = new Damage(this.chrSource, this.chrTarget, nDetonationDamage),

            fDelay = 1.0f,
            sLabel = this.chrTarget.sName + "'s bomb is exploding"
        });

    }

    public SoulStickyBomb(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget) {

        sName = "Stuck";
        
        nDetonationDamage = 30;

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(1);

    }

    public override void funcOnExpiration() { 

        //Deal damage only if this soul effect expires naturally
        Detonate();
    }
}
