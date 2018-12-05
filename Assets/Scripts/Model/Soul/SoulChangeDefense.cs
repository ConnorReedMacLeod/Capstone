using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulChangeDefense : Soul {

    int nDefenseChange;

    public SoulChangeDefense(Chr _chrSource, Chr _chrTarget, int _nDefenseChange, int _nDuration = -1) : base(_chrSource, _chrTarget) {

        nDefenseChange = _nDefenseChange;

        sName = "Defense: " + nDefenseChange.ToString();

        bVisible = false;

        //Check if a duration was specified
        if (_nDuration == -1) {
            bDuration = false;
        } else {
            bDuration = true;
            pnMaxDuration = new Property<int>(_nDuration);
        }


        lstTriggers = new List<TriggerEffect>();
    }

    public override void funcOnApplication() {
        chrTarget.ChangeFlatDefense(nDefenseChange);
        Debug.Log(sName + " has been applied");
    }

    public override void funcOnRemoval() {
        chrTarget.ChangeFlatDefense(-nDefenseChange);
        Debug.Log(sName + " has been removed");
    }

    public override void funcOnExpiration() {
        Debug.Log(sName + " has expired");

    }
}
