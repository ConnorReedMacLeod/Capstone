using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulSpooked : SoulChr {

    public int nPowerDebuff;

    public SoulChangePower soulChangePower;

    public SoulSpooked(Chr _chrSource, Chr _chrTarget, Skill _skillSource) : base(_chrSource, _chrTarget, _skillSource) {

        sName = "Spooked";

        nPowerDebuff = -10;

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(3);

    }

    public override void ApplicationEffect() {
        //Make a Permanent SoulChangePower, and save a reference to it, so it can be removed later
        soulChangePower = new SoulChangePower(chrSource, chrTarget, skillSource, nPowerDebuff);
        chrTarget.soulContainer.ApplySoul(soulChangePower);
    }

    public override void RemoveEffect() {
        chrTarget.soulContainer.RemoveSoul(soulChangePower);
    }

    public SoulSpooked(SoulSpooked other, Chr _chrTarget = null) : base(other) {
        if(_chrTarget != null) {
            //If a Target was provided, then we'll use that
            chrTarget = _chrTarget;
        } else {
            //Otherwise, just copy from the other object
            chrTarget = other.chrTarget;
        }

        nPowerDebuff = other.nPowerDebuff;

    }
}
