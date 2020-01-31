using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeCantrip : TypeAction {

    public const int nActionPointCost = 0;

    public TypeCantrip(Action act) : base(act) {

    }

    public override string getName() {
        return "Cantrip";
    }

    public override TYPE Type() {
        return TYPE.CANTRIP;
    }

    public override int GetActionPointCost() {
        return nActionPointCost;
    }

    public override void UseAction() {
        //Get the action to push all of its effects onto its stack
        act.Execute();

        PayActionPoints();

        act.PayCooldown();
        act.PayFatigue();

        //Stay in a Ready state for now
    }
}
