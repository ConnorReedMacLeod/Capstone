using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulImpaled : Soul {

    public int nMaxLifeReduction;

    public SoulImpaled(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget) {

        sName = "Impaled";

        nMaxLifeReduction = 10;

        bVisible = false;
        bDuration = false;


        lstTriggers = new List<TriggerEffect>(); //no triggers needed
    }

    public override void funcOnApplication() {
        Debug.Log(sName + " has been applied");
        Debug.Log("Should be applying a max health reduction static effect");
    }

    public override void funcOnRemoval() {
        Debug.Log(sName + " has been removed");
        Debug.Log("Should be removing the max health reduction static effect");
    }

    public override void funcOnExpiration() {
        Debug.Log(sName + " has expired");
    }
}
