using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing {

    public Chr chrSource;
    public Chr chrTarget;

    public delegate int FuncBaseHeal(Chr chrSource, Chr chrTarget);
    public FuncBaseHeal GetBase;
    public LibFunc.Get<int> GetPower;

    //For convenience, allow a constructor that just accepts a number, rather than a function
    public Healing(Chr _chrSource, Chr _chrTarget, int _nBase) {

        //If a simple number is provided, then don't have GetBase depend on consume ChrSource/ChrTarget
        GetBase = (Chr __chrSource, Chr __chrTarget) => _nBase;

        //Store the chrSource and apply its power buff
        SetChrSource(_chrSource);
        SetChrTarget(_chrTarget);
    }

    public Healing(Chr _chrSource, Chr _chrTarget, FuncBaseHeal _GetBase) {

        //Copy the fields as they've been passed in
        GetBase = _GetBase;

        //Store the chrSource and apply its power buff
        SetChrSource(_chrSource);
        SetChrTarget(_chrTarget);
    }

    public int Get() {
        return GetBase(chrSource, chrTarget) + GetPower();
    }

    public void SnapShotPower() {

        //If we need to snapshot, then fetch and fix the power in the GetPower function
        int nSnapshotPower = chrSource.pnPower.Get();
        GetPower = () => nSnapshotPower;

    }

    public void SetChrSource(Chr _chrSource) {

        chrSource = _chrSource;

        //Set the GetPower function to fetch the chrSource's current power
        GetPower = () => chrSource.pnPower.Get();
    }

    public void SetChrTarget(Chr _chrTarget) {

        chrTarget = _chrTarget;

    }

    public Healing(Healing healToCopy) {
        //Copy over all the attributes of the original Healing instance
        chrSource = healToCopy.chrSource;
        chrTarget = healToCopy.chrTarget;

        GetBase = healToCopy.GetBase;

        //Copy the Power fetch method too
        GetPower = healToCopy.GetPower;
    }



}
