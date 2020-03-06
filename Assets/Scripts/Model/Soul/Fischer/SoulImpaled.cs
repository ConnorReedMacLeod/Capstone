using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulImpaled : Soul {

    public int nMaxLifeReduction;
    public LinkedListNode<Property<int>.Modifier> modifierLifeReduction;

    public SoulImpaled(Chr _chrSource, Chr _chrTarget, Action _actSource) : base(_chrSource, _chrTarget, _actSource) {

        sName = "Impaled";

        nMaxLifeReduction = 10;

        bVisible = false;
        bDuration = false;


        lstTriggers = new List<TriggerEffect>(); //no triggers needed

    }

    public int funcHealthReductionModifier (int nCurHealth) { return nCurHealth - nMaxLifeReduction; }

    public override void ApplicationEffect() {
        //Apply a modifier (and save a reference to the modifier node)) to reduce max health by nMaxLifeReduction
        modifierLifeReduction = chrTarget.pnMaxHealth.AddModifier(funcHealthReductionModifier);

        //Then do a check to make sure cur health isn't above max health
        if (chrTarget.nCurHealth > chrTarget.pnMaxHealth.Get()) {
            //If it is, then change the curhealth by 0 (which should catch oversetting curhealth)
            chrTarget.ChangeHealth(0);
        }
    }

    public override void RemoveEffect() {
        chrTarget.pnMaxHealth.RemoveModifier(modifierLifeReduction);
    }

    public SoulImpaled(SoulImpaled other, Chr _chrTarget = null) : base(other) {
        if (_chrTarget != null) {
            //If a Target was provided, then we'll use that
            chrTarget = _chrTarget;
        } else {
            //Otherwise, just copy from the other object
            chrTarget = other.chrTarget;
        }

        nMaxLifeReduction = other.nMaxLifeReduction;

    }
}
