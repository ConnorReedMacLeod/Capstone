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

    public override void UseAction() {
        //Get the action to push all of its effects onto its stack
        act.Execute();

        //Then give that action's stack of clauses to the Ability Engine to process
        ContAbilityEngine.AddClauseStack(ref act.stackClauses);

        PayActionPoints();

        act.PayCooldown();
        act.PayFatigue();

        //Stay in a Ready state for now
    }
}
