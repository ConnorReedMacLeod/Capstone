using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExecSoulChr : Executable {

    public SoulChr soulTarget;

    public override bool isLegal() {

        if(soulTarget == null || soulTarget.chrTarget.bDead) {
            Debug.Log("Executable of type  " + this.GetType().ToString() + " not legal since " + soulTarget.chrTarget.sName + "(target) is dead");
            return false;
        }
        return base.isLegal();
    }

    public ExecSoulChr(Chr _chrSource, SoulChr _soulTarget) : base(_chrSource) {
        soulTarget = _soulTarget;
    }

    public ExecSoulChr(ExecSoulChr other) : base(other) {
        soulTarget = other.soulTarget;
    }

}
