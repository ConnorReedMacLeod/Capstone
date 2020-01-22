using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExecChr : Executable {

    public Chr chrTarget;

    public delegate Chr Targetter();

    public Targetter GetChrTarget; 

    public override bool isLegal() { 

        if (chrTarget == null || chrTarget.bDead) {
            Debug.Log("Executable of type  " + this.GetType().ToString() + " not legal since " + chrTarget.sName + "(target) is dead");
            return false;
        }
        return base.isLegal();
    }

    public override void SetTarget() {
        //Just call whatever custom function has been given as the method for selecting our target
        chrTarget = GetChrTarget();
    }

    public ExecChr() : base() {

    }

    public ExecChr(ExecChr other) : base (other){
        chrTarget = other.chrTarget;
        GetChrTarget = other.GetChrTarget;
    }

}
