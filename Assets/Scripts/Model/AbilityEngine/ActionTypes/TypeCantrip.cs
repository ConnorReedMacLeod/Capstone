using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeCantrip : TypeAction {

    public const int nActionPointCost = 0;

    public TypeCantrip(Action act) : base(act) {

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

        //Stay in a Ready state for now
    }
}
