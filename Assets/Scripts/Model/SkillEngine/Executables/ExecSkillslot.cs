using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExecSkillslot : Executable {

    public SkillSlot ssTarget;

    public override bool isLegal() {

        if(ssTarget == null) {
            Debug.Log("Executable of type  " + this.GetType().ToString() + " not legal since ssTarget is null");
            return false;
        }
        if(ssTarget.chrOwner.bDead) {
            Debug.Log("Executable of type  " + this.GetType().ToString() + " not legal since " + ssTarget.chrOwner.sName + "(target) is dead");
            return false;
        }
        return base.isLegal();
    }

    public ExecSkillslot(Chr _chrSource, SkillSlot _ssTarget) : base(_chrSource) {
        ssTarget = _ssTarget;
    }

    public ExecSkillslot(ExecSkillslot other) : base(other) {
        ssTarget = other.ssTarget;
    }

}
