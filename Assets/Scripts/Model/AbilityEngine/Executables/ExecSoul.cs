using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExecSoul : Executable {

    public Soul soulTarget;

    public delegate Soul Targetter();

    public Targetter GetSoulTarget;

    public override bool isLegal() {

        if (soulTarget == null || soulTarget.chrTarget.bDead) {
            Debug.Log("Executable of type  " + this.GetType().ToString() + " not legal since " + soulTarget.chrTarget.sName + "(target) is dead");
            return false;
        }
        return base.isLegal();
    }

    public override void SetTarget() {
        //Just call whatever custom function has been given as the method for selecting our target
        soulTarget = GetSoulTarget();
    }

    public ExecSoul() : base() {

    }

    public ExecSoul(ExecSoul other) : base(other) {
        soulTarget = other.soulTarget;
        GetSoulTarget = other.GetSoulTarget;
    }

}
