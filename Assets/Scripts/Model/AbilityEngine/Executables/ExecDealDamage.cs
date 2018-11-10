using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Can create executables like ...= new ExecDealDamage(){chrTarget = ..., nDamage = ...};

public class ExecDealDamage : Executable {

    public Chr chrTarget;
    public int nDamage;

	public override void Execute() {

        chrTarget.ChangeHealth(-nDamage);

        base.Execute();
    }

}
