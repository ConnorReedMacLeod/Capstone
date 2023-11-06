using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulCloudCushion : SoulChr {

    public int nDefenseBuff;

    public SoulChangeDefense soulChangeDefense;

    public SoulCloudCushion(Chr _chrSource, Chr _chrTarget, Skill _skillSource) : base(_chrSource, _chrTarget, _skillSource) {

        sName = "CloudCushion";

        nDefenseBuff = 25;

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(4);

    }

    public override void ApplicationEffect() {
        base.ApplicationEffect();
        //Make a Permanent SoulChangeDefense, and save a reference to it, so it can be removed later
        soulChangeDefense = new SoulChangeDefense(chrSource, chrTarget, skillSource, nDefenseBuff);
        chrTarget.soulContainer.ApplySoul(soulChangeDefense);
    }

    public override void RemoveEffect() {
        base.RemoveEffect();
        chrTarget.soulContainer.RemoveSoul(soulChangeDefense);
    }

    public SoulCloudCushion(SoulCloudCushion other, Chr _chrTarget = null) : base(other, _chrTarget) {

        nDefenseBuff = other.nDefenseBuff;

    }
}
