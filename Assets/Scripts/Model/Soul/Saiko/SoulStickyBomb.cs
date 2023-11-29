using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulStickyBomb : SoulChr {

    public int nDetonationDamage;
    public Damage dmg;

    public SoulStickyBomb(Chr _chrSource, Chr _chrTarget, Skill _skillSource) : base(_chrSource, _chrTarget, _skillSource) {

        sName = "StickyBomb";

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(1);

        nDetonationDamage = 30;

        //Create a base Damage object that this skill will apply
        dmg = new Damage(this.chrSource, null, nDetonationDamage);

    }

    public override void ApplicationEffect() {
        base.ApplicationEffect();

        //When this effect is applied, save the power value as it is right now
        // so that future changes to the chrSource's power won't affect the damage
        dmg.SnapShotPower();
    }

    //Only want the damage to go off if the soul effect expires naturally 
    public override void ExpirationEffect() {
        base.ExpirationEffect();

        ContSkillEngine.Get().AddExec(new ExecDealDamage(chrSource, chrTarget, dmg) {
            arSoundEffects = new SoundEffect[] { new SoundEffect("Saiko/sndStickyBombDetonate", 3.1f) },
            sLabel = "Ai-same-CRIER, aibu-save-LIAR"
        });

    }

    public SoulStickyBomb(SoulStickyBomb other, Chr _chrTarget = null) : base(other, _chrTarget) {


        nDetonationDamage = other.nDetonationDamage;
        dmg = new Damage(other.dmg);

    }

}
