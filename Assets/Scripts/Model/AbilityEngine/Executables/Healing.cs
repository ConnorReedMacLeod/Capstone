using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing {

    public Chr chrSource;
    public Chr chrTarget;

    public int nBaseHealing;
    public bool bSnapShotPower;

    public LibFunc.Get<int> GetHealing;

    public Healing(Chr _chrSource, Chr _chrTarget, int _nBaseHealing, bool _bSnapshotPower = false) {

        //Copy the fields as they've been passed in
        chrSource = _chrSource;
        chrTarget = _chrTarget;
        nBaseHealing = _nBaseHealing;
        bSnapShotPower = _bSnapshotPower;

        if (bSnapShotPower == true) {
            //If we want to take a snapshot of the source's power as it is now
            //(so that future power changes won't change this healing)
            //then we should use LibFunc's ReturnSnapShot function
            GetHealing = LibFunc.ReturnSnapShot<int>(nBaseHealing + chrSource.pnPower.Get());

        } else {
            //Otherwise, we want to fetch the current source's power whenever the GetDamage()
            //method is called
            GetHealing = () => { return nBaseHealing + chrSource.pnPower.Get(); };
        }

    }


    public Healing(Healing dmgToCopy) {
        //Copy over all the attributes of the original Damage instance
        chrSource = dmgToCopy.chrSource;
        chrTarget = dmgToCopy.chrTarget;
        nBaseHealing = dmgToCopy.nBaseHealing;
        bSnapShotPower = dmgToCopy.bSnapShotPower;

        //We'll also copy over the GetHealing method so we heal in the exact same way as the original
        GetHealing = dmgToCopy.GetHealing;
    }



}
