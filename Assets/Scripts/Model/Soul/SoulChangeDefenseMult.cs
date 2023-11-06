using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulChangeDefenseMult : SoulChr {

    int nDefenseMultModifier;

    public LinkedListNode<Property<int>.Modifier> nodeDefenseMultModifier;

    public SoulChangeDefenseMult(Chr _chrSource, Chr _chrTarget, Skill _skillSource, int _nDefenseMultModifier, int _nDuration = -1) : base(_chrSource, _chrTarget, _skillSource) {

        nDefenseMultModifier = _nDefenseMultModifier;

        sName = "Defense(Mult): " + nDefenseMultModifier.ToString();

        bVisible = false;
        bRecoilWhenApplied = false;


        //Check if a duration was specified
        if (_nDuration == -1) {
            bDuration = false;
        } else {
            bDuration = true;
            pnMaxDuration = new Property<int>(_nDuration);
        }


    }

    public SoulChangeDefenseMult(SoulChangeDefenseMult other, Chr _chrTarget = null) : base(other, _chrTarget) {

        nDefenseMultModifier = other.nDefenseMultModifier;

    }

    public override void ApplicationEffect() {
        base.ApplicationEffect();
        //We're adding our flat Power change to whatever the flat Power amount currently is
        nodeDefenseMultModifier = chrTarget.pnDefenseMult.AddModifier((nDefenseMultBelow) => this.nDefenseMultModifier + nDefenseMultBelow);
    }

    public override void RemoveEffect() {
        base.RemoveEffect();
        chrTarget.pnDefenseMult.RemoveModifier(nodeDefenseMultModifier);

    }
}
