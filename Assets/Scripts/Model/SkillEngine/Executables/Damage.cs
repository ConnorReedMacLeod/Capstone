using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage {

    public Chr chrSource;
    public Chr chrTarget;

    public int nBase;
    public bool bPiercing;

    public delegate int FuncBaseDamage(Chr chrSource, Chr chrTarget);
    public FuncBaseDamage GetBase;
    public LibFunc.Get<int> GetPower;


    //For convenience, allow a constructor that just accepts a number, rather than a function
    public Damage(Chr _chrSource, Chr _chrTarget, int _nBaseDamage, bool _bPiercing = false) {

        //Copy the fields as they've been passed in
        GetBase = (Chr __chrSource, Chr __chrTarget) => _nBaseDamage;

        bPiercing = _bPiercing;

        //Store the chrSource and apply its power buff
        SetChrSource(_chrSource);
        SetChrTarget(_chrTarget);
    }


    public Damage(Chr _chrSource, Chr _chrTarget, FuncBaseDamage _GetBase, bool _bPiercing = false) {

        //Copy the fields as they've been passed in
        GetBase = _GetBase;

        bPiercing = _bPiercing;


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

    public void SetBase(FuncBaseDamage _GetBase) {
        GetBase = _GetBase;
    }

    public Damage(Damage dmgToCopy) {
        //Copy over all the attributes of the original Damage instance
        chrSource = dmgToCopy.chrSource;
        chrTarget = dmgToCopy.chrTarget;

        GetBase = dmgToCopy.GetBase;

        bPiercing = dmgToCopy.bPiercing;

        //Copy the Power fetch method too
        GetPower = dmgToCopy.GetPower;
    }



}
