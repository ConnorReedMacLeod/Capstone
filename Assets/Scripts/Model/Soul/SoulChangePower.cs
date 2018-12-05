﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulChangePower : Soul {

    int nPowerChange;

    public SoulChangePower(Chr _chrSource, Chr _chrTarget, int _nPowerChange, int _nDuration = -1) : base(_chrSource, _chrTarget) {

        nPowerChange = _nPowerChange;

        sName = "Power: " + nPowerChange.ToString();

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
        chrTarget.ChangeFlatPower(nPowerChange);
        Debug.Log(sName + " has been applied");
    }

    public override void funcOnRemoval() {
        chrTarget.ChangeFlatPower(-nPowerChange);
        Debug.Log(sName + " has been removed");
    }

    public override void funcOnExpiration() {
        Debug.Log(sName + " has expired");
        
    }
}
