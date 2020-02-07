using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClauseSpecial : Clause {

    //TODO:: Is this even necessary to have this class?

    public abstract void ClauseEffect();


    public override void Execute() {

        ClauseEffect();
        
    }

    public ClauseSpecial(Action _action): base(_action) {

    }

    public ClauseSpecial(ClauseSpecial other): base(other) {

    }
}
