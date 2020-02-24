using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TypeAction {

    public Action act;
    public enum TYPE {ACTIVE, CANTRIP, CHANNEL, PASSIVE };

    public TypeAction(Action _act) {
        act = _act;
    }

    public abstract string getName();
    public abstract TYPE Type();
    public abstract int GetActionPointCost();

    public virtual bool Usable() {
        //By default, abilities can be used

        return true;
    }

    public virtual void PayActionPoints() {
        //Ensure we're in a ready state
        Debug.Assert(act.chrSource.curStateReadiness.Type() == StateReadiness.TYPE.READY);

        StateReady stateReady = (StateReady)(act.chrSource.curStateReadiness);

        //Ensure we have enough Action Points left to use this ability
        Debug.Assert(stateReady.nCurActionsLeft >= GetActionPointCost());

        stateReady.nCurActionsLeft -= GetActionPointCost();
    }

    public virtual void UseAction() {
        //By default, just execute the ability
        act.Execute();

    }

    //Fetch the current selection information for the player using this ability
    public virtual SelectionSerializer.SelectionInfo GetSelectionInfo() {
        //TODONOW - implement this
        //By default, grab the player's current selection

    }

}
