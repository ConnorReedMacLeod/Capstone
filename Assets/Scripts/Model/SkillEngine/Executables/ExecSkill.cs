using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExecSkill : Executable {

    public Skill skTarget;

    public override bool isLegal() {

        if(skTarget == null) {
            Debug.Log("Executable of type  " + this.GetType().ToString() + " not legal since skTarget is null");
            return false;
        }
        if(skTarget.chrOwner.bDead) {
            Debug.Log("Executable of type  " + this.GetType().ToString() + " not legal since " + skTarget.chrOwner.sName + "(target) is dead");
            return false;
        }
        return base.isLegal();
    }

    public ExecSkill(Chr _chrSource, Skill _skTarget) : base(_chrSource) {
        skTarget = _skTarget;
    }

    public ExecSkill(ExecSkill other) : base(other) {
        skTarget = other.skTarget;
    }

}
