using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExecSkill  : Executable {

    public Action skTarget;

    public delegate Action Targetter();

    public Targetter GetSkillTarget;

    public override bool isLegal() {

        if (skTarget == null || skTarget.chrSource.bDead) {
            Debug.Log("Executable of type  " + this.GetType().ToString() + " not legal since " + skTarget.chrSource.sName + "(target) is dead");
            return false;
        }
        return base.isLegal();
    }

    public override void SetTarget() {
        //Just call whatever custom function has been given as the method for selecting our target
        skTarget = GetSkillTarget();
    }

    public ExecSkill() : base() {

    }

    public ExecSkill(ExecSkill other) : base(other) {
        skTarget = other.skTarget;
        GetSkillTarget = other.GetSkillTarget;
    }

}
