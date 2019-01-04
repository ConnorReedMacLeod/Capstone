﻿using System.Collections;
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
    }

    public override void funcOnApplication() {
        Debug.Log(sName + " has been applied");
        //Apply a modifier (and save a reference to the modifier node)) to reduce max health by 10
        modifierLifeReduction = chrTarget.pnMaxHealth.AddModifier((int nBelow) => (nBelow - 10));
    }

    public override void funcOnRemoval() {
        Debug.Log(sName + " has been removed");

        chrTarget.pnMaxHealth.RemoveModifier(modifierLifeReduction);
    }

    public override void funcOnExpiration() {
        Debug.Log(sName + " has expired");
    }
}