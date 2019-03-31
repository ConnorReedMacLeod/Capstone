using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulChangePower : Soul {

    int nPowerChange;

    public LinkedListNode<Property<int>.Modifier> nodePowerModifier;

    public SoulChangePower(Chr _chrSource, Chr _chrTarget, int _nPowerChange, int _nDuration = -1) : base(_chrSource, _chrTarget) {

        nPowerChange = _nPowerChange;

        sName = "Power: " + nPowerChange.ToString();

        bVisible = false;
        bRecoilWhenApplied = false;
        bRemoveOnChrDeath = true;


        //Check if a duration was specified
        if (_nDuration == -1) {
            bDuration = false;
        } else {
            bDuration = true;
            pnMaxDuration = new Property<int>(_nDuration);
        }


        lstTriggers = new List<TriggerEffect>();

        funcOnApplication = () => {
            nodePowerModifier = chrTarget.pnPower.AddModifier((nPowerBelow) => this.nPowerChange + nPowerBelow);
        };

        funcOnRemoval = () => {
            chrTarget.pnPower.RemoveModifier(nodePowerModifier);
        };
    }
}
