using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeUsageCantrip : TypeUsage {

    public const int nSkillPointCost = 0;

    public TypeUsageCantrip(Skill skill) : base(skill) {

    }

    public override string getName() {
        return "Cantrip";
    }

    public override TYPE Type() {
        return TYPE.CANTRIP;
    }

    public override int GetSkillPointCost() {
        return nSkillPointCost;
    }

}
