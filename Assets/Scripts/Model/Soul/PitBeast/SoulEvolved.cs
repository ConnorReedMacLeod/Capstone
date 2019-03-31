using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulEvolved : Soul {

    int nPowerBuff;

    public SoulChangePower soulChangePower;

    public SoulEvolved(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget) {

        sName = "Evolved";

        nPowerBuff = 5;

        bVisible = false;
        bDuration = false;

        bRecoilWhenApplied = false;

        funcOnApplication = () => {
            //Make a Permanent SoulChangePower, and save a reference to it, so it can be removed later
            soulChangePower = new SoulChangePower(chrSource, chrTarget, nPowerBuff);
            chrTarget.soulContainer.ApplySoul(soulChangePower);
        };

        funcOnRemoval = () => {

            chrTarget.soulContainer.RemoveSoul(soulChangePower);

        };
    }
}
