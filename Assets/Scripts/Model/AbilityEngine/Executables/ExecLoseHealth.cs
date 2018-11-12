using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Can create executables like ...= new ExecLoseHealth(){chrTarget = ..., nAmount = ...};

public class ExecLoseHealth : Executable {

    public Chr chrTarget;
    public int nAmount;

    public override void Execute() {

        chrTarget.ChangeHealth(-nAmount);

        base.Execute();
    }
}
