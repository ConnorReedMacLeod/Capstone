using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulFortissimo : SoulChr {

    public int nPowerBuff;
    public int nDefenseBuff;

    public SoulChangePower soulChangePower;
    public SoulChangeDefense soulChangeDefense;

    public SoulFortissimo(Chr _chrSource, Chr _chrTarget, Skill _skillSource) : base(_chrSource, _chrTarget, _skillSource) {

        sName = "Fortissimo";

        skillSource = _skillSource;

        nPowerBuff = 10;
        nDefenseBuff = 10;

        bVisible = true;
        bDuration = true;
        bRecoilWhenApplied = false;

        pnMaxDuration = new Property<int>(4);

    }

    public override void ApplicationEffect() {
        //Make a Permanent SoulChangePower, and save a reference to it, so it can be removed later
        soulChangePower = new SoulChangePower(chrSource, chrTarget, skillSource, nPowerBuff);
        chrTarget.soulContainer.ApplySoul(soulChangePower);

        //Make a Permanent SoulChangeDefense, and save a reference to it, so it can be removed later
        soulChangeDefense = new SoulChangeDefense(chrSource, chrTarget, skillSource, nDefenseBuff);
        chrTarget.soulContainer.ApplySoul(soulChangeDefense);
    }

    public override void RemoveEffect() {

        chrTarget.soulContainer.RemoveSoul(soulChangePower);
        chrTarget.soulContainer.RemoveSoul(soulChangeDefense);

    }

    public SoulFortissimo(SoulFortissimo other, Chr _chrTarget = null) : base(other, _chrTarget) {

        nPowerBuff = other.nPowerBuff;
        nDefenseBuff = other.nDefenseBuff;

    }
}
