using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeUsageActive : TypeUsage {

    public const int nSkillPointCost = 1;

    public TypeUsageActive(Skill skill) : base(skill) {

    }

    public override string getName() {
        return "Active";
    }

    public override TYPE Type() {
        return TYPE.ACTIVE;
    }
    public override int GetSkillPointCost() {
        return nSkillPointCost;
    }

}
