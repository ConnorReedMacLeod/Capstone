using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulCloudCushion : Soul {
   
    public int nDefenseBuff;
    
    public SoulChangeDefense soulChangeDefense;

    public SoulCloudCushion(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget) {

        sName = "CloudCushion";

        nDefenseBuff = 25;

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(4);

    }


    public override void funcOnApplication() {

        //Make a Permanent SoulChangeDefense, and save a reference to it, so it can be removed later
        soulChangeDefense = new SoulChangeDefense(chrSource, chrTarget, nDefenseBuff);
        chrTarget.soulContainer.ApplySoul(soulChangeDefense);

    }

    public override void funcOnRemoval() {

        chrTarget.soulContainer.RemoveSoul(soulChangeDefense);

    }

}
