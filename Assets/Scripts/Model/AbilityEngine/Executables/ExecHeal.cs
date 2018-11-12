using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Can create executables like ...= new Exec(){chrTarget = ..., nDamage = ...};

public class ExecHeal : Executable {

    public Chr chrTarget;
    public int nAmount;

    public override void Execute() {
        //TODO:: Take into account armour and power/defense
        chrTarget.ChangeHealth(nAmount);

        base.Execute();
    }
}
