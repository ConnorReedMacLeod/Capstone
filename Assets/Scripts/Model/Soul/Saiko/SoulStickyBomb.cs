using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulStickyBomb : Soul {

    public int nDetonationDamage;
    public Damage dmg;

    public void Detonate() {

        //Make a copy of the damage object to give to the executable
        Damage dmgToApply = new Damage(dmg);
        //Give the damage object its target
        dmgToApply.SetChrTarget(this.chrTarget);

        ContAbilityEngine.Get().AddExec(new ExecDealDamage() {
            chrSource = this.chrSource,
            chrTarget = this.chrTarget,
            dmg = dmgToApply,

            arSoundEffects = new SoundEffect[] { new SoundEffect("Saiko/sndStickyBombDetonate", 3.1f) },

            fDelay = ContTurns.fDelayStandard,
            sLabel = this.chrTarget.sName + "'s bomb is exploding"
        });

    }

    public SoulStickyBomb(Chr _chrSource, Chr _chrTarget, Action _actSource) : base(_chrSource, _chrTarget, _actSource) {

        sName = "StickyBomb";

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(1);

        nDetonationDamage = 30;
        //Create a base Damage object that this action will apply
        dmg = new Damage(this.chrSource, null, nDetonationDamage);

        funcOnExpiration = () => {
            //Deal damage only if this soul effect expires naturally
            Detonate();
        };

        funcOnApplication = () => {
            //When this effect is applied, save the power value right now
            dmg.SnapShotPower();
        };
    }


}
