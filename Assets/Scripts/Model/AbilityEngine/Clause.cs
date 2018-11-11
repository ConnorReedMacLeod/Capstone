using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
