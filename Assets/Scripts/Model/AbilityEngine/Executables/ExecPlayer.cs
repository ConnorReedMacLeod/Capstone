using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExecPlayer : Executable {

    public Player plyrTarget;

    public delegate Player Targetter();

    public Targetter GetPlayerTarget;

    public override bool isLegal() {


        return base.isLegal();
    }

    public override void SetTarget() {
        //Just call whatever custom function has been given as the method for selecting our target
        plyrTarget = GetPlayerTarget();
    }

    public ExecPlayer() : base() {

    }

    public ExecPlayer(ExecPlayer other) : base(other) {
        plyrTarget = other.plyrTarget;
        GetPlayerTarget = other.GetPlayerTarget;
    }

}
