using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExecChr : Executable {

    public Chr chrTarget;

    public override bool isLegal() { 

        if (chrTarget == null || chrTarget.bDead) {
            Debug.Log("Executable of type  " + this.GetType().ToString() + " not legal since " + chrTarget.sName + "(target) is dead");
            return false;
        }
        return base.isLegal();
    }

    public ExecChr(Chr _chrSource, Chr _chrTarget) : base(_chrSource) {
        chrTarget = _chrTarget;
    }

    public ExecChr(ExecChr other) : base (other){
        chrTarget = other.chrTarget;
    }

}
