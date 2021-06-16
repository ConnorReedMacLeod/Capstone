using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeCantrip : TypeSkill {

    public const int nSkillPointCost = 0;

    public TypeCantrip(Skill skill) : base(skill) {

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
