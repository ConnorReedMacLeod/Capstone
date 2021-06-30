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
        //Make a Permanent SoulChangePower, and save a reference to it, so it can be removed later
        soulChangePower = new SoulChangePower(chrSource, chrTarget, skillSource, nPowerBuff);
        chrTarget.soulContainer.ApplySoul(soulChangePower);
    }

    public override void RemoveEffect() {

        chrTarget.soulContainer.RemoveSoul(soulChangePower);

    }

    public SoulEvolved(SoulEvolved other, Chr _chrTarget = null) : base(other) {
        if(_chrTarget != null) {
            //If a Target was provided, then we'll use that
            chrTarget = _chrTarget;
        } else {
            //Otherwise, just copy from the other object
            chrTarget = other.chrTarget;
        }

        nPowerBuff = other.nPowerBuff;

    }
}
