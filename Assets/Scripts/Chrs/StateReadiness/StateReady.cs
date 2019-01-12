using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateReady : StateReadiness {

    public int nCurActionsLeft;     //The number of actions left in a turn that the character can use (cantrips cost 0)
    public int nQueuedFatigue;      //The amount of fatigue that will be added to the characters fatigue when they're done acting for the turn

    public StateReady(Chr _chrOwner, int _nCurActionsLeft) : base(_chrOwner) {

        nCurActionsLeft = _nCurActionsLeft;
        
    }

    public override void Recharge() {
        Debug.Log("We shouldn't be able to recharge while in the ready state");
    }

    public override bool CanSelectAction(Action act) {
        //We actually can select another action if we're in the Ready state

        //But only if it's not a passive
        if (act.type == ActionTypes.TYPE.PASSIVE) return false;

        //If it's a cantrip, we can always use it
        if (act.type == ActionTypes.TYPE.CANTRIP) return true;

        //Otherwise, if it's a channel/active, check if we have enough actions left for this turn
        if(act.type == ActionTypes.TYPE.ACTIVE || act.type == ActionTypes.TYPE.CHANNEL) {
            return nCurActionsLeft >= act.nActionCost;
        }

        Debug.LogError("We didn't recognize this type of action");
        return false;
    }


    public override void OnEnter() {

    }

    public override void OnLeave() {

       

    }
}
