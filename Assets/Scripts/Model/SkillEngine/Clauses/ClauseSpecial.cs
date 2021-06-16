using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClauseSpecial : Clause {

    public abstract void ClauseEffect();

    public override bool IsSelectable(SelectionSerializer.SelectionInfo selectionInfo) {
        // Any use of a special clause should always be legal
        return true;
    }

    public override bool HasFinalTarget(SelectionSerializer.SelectionInfo selectionInfo) {
        //Special Clauses should always be legal by default
        return true;
    }

    public override void Execute() {

        ClauseEffect();

    }

    public ClauseSpecial(Skill _skill) : base(_skill) {
        targetType = TargetType.SPECIAL;
    }
}
