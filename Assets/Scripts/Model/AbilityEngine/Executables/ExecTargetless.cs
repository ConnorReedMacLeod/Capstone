using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExecTargetless : Executable {

    public override void SetTarget() {
        //Don't need to do anything since there's no targets to assign to
    }

    public ExecTargetless() : base() {

    }

    public ExecTargetless(ExecTargetless other) : base(other) {

    }

}
