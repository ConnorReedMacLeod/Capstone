using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulImpaled : Soul {

    public int nMaxLifeReduction;
    public LinkedListNode<Property<int>.Modifier> modifierLifeReduction;

    public SoulImpaled(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget) {

        sName = "Impaled";

        nMaxLifeReduction = 10;

        bVisible = false;
        bDuration = false;


        lstTriggers = new List<TriggerEffect>(); //no triggers needed

        funcOnApplication = () => {

            //Apply a modifier (and save a reference to the modifier node)) to reduce max health by 10
            modifierLifeReduction = chrTarget.pnMaxHealth.AddModifier((int nBelow) => (nBelow - 10));

        };

        funcOnRemoval = () => {

            chrTarget.pnMaxHealth.RemoveModifier(modifierLifeReduction);

        };
    }
}
