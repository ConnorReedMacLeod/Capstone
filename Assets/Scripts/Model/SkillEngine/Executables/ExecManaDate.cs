using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExecManaDate : Executable {

    public ManaDate manadateTarget;

    public override bool isLegal() {

        if (manadateTarget == null) {
            Debug.Log("Executable of type " + this.GetType().ToString() + " not legal since manadateTarget is null");
        }
        return base.isLegal();
    }

    public ExecManaDate(Chr _chrSource, ManaDate _manadateTarget) : base(_chrSource) {
        manadateTarget = _manadateTarget;
    }

    public ExecManaDate(ExecManaDate other) : base(other) {
        manadateTarget = other.manadateTarget;

    }

}
