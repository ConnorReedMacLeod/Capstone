using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateReady : StateReadiness {

    public StateReady(Chr _chrOwner) : base(_chrOwner) {

    }

    public override bool CanSelectAction() {
        //We actually can select another action if we're in the Ready state
        return true;
    }

}
