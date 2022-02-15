using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSlot {

    public int iSlot;
    public Chr chrOwner;
    public Skill skill;
    public int nCooldown;


    public void SetSkill(SkillType.SKILLTYPE skilltype) {
        SetSkill(SkillType.InstantiateNewSkill(skilltype, chrOwner));
    }

    public void SetSkill(Skill _skill) {

        if(skill != null) {
            skill.OnUnequip();
            skill.skillslot = null;
        }

        skill = _skill;
        skill.skillslot = this;

        if(skill != null) {
            skill.OnEquip();
        }

        //Debug.Log("Skill in slot " + iSlot + " has been set to " + skill.sDisplayName);
        skill.subSkillChange.NotifyObs();
        //TODO - if a skill transforms while you're hovering over it, the tooltip doesn't instantly update to match the new skill description
    }


    public void ChangeCooldown(int nDelta) {
        SetCooldown(nCooldown + nDelta);
    }

    public void SetCooldown(int _nCooldown) {
        nCooldown = _nCooldown;

        if(nCooldown < 0) nCooldown = 0;

        skill.subSkillChange.NotifyObs();
    }

    public bool IsOffCooldown() {
        return nCooldown == 0;
    }

    public SkillSlot(Chr _chrOwner, int _iSlot) {
        chrOwner = _chrOwner;
        iSlot = _iSlot;

        nCooldown = 0;
    }
}
