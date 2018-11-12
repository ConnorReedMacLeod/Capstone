using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Can create executables like ...= new Exec(){chrTarget = ..., nDamage = ...};

public class ExecDealDamage : Executable {

    public Chr chrTarget;
    public int nDamage;

	public override void Execute() {
        //TODO:: Take into account armour and power/defense
        chrTarget.ChangeHealth(-nDamage);

        base.Execute();
    }

}
