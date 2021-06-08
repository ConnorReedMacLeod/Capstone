using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSlot {

    public Chr chrOwner;
    public Action skill;
    public int nCooldown;


    public void SetSkill(SkillType.SKILLTYPE skilltype) {
        SetSkill(SkillType.InstantiateNewSkill(skilltype, chrOwner));
    }

    public void SetSkill(Action _skill) {
        skill = _skill;
    }


    public void ChangeCooldown(int nDelta) {
        SetCooldown(nCooldown + nDelta);
    }

    public void SetCooldown(int _nCooldown) {
        nCooldown = _nCooldown;

        if(nCooldown < 0) nCooldown = 0;
    }

    public bool IsOffCooldown() {
        return nCooldown == 0;
    }

}
