using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulChangeDefense : Soul {

    int nDefenseChange;

    public LinkedListNode<Property<int>.Modifier> nodeDefenseModifier;


    public SoulChangeDefense(Chr _chrSource, Chr _chrTarget, Action _actSource, int _nDefenseChange, int _nDuration = -1) : base(_chrSource, _chrTarget, _actSource) {

        nDefenseChange = _nDefenseChange;

        sName = "Defense: " + nDefenseChange.ToString();

        bVisible = false;
        bRecoilWhenApplied = false;

        bRemoveOnChrDeath = true;

        //Check if a duration was specified
        if(_nDuration == -1) {
            bDuration = false;
        } else {
            bDuration = true;
            pnMaxDuration = new Property<int>(_nDuration);
        }


        lstTriggers = new List<TriggerEffect>();

    }

    public SoulChangeDefense(SoulChangeDefense other, Chr _chrTarget = null) : base(other) {
        if(_chrTarget != null) {
            //If a Target was provided, then we'll use that
            chrTarget = _chrTarget;
        } else {
            //Otherwise, just copy from the other object
            chrTarget = other.chrTarget;
        }

        nDefenseChange = other.nDefenseChange;

    }

    public override void ApplicationEffect() {
        base.ApplicationEffect();
        nodeDefenseModifier = chrTarget.pnDefense.AddModifier((nDefenseBelow) => this.nDefenseChange + nDefenseBelow);

    }

    public override void RemoveEffect() {
        base.RemoveEffect();
        chrTarget.pnDefense.RemoveModifier(nodeDefenseModifier);
    }
}
