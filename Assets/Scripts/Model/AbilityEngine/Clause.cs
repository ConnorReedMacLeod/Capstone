using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clauses store a set of executables that will be pushed onto the stack
/// once the Clause is on top of the stack.  This means that any replacement effects
/// or triggers that would interact with any of the stored executables 
///  will only take effect if they are present when the clause is evaluated
/// </summary>

//This class should be set up to have an fExecute method that will
// add a number of Executables to the executable stack

//Can be initialized like ... = new Clause(){ fExecute = (() => ...)};
public abstract class Clause<T> {

    public Action action;

    public Targetter<T> targetter;

    public delegate void funcExecuteClause(T t);
    public funcExecuteClause fExecute;

    public void Execute() {

        List<T> lstTargets = targetter.GetTargets();

        for (int i = 0; i < lstTargets.Count; i++) {
            fExecute(lstTargets[i]);
        }

    }

    public Clause (Action _action){
        action = _action;
    }

}
