using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Can create executables like ...= new Exec(){soulContainerTarget = ..., funcApplySoul = ...};

public class ExecApplySoul : Executable {

    public SoulContainer soulContainerTarget;

    public delegate Soul FuncCreateSoul();

    public FuncCreateSoul funcCreateSoul;

    public override void Execute() {

        Debug.Log("In ExecApplySoul's execute method");

        soulContainerTarget.ApplySoul(funcCreateSoul());

        base.Execute();
    }
}
