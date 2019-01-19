using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulSpooked : Soul {

    int nPowerDebuff;

    public SoulChangePower soulChangePower;

    public SoulSpooked(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget) {

        sName = "Spooked";

        nPowerDebuff = -10;

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(3);

        funcOnApplication = () => {
            //Make a Permanent SoulChangePower, and save a reference to it, so it can be removed later
            soulChangePower = new SoulChangePower(chrSource, chrTarget, nPowerDebuff);
            chrTarget.soulContainer.ApplySoul(soulChangePower);
        };

        funcOnRemoval = () => {
            chrTarget.soulContainer.RemoveSoul(soulChangePower);
        };
    }
}
