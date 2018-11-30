using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulClosingIn : Soul {

    public int nDefenseLoss;

    public SoulClosingIn(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget) {

        sName = "ClosingIn";

        bVisible = false;
        bDuration = true;
        nMaxDuration = 1;

    }

    public override void funcOnApplication() {
        Debug.Log(sName + " has been applied");
        Debug.Log("Should apply a static defense loss debuff");
    }

    public override void funcOnRemoval() {
        Debug.Log(sName + " has been removed");
        Debug.Log("Should remove the static defense loss debuff");
    }

    public override void funcOnExpiration() {
        Debug.Log(sName + " has expired");
    }
}
