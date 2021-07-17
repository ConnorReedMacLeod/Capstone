using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExecSoulPosition : Executable {

    public SoulPosition soulTarget;

    public override bool isLegal() {

        if(soulTarget == null) {
            Debug.Log("Executable of type  " + this.GetType().ToString() + " not legal since " + soulTarget + "(target) is null");
            return false;
        }
        return base.isLegal();
    }

    public ExecSoulPosition(Chr _chrSource, SoulPosition _soulTarget) : base(_chrSource) {
        soulTarget = _soulTarget;
    }

    public ExecSoulPosition(ExecSoulPosition other) : base(other) {
        soulTarget = other.soulTarget;
    }

}
