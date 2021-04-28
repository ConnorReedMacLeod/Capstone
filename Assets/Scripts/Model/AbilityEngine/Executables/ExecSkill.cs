using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExecSkill : Executable {

    public Action skTarget;

    public override bool isLegal() {

        if(skTarget == null) {
            Debug.Log("Executable of type  " + this.GetType().ToString() + " not legal since skTarget is null");
            return false;
        }
        if(skTarget.chrSource.bDead) {
            Debug.Log("Executable of type  " + this.GetType().ToString() + " not legal since " + skTarget.chrSource.sName + "(target) is dead");
            return false;
        }
        return base.isLegal();
    }

    public ExecSkill(Chr _chrSource, Action _skTarget) : base(_chrSource) {
        skTarget = _skTarget;
    }

    public ExecSkill(ExecSkill other) : base(other) {
        skTarget = other.skTarget;
    }

}
