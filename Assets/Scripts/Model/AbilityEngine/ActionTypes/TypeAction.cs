using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TypeAction {

    public Action act;

    public TypeAction(Action _act) {
        act = _act;
    }

    public abstract int GetActionPointCost();

    public virtual bool Usable() {
        //By default, abilities can be used

        return true;
    }

    public void PayActionPoints() {
        //Ensure we're in a ready state
        Debug.Assert(act.chrSource.curStateReadiness.GetType() == typeof(StateReady));

        StateReady stateReady = (StateReady)(act.chrSource.curStateReadiness);

        //Ensure we have enough Action Points left to use this ability
        Debug.Assert(stateReady.nCurActionsLeft >= GetActionPointCost());

        stateReady.nCurActionsLeft -= GetActionPointCost();
    }

    public abstract void UseAction();

}
