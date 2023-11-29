using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulEvolved : SoulChr {

    public int nPowerBuff;

    public SoulChangePower soulChangePower;

    public SoulEvolved(Chr _chrSource, Chr _chrTarget, Skill _skillSource) : base(_chrSource, _chrTarget, _skillSource) {

        sName = "Evolved";

        nPowerBuff = 5;

        bVisible = false;
        bDuration = false;

        bRecoilWhenApplied = false;

    }

    public override void ApplicationEffect() {
        base.ApplicationEffect();

        //Make a Permanent SoulChangePower, and save a reference to it, so it can be removed later
        soulChangePower = new SoulChangePower(chrSource, chrTarget, skillSource, nPowerBuff);
        chrTarget.soulContainer.ApplySoul(soulChangePower);
    }

    public override void RemoveEffect() {
        base.RemoveEffect();

        chrTarget.soulContainer.RemoveSoul(soulChangePower);

    }

    public SoulEvolved(SoulEvolved other, Chr _chrTarget = null) : base(other, _chrTarget) {

        nPowerBuff = other.nPowerBuff;

    }
}
