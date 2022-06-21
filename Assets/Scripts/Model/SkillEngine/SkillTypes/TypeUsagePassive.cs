using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeUsagePassive : TypeUsage {

    public TypeUsagePassive(Skill skill) : base(skill) {

    }

    public override string getName() {
        return "Passive";
    }

    public override TYPE Type() {
        return TYPE.PASSIVE;
    }

    public override bool Usable() {
        //Passives cannot be used

        return false;
    }

    public override bool IsDefaultHidden() {
        //Passives can't be used, and therefore, are never hidden
        return false;
    }

    public override int GetSkillPointCost() {
        Debug.Log("Shouldn't be getting the skill point cost of a passive");
        return 0;
    }

    public override void UseSkill() {

        Debug.LogError("Can't use a passive skill");

    }
}
