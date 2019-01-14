using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Provides the methods for executing an action(either cantrip, active, or channel)

public static class ActionTypes {

    public enum TYPE { ACTIVE, PASSIVE, CHANNEL, CANTRIP };
    public const int nActionCostCantrip = 0;
    public const int nActionCostActive = 1;
    public const int nActionCostChannel = 1;

    static void UseCantrip(Action act, StateReady curState) {

        //Get the action to push all of its effects onto its stack
        act.Execute();

        //Then pop them off of its stack and push them onto the ability engine's stack
        while (act.stackClauses.Count != 0) {
            ContAbilityEngine.Get().AddClause(act.stackClauses.Pop());
        }

        Debug.Assert(curState.nCurActionsLeft >= nActionCostCantrip);
        //Reduce the Ready state's current number of actions by 0
        curState.nCurActionsLeft -= nActionCostCantrip;

        //Stay in a Ready state
    }

    static void UseActive(Action act, StateReady curState) {

        //Get the action to push all of its effects onto its stack
        act.Execute();

        //Then pop them off of its stack and push them onto the ability engine's stack
        while (act.stackClauses.Count != 0) {
            ContAbilityEngine.Get().AddClause(act.stackClauses.Pop());
        }

        Debug.Assert(curState.nCurActionsLeft >= nActionCostActive);
        //Reduce the Ready state's current number of actions by 1
        curState.nCurActionsLeft -= nActionCostActive;

        //Stay in a Ready state for now
    }

    static void UseChannel(Action act, StateReady curState) {

        //Make a soul effect from the act's Execute effect
        if(act.soulChannel == null) {
            //if we don't have a soul effect, then make one out of the action's
            // execute effect
            act.soulChannel = new Soul(act.chrSource, act.chrSource) {
                bVisible = false,
                bDuration = false,

                //TODO - complete this
            };
        }

        Debug.Assert(curState.nCurActionsLeft >= nActionCostChannel);
        //It's a bit weird to reduce your action cost on a channel since you're
        //gonna be switching states, but it should be done for the sake of completeness
        curState.nCurActionsLeft -= nActionCostChannel;

        //Move to a Channel State
    }

    public static void UseAction(Action act) {
        Debug.Assert(act.VerifyLegal());

        //Double check we're in a ready state
        Debug.Assert(act.chrSource.curStateReadiness.GetType() == typeof(StateReady));

        StateReady curState = (StateReady)(act.chrSource.curStateReadiness);

        //Pay for the action
        act.PayForAction();

        switch (act.type) {
            case TYPE.ACTIVE:
                UseActive(act, curState);

                break;
            case TYPE.CANTRIP:
                UseCantrip(act, curState);

                break;
            case TYPE.CHANNEL:
                UseChannel(act, curState);

                break;
            case TYPE.PASSIVE:
                Debug.LogError("ERROR - can't activate a passive");

                break;
        }

        //Reset all of the targets selected for the action
        act.ResetTargettingArgs();

    }

}
