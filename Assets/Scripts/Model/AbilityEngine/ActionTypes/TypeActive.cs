using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeActive : TypeAction {

    public const int nActionPointCost = 1;

    public TypeActive(Action act) : base(act) {

    }

    public override string getName() {
        return "Active";
    }

    public override TYPE Type() {
        return TYPE.ACTIVE;
    }
    public override int GetActionPointCost() {
        return nActionPointCost;
    }

    public override void UseAction(int[] lstTargettingIndices) {

        //Then give that action's stack of clauses to the Ability Engine to process
        ContAbilityEngine.PushClauses(act.lstClauses);

        PayActionPoints();

        act.PayCooldown();
        act.PayFatigue();

        //Stay in a Ready state for now
    }
}
