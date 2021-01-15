using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClauseSpecial : Clause {

    public abstract void ClauseEffect();

    public override bool IsValidSelection(SelectionSerializer.SelectionInfo selectionInfo) {
        // Any use of a special clause should always be legal
        return true;
    }

    public override void Execute() {

        ClauseEffect();

    }

    public ClauseSpecial(Action _action) : base(_action) {
        targetType = TargetType.SPECIAL;
    }
}
