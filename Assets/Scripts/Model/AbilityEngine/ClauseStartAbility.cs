using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClauseStartAbility : Clause {


    public ClauseStartAbility(Action _act) {
        //This is just a pre-built clause that will just put a startability
        // marker on the stack
        fExecute = () => {
            ContAbilityEngine.Get().AddExec(new ExecStartAbility(_act));
        };

    }
}
