using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulFortissimo : Soul {

    int nPowerBuff;
    int nDefenseBuff;

    public SoulFortissimo(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget) {

        sName = "Fortissimo";

        nPowerBuff = 10;
        nDefenseBuff = 10;

        bVisible = true;
        bDuration = true;
        nMaxDuration = 4;

    }



    public override void funcOnApplication() {
        Debug.Log(sName + " has been applied");
        Debug.Log("Should be applying a power and defense buff to " + chrTarget.sName + " right now");
    }

    public override void funcOnRemoval() {
        Debug.Log(sName + " has been removed");
        Debug.Log("Should be removing the power and defense buff from " + chrTarget.sName + " right now");
    }

    public override void funcOnExpiration() {
        Debug.Log(sName + " has expired");
    }
}
