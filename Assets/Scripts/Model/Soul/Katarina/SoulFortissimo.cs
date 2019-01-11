using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulFortissimo : Soul {

    public int nPowerBuff;
    public int nDefenseBuff;

    public SoulChangePower soulChangePower;
    public SoulChangeDefense soulChangeDefense;

    public SoulFortissimo(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget) {

        sName = "Fortissimo";

        nPowerBuff = 10;
        nDefenseBuff = 10;

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(4);

        funcOnApplication = () => {

            //Make a Permanent SoulChangePower, and save a reference to it, so it can be removed later
            soulChangePower = new SoulChangePower(chrSource, chrTarget, nPowerBuff);
            chrTarget.soulContainer.ApplySoul(soulChangePower);

            //Make a Permanent SoulChangeDefense, and save a reference to it, so it can be removed later
            soulChangeDefense = new SoulChangeDefense(chrSource, chrTarget, nDefenseBuff);
            chrTarget.soulContainer.ApplySoul(soulChangeDefense);
        };

        funcOnRemoval = () => {

            chrTarget.soulContainer.RemoveSoul(soulChangePower);
            chrTarget.soulContainer.RemoveSoul(soulChangeDefense);
        };

    }
}
