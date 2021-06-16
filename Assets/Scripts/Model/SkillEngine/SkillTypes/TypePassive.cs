using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypePassive : TypeSkill {

    public TypePassive(Skill skill) : base(skill) {

    }

    public override string getName() {
        return "Passive";
    }

    public override TYPE Type() {
        return TYPE.PASSIVE;
    }

    public override bool Usable() {
        //Passive's cannot be used

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
