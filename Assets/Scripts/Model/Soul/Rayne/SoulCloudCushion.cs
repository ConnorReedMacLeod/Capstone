using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulCloudCushion : Soul {
   
    public int nDefenseBuff;
    
    public SoulChangeDefense soulChangeDefense;

    public SoulCloudCushion(Chr _chrSource, Chr _chrTarget, Action _actSource) : base(_chrSource, _chrTarget, _actSource) {

        sName = "CloudCushion";

        nDefenseBuff = 25;

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(4);

        funcOnApplication = () => {
            //Make a Permanent SoulChangeDefense, and save a reference to it, so it can be removed later
            soulChangeDefense = new SoulChangeDefense(chrSource, chrTarget, actSource, nDefenseBuff);
            chrTarget.soulContainer.ApplySoul(soulChangeDefense);
        };

        funcOnRemoval = () => {
            chrTarget.soulContainer.RemoveSoul(soulChangeDefense);
        };
    }
}
