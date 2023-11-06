using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulBurdened : SoulChr {

    public int nPowerDebuff;

    public SoulChangePower soulChangePower;

    public SoulBurdened(Chr _chrSource, Chr _chrTarget, Skill _skillSource) : base(_chrSource, _chrTarget, _skillSource) {

        sName = "Burdened";

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(4);

        nPowerDebuff = -5;
        
    }

    public override void ApplicationEffect() {
        base.ApplicationEffect();

        //Make a Permanent SoulChangePower, and save a reference to it, so it can be removed later
        soulChangePower = new SoulChangePower(chrSource, chrTarget, skillSource, nPowerDebuff);
        chrTarget.soulContainer.ApplySoul(soulChangePower);
    }

    public override void RemoveEffect() {
        base.RemoveEffect();
        
        chrTarget.soulContainer.RemoveSoul(soulChangePower);

    }

    public SoulBurdened(SoulBurdened other, Chr _chrTarget = null) : base(other, _chrTarget) {
        nPowerDebuff = other.nPowerDebuff;
    }

}
