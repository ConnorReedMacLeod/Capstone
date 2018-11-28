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
        nMaxDuration = 3;

    }



    public override void funcOnApplication() {
        Debug.Log(sName + " has been applied");
        Debug.Log("Should be applying a power debuff to " + chrTarget.sName + " right now");
    }

    public override void funcOnRemoval() {
        Debug.Log(sName + " has been removed");
        Debug.Log("Should be removing the power debuff from " + chrTarget.sName + " right now");
    }

    public override void funcOnExpiration() {
        Debug.Log(sName + " has expired");
    }
}
