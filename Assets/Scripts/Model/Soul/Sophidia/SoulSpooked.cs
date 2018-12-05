using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulSpooked : Soul {

    int nPowerDebuff;

    public SoulSpooked(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget) {

        sName = "Spooked";

        nPowerDebuff = 10;

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(3);

    }



    public override void funcOnApplication() {
        Debug.Log(sName + " has been applied");
        chrTarget.ChangeFlatPower(-nPowerDebuff);
    }

    public override void funcOnRemoval() {
        Debug.Log(sName + " has been removed");
        chrTarget.ChangeFlatPower(nPowerDebuff);
    }

    public override void funcOnExpiration() {
        Debug.Log(sName + " has expired");
    }
}
