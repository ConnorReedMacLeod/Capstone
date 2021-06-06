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

    public abstract void SetInitialSkills();

    public abstract void SetDisciplines();

    public void SetGenericActiveSkills() {
        //Sets the basic generic actions like resting and blocking

        chrOwner.arSkills[Chr.idAdapt] = new ActionRest(chrOwner); //TODO - replace this with an actual adapt action
        chrOwner.arSkills[Chr.idResting] = new ActionRest(chrOwner);
        chrOwner.arSkills[Chr.idBlocking] = new ActionBlock(chrOwner);

    }

    //Just using this temporarily to fill the bench skills with fireballs
    public void FillBenchWithFireballs() {
        for(int i = Chr.nActiveCharacterSkills; i < Chr.nLoadoutSkills; i++) {
            chrOwner.arSkills[i] = new ActionFireball(chrOwner);
        }
    }

    //Once the actions have been selected, equip them all onto the character
    public void EquipAllActions() {
        for(int i = 0; i < Chr.nTotalSkills; i++) {
            chrOwner.SetAction(i, chrOwner.arSkills[i]);
        }
    }

    public void Init() {

        SetName();
        SetMaxHealth();

        SetDisciplines();

        SetInitialSkills();
        FillBenchWithFireballs();
        SetGenericActiveSkills();

        EquipAllActions();

    }
}
