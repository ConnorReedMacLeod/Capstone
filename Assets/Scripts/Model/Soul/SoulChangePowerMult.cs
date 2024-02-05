using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulChangePowerMult : SoulChr {

    int nPowerMultModifier;

    public LinkedListNode<Property<int>.Modifier> nodePowerMultModifier;

    public SoulChangePowerMult(Chr _chrSource, Chr _chrTarget, Skill _skillSource, int _nPowerMultModifier, int _nDuration = -1) : base(_chrSource, _chrTarget, _skillSource) {

        nPowerMultModifier = _nPowerMultModifier;

        sName = "Power(Mult): " + nPowerMultModifier.ToString();

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

    public SoulChangePowerMult(SoulChangePowerMult other, Chr _chrTarget = null) : base(other, _chrTarget) {

         nPowerMultModifier = other.nPowerMultModifier;

    }

    public override void ApplicationEffect() {
        base.ApplicationEffect();
        //We're adding our flat Power change to whatever the flat Power amount currently is
        nodePowerMultModifier = chrTarget.pnPowerMult.AddModifier((nPowerMultBelow) => this.nPowerMultModifier + nPowerMultBelow);
    }

    public override void RemoveEffect() {
        base.RemoveEffect();
        chrTarget.pnPowerMult.RemoveModifier(nodePowerMultModifier);

    }
}
