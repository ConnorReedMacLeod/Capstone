using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseChr {

    public Chr chrOwner;

    public BaseChr(Chr _chrOwner) {
        chrOwner = _chrOwner;
    }

    public virtual void SetName() {
        chrOwner.sName = "Default";
    }

    //Can Override this if you want to change base health for a particular character
    public virtual void SetMaxHealth() {
        chrOwner.pnMaxHealth.SetBase(100);
        chrOwner.nCurHealth = chrOwner.pnMaxHealth.Get();
    }

    //TODO - Change this to be integrated with the draft's loadout setup phase
    public abstract void SetLoadoutSkills();

    public abstract void SetDisciplines();


    //Once the actions have been selected, equip them all onto the character
    public void EquipAllLoadoutSkills() {
        for(int i = 0; i < Chr.nStandardCharacterSkills; i++) {
            chrOwner.arSkillSlots[i].SetSkill(chrOwner.arSkillTypesOpeningLoadout[i]);
        }
    }

    public void Init() {

        SetName();
        SetMaxHealth();

        SetDisciplines();

        chrOwner.arSkillTypesOpeningLoadout = new SkillType.SKILLTYPE[Chr.nStandardCharacterSkills];
        SetLoadoutSkills();

        EquipAllLoadoutSkills();

    }
}
