using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clauses will create a set of executables that will be pushed onto the stack
/// once the Clause is on top of the stack.  This means that any replacement effects
/// or triggers that would interact with any of the stored executables 
///  will only take effect if they are present when the clause is evaluated
/// </summary>


public abstract class Clause {

    //Describe what the clause effect should do
    public abstract string GetDescription();

    //Each type of clause should define how they get their input for their effect, and then
    //  how they should use that input to push executables to affect the game state
    public abstract void Execute();
    

}
