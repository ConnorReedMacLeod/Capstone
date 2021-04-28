using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExecPlayer : Executable {

    public Player plyrTarget;

    public override bool isLegal() {


        return base.isLegal();
    }

    public ExecPlayer(Chr _chrSource, Player _plyrTarget) : base(_chrSource) {
        plyrTarget = _plyrTarget;
    }

    public ExecPlayer(ExecPlayer other) : base(other) {
        plyrTarget = other.plyrTarget;
    }

}
