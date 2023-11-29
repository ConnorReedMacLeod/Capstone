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
    public LibFunc.Get<int> GetPowerMult;


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


        //Store the chrSource and register the Power buff grabbing functions
        SetChrSource(_chrSource);
        SetChrTarget(_chrTarget);

    }


    //Calculate the final damage based on the base damage method, and any Power/Defense modifiers
    public int Get() {

        int nDamageToApply;
        if (bPiercing || chrTarget == null) {
            //If we pierce through all defenses, or if we have no target whose defense we can apply, then we'll just calculate our raw offensive damage
            nDamageToApply = DamageWithNoDefense();
        } else {
            nDamageToApply = DamageWithAllModifiers();
        }

        nDamageToApply = Mathf.Max(0, nDamageToApply); //Ensure we don't produce a negative amount of damage

        //Note - we're returning an int here, so we may be rounding fractional damage values before changing our health amount
        return nDamageToApply;
    }

    public int DamageWithAllModifiers() {
        //Debug.LogFormat("0.01f * (100 + {0} - {1}) * {2} + ({3} - {4})", GetPowerMult(), chrTarget.pnDefenseMult.Get(), GetBase(chrSource, chrTarget),
        //    GetPower(), chrTarget.pnDefense.Get());
        return Mathf.RoundToInt(0.01f * (100 + GetPowerMult() - chrTarget.pnDefenseMult.Get()) * GetBase(chrSource, chrTarget) + (GetPower() - chrTarget.pnDefense.Get()));
    }

    //Calculate the outgoing damage with Power, but with no Defense modifiers
    public int DamageWithNoDefense() {
        return Mathf.RoundToInt(0.01f * (100 + GetPowerMult()) * GetBase(chrSource, chrTarget) + GetPower());
    }

    public void SnapShotPower() {

        //If we need to snapshot, then fetch and create a new function that just returns the Power (and PowerMult) as it is now
        int nSnapshotPower = chrSource.pnPower.Get();
        GetPower = () => nSnapshotPower;

        int nSnapshotPowerMult = chrSource.pnPowerMult.Get();
        GetPowerMult = () => nSnapshotPowerMult;

    }

    public void SetChrSource(Chr _chrSource) {

        chrSource = _chrSource;

        //Set the GetPower and GetPowerMult functions to fetch the chrSource's current power at the time of calling the lambda
        GetPower = () => chrSource.pnPower.Get();
        GetPowerMult = () => chrSource.pnPowerMult.Get();
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
        GetPowerMult = dmgToCopy.GetPowerMult;

    }



}
