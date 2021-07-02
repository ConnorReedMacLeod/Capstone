using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulImpaled : SoulChr {

    public int nMaxLifeReduction;
    public LinkedListNode<Property<int>.Modifier> modifierLifeReduction;

    public SoulImpaled(Chr _chrSource, Chr _chrTarget, Skill _skillSource) : base(_chrSource, _chrTarget, _skillSource) {

        sName = "Impaled";

        nMaxLifeReduction = 10;

        bVisible = false;
        bDuration = false;



    }

    public int funcHealthReductionModifier(int nCurHealth) { return nCurHealth - nMaxLifeReduction; }

    public override void ApplicationEffect() {
        //Apply a modifier (and save a reference to the modifier node)) to reduce max health by nMaxLifeReduction
        modifierLifeReduction = chrTarget.pnMaxHealth.AddModifier(funcHealthReductionModifier);

        //Then do a check to make sure cur health isn't above max health
        if(chrTarget.nCurHealth > chrTarget.pnMaxHealth.Get()) {
            //If it is, then change the curhealth by 0 (which should catch oversetting curhealth)
            chrTarget.ChangeHealth(0);
        }
    }

    public override void RemoveEffect() {
        chrTarget.pnMaxHealth.RemoveModifier(modifierLifeReduction);
    }

    public SoulImpaled(SoulImpaled other, Chr _chrTarget = null) : base(other, _chrTarget) {

        nMaxLifeReduction = other.nMaxLifeReduction;

    }
}
