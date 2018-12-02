using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulEvolved : Soul {

    int nPowerBuff;

    public SoulEvolved(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget) {

        sName = "Evolved";

        nPowerBuff = 5;

        bVisible = false;
        bDuration = false;

    }



    public override void funcOnApplication() {
        Debug.Log(sName + " has been applied");
        chrTarget.ChangeFlatPower(nPowerBuff);

    }

    public override void funcOnRemoval() {
        Debug.Log(sName + " has been removed");
        chrTarget.ChangeFlatPower(-nPowerBuff);
    }

    public override void funcOnExpiration() {
        Debug.Log(sName + " has expired");
    }
}
