using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateReady : StateReadiness {

    public int nCurActionsLeft;     //The number of actions left in a turn that the character can use (cantrips cost 0)
    public int nQueuedFatigue;      //The amount of fatigue that will be added to the characters fatigue when they're done acting for the turn


    public StateReady(Chr _chrOwner, int _nCurActionsLeft) : base(_chrOwner) {

        nCurActionsLeft = _nCurActionsLeft;
        
    }

    public override TYPE Type() {
        return TYPE.READY;
    }

    public override void Recharge() {
        Debug.Log("We shouldn't be able to recharge while in the ready state");
    }

    public override bool CanSelectAction(Action act) {
        //We actually can select another action if we're in the Ready state

        if (!act.type.Usable()) {
            //Then this type of action cannot be activated

            return false;
        }
        
        if(act.type.GetActionPointCost() > nCurActionsLeft) {
            //Then we don't have enough ability activations left for this character to use the action

            return false;
        }

        return true;
    }


    public override void OnEnter() {

    }

    public override void OnLeave() {

       

    }
}
