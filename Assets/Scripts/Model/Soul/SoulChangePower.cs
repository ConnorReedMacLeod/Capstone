using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulChangePower : SoulChr {

    int nPowerChange;

    public LinkedListNode<Property<int>.Modifier> nodePowerModifier;

    public SoulChangePower(Chr _chrSource, Chr _chrTarget, Skill _skillSource, int _nPowerChange, int _nDuration = -1) : base(_chrSource, _chrTarget, _skillSource) {

        nPowerChange = _nPowerChange;

        sName = "Power: " + nPowerChange.ToString();

        bVisible = false;
        bRecoilWhenApplied = false;


        //Check if a duration was specified
        if(_nDuration == -1) {
            bDuration = false;
        } else {
            bDuration = true;
            pnMaxDuration = new Property<int>(_nDuration);
        }


    }

    public SoulChangePower(SoulChangePower other, Chr _chrTarget = null) : base(other, _chrTarget) {

        nPowerChange = other.nPowerChange;

    }

    public override void ApplicationEffect() {
        base.ApplicationEffect();
        //We're adding our flat Power change to whatever the flat Power amount currently is
        nodePowerModifier = chrTarget.pnPower.AddModifier((nPowerBelow) => this.nPowerChange + nPowerBelow);
    }

    public override void RemoveEffect() {
        base.RemoveEffect();
        chrTarget.pnPower.RemoveModifier(nodePowerModifier);

    }
}
