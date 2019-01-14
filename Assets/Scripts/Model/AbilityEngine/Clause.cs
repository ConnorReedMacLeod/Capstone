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
public class Clause {

    public delegate void funcExecuteClause();
    public funcExecuteClause fExecute;

    public void Execute() {

        fExecute();

    }

}
