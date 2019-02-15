using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClauseEndAbility : Clause {

    public ClauseEndAbility(Action _act) {
        //This is just a pre-built clause that will just put a startability
        // marker on the stack
        fExecute = () => {
            ContAbilityEngine.Get().AddExec(new ExecEndAbility(_act));
        };

    }
}
