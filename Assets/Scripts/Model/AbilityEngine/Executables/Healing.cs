using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing {

    public Chr chrSource;
    public Chr chrTarget;

    public int nBaseHealing;
    public LibFunc.Get<int> GetPower;

    public Healing(Chr _chrSource, Chr _chrTarget, int _nBaseHealing) {

        //Copy the fields as they've been passed in
        nBaseHealing = _nBaseHealing;

        //Store the chrSource and apply its power buff
        SetChrSource(_chrSource);
        SetChrTarget(_chrTarget);

    }

    public int Get() {
        return nBaseHealing + GetPower();
    }

    public void SnapShotPower() {

        //If we need to snapshot, then fetch and fix the power in the GetPower function
        int nSnapshotPower = chrSource.pnPower.Get();
        GetPower = () => nBaseHealing + nSnapshotPower;

    }

    public void SetChrSource(Chr _chrSource) {

        chrSource = _chrSource;

        //Set the GetPower function to fetch the chrSource's current power
        GetPower = () => chrSource.pnPower.Get();
    }

    public void SetChrTarget(Chr _chrTarget) {

        chrTarget = _chrTarget;

    }

    public Healing(Healing dmgToCopy) {
        //Copy over all the attributes of the original Damage instance
        chrSource = dmgToCopy.chrSource;
        chrTarget = dmgToCopy.chrTarget;

        nBaseHealing = dmgToCopy.nBaseHealing;

        //Copy the Power fetch method too
        GetPower = dmgToCopy.GetPower;
    }



}
