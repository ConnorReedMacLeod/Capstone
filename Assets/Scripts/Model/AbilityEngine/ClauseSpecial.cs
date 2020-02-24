using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClauseSpecial : Clause {

    public abstract void ClauseEffect();


    public override void Execute() {

        ClauseEffect();
        
    }

    public ClauseSpecial(Action _action): base(_action) {

    }
}
