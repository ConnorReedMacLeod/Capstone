using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Can create executables like ...= new Exec(){chrTarget = ..., nDamage = ...};

public class ExecStun : Executable {

    public Chr chrTarget;
    public int nAmount;

    public override void Execute() {

        chrTarget.ChangeFatigue(nAmount);

        base.Execute();
    }

}
