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

    public Action action;

    public List<Executable> lstExec = new List<Executable>();

    public abstract void Execute();

    public Clause (Action _action){
        action = _action;
    }

    public Clause(Clause other) {
        action = other.action;
        lstExec = new List<Executable>(other.lstExec);
    }

    public abstract Clause MakeCopy();
}
