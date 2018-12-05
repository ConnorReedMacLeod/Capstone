using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage {

    public Chr chrSource;
    public Chr chrTarget;

    public int nBaseDamage;
    public bool bSnapShotPower;
    public bool bPiercing;

    public LibFunc.Get<int> GetDamage;

    public Damage(Chr _chrSource, Chr _chrTarget, int _nBaseDamage, bool _bSnapshotPower = false, bool _bPiercing = false) {

        //Copy the fields as they've been passed in
        chrSource = _chrSource;
        chrTarget = _chrTarget;
        nBaseDamage = _nBaseDamage;
        bSnapShotPower = _bSnapshotPower;
        bPiercing = _bPiercing;

        if(bSnapShotPower == true) {
            //If we want to take a snapshot of the source's power as it is now
            //(so that future power changes won't change this damage)
            //then we should use LibFunc's ReturnSnapShot function
            GetDamage = LibFunc.ReturnSnapShot<int>(nBaseDamage + chrSource.pnPower.Get());

        } else {
            //Otherwise, we want to fetch the current source's power whenever the GetDamage()
            //method is called
            GetDamage = () => { return nBaseDamage + chrSource.pnPower.Get(); };
        }

    }


    public Damage(Damage dmgToCopy) {
        //Copy over all the attributes of the original Damage instance
        chrSource = dmgToCopy.chrSource;
        chrTarget = dmgToCopy.chrTarget;
        nBaseDamage = dmgToCopy.nBaseDamage;
        bSnapShotPower = dmgToCopy.bSnapShotPower;
        bPiercing = dmgToCopy.bPiercing;

        //We'll also copy over the GetDamage method so we deal damage in the exact same way as the original
        GetDamage = dmgToCopy.GetDamage;
    }



}
