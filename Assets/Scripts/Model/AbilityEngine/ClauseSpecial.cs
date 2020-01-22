using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClauseSpecial : Clause {

    public override void Execute() {

        for (int j = 0; j < lstExec.Count; j++) {

            //Make a new copy of the mold of the current executable
            Executable execCopy = lstExec[j].MakeCopy();

            //Ensure its targetting information is properly filled out
            execCopy.SetTarget();

            //Push the new copy onto the stack
            ContAbilityEngine.Get().AddExec(execCopy);
        }

    }

    public ClauseSpecial(Action _action): base(_action) {

    }

    public ClauseSpecial(ClauseSpecial other): base(other) {

    }

    public override Clause MakeCopy() {
        return new ClauseSpecial(this);
    }
}
