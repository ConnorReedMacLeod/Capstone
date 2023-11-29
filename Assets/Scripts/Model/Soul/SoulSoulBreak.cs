using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulSoulBreak : SoulChr {

    int nPowerModifier;
    int nDefenseModifier;

    public LinkedListNode<Property<int>.Modifier> nodePowerModifier;
    public LinkedListNode<Property<int>.Modifier> nodeDefenseModifier;

    public SoulSoulBreak(Chr _chrSource, Chr _chrTarget, Skill _skillSource, int _nPowerModifier = Match.NSOULBREAKPOWERMODIFIER, int _nDefenseModifier = Match.NSOULBREAKDEFENSEMODIFIER, int _nDuration = Match.NSOULBREAKDURATION) : base(_chrSource, _chrTarget, _skillSource) {

        nPowerModifier = _nPowerModifier;
        nDefenseModifier = _nDefenseModifier;

        sName = string.Format("Soulbreak: +{0}% Power / {1}% Defense", nPowerModifier, nDefenseModifier);

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

    public SoulSoulBreak(SoulSoulBreak other, Chr _chrTarget = null) : base(other, _chrTarget) {

        nPowerModifier = other.nPowerModifier;
        nDefenseModifier = other.nDefenseModifier;

    }


    public override void ApplicationEffect() {
        base.ApplicationEffect();
        //We're adding the multiplicative Power modifier to whatever existing multiplicative Power already exists
        nodePowerModifier = chrTarget.pnPowerMult.AddModifier((nPowerBelow) => this.nPowerModifier + nPowerBelow);
        nodeDefenseModifier = chrTarget.pnDefenseMult.AddModifier((nDefenseBelow) => this.nDefenseModifier + nDefenseBelow);

        //Update our character's soulbreak reference to this
        chrTarget.soulSoulBreak = this;
        chrTarget.subSoulbreakChanged.NotifyObs();

    }

    public override void RemoveEffect() {

        //Clear out our soulbreak reference
        chrTarget.soulSoulBreak = null;
        chrTarget.subSoulbreakChanged.NotifyObs();

        chrTarget.pnPowerMult.RemoveModifier(nodePowerModifier);
        chrTarget.pnDefenseMult.RemoveModifier(nodeDefenseModifier);

        base.RemoveEffect();

        //After soulbreak ends, we can clear out all of our soul
        chrTarget.soulContainer.RemoveAllVisibleSoul();
    }
    
}
