using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clauses store a set of executables that will be pushed onto the stack
/// once the Clause is on top of the stack.  This means that any replacement effects
/// or triggers that would interact with any of the stored executables 
///  will only take effect if they are present when the clause is evaluated
/// </summary>

public abstract class Clause {

    public Skill skill;

    public abstract string GetDescription();

    public void Execute() {
        //Grab the stored serialized selections, and pass it to the overrideable ClauseEffect where
        // the abilities can customize what they will do

        ClauseEffect(ContSkillSelection.Get().selectionsFromMaster);

    }

    //Let any extending clause instance decide what they want to do for their clause effect
    public abstract void ClauseEffect(Selections selections);

    public Clause(Skill _skill) {
        skill = _skill;
    }

}
