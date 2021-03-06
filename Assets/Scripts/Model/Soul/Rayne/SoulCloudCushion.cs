﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulCloudCushion : Soul {

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
        //Make a Permanent SoulChangeDefense, and save a reference to it, so it can be removed later
        soulChangeDefense = new SoulChangeDefense(chrSource, chrTarget, skillSource, nDefenseBuff);
        chrTarget.soulContainer.ApplySoul(soulChangeDefense);
    }

    public override void RemoveEffect() {
        chrTarget.soulContainer.RemoveSoul(soulChangeDefense);
    }

    public SoulCloudCushion(SoulCloudCushion other, Chr _chrTarget = null) : base(other) {
        if(_chrTarget != null) {
            //If a Target was provided, then we'll use that
            chrTarget = _chrTarget;
        } else {
            //Otherwise, just copy from the other object
            chrTarget = other.chrTarget;
        }

        nDefenseBuff = other.nDefenseBuff;

    }
}
