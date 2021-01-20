using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulStickyBomb : Soul {

    public int nDetonationDamage;
    public Damage dmg;

    public SoulStickyBomb(Chr _chrSource, Chr _chrTarget, Action _actSource) : base(_chrSource, _chrTarget, _actSource) {

        sName = "StickyBomb";

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(1);

        nDetonationDamage = 30;

        //Create a base Damage object that this action will apply
        dmg = new Damage(this.chrSource, null, nDetonationDamage);

    }

    public override void ApplicationEffect() {
        //When this effect is applied, save the power value as it is right now
        // so that future changes to the chrSource's power won't affect the damage
        dmg.SnapShotPower();
    }

    //Only want the damage to go off if the soul effect expires naturally 
    public override void ExpirationEffect() {

        ContAbilityEngine.Get().AddExec(new ExecDealDamage(chrSource, chrTarget, dmg) {
            arSoundEffects = new SoundEffect[] { new SoundEffect("Saiko/sndStickyBombDetonate", 3.1f) },
            sLabel = "Ai-same-CRIER, aibu-save-LIAR"
        });

    }

    public SoulStickyBomb(SoulStickyBomb other, Chr _chrTarget = null) : base(other) {
        if(_chrTarget != null) {
            //If a Target was provided, then we'll use that
            chrTarget = _chrTarget;
        } else {
            //Otherwise, just copy from the other object
            chrTarget = other.chrTarget;
        }
        nDetonationDamage = other.nDetonationDamage;
        dmg = new Damage(other.dmg);

    }
}
