using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Target {

    public delegate bool FnValidSelection(object objSelected, Selections selectionsSoFar);

    public FnValidSelection IsValidSelection;

    //Return a list of all entities of the corresponding type for this target
    public abstract IEnumerable<object> GetSelactableUniverse();

    //Return a list of all valid entities that could be selected our of the universe of the corresponding type
    public List<object> GetValidSelectable(Selections selectionsSoFar) {

        return GetSelactableUniverse().Where(obj => IsValidSelection(obj, selectionsSoFar)).ToList();

    }

    //Get a random valid selection for this type of target (for AI purposes mainly)
    public object GetRandomValidSelectable(Selections selectionsSoFar) {

        List<object> lstPossibleValidSelections = GetValidSelectable(selectionsSoFar);

        int nRandomIndex = Random.Range(0, lstPossibleValidSelections.Count);

        return lstPossibleValidSelections[nRandomIndex];

    }

    //Get a random **possibly invalid** selection for this type of target (currently used for a simple AI with a randomized script of selections)
    public object GetRandomSelectable() {
        List<object> lstPossibleSelections = GetSelactableUniverse().ToList();

        int nRandomIndex = Random.Range(0, lstPossibleSelections.Count);

        return lstPossibleSelections[nRandomIndex];
    }

    public abstract int Serialize(object objToSerialize);
    public abstract object Unserialize(int nSerialized);

    // Essentially these are State's OnEnter and OnLeave triggers for when the local player
    //  is filling out the required selections for their chosen skill.
    // Can set up any needed UI elements/highlighting to facilitate selections, and can set up
    //  any custom subscriptions to those UI elements
    public abstract void OnLocalStartSelection();
    public abstract void OnLocalEndSelection();

    //Sets the TargetDescription message (derived types can set it to a default description for the type)
    public abstract void InitTargetDescription();

    public string sTargetDescription;

    public void cbAttemptedSelection(Object target, params object[] args) {
        if(IsValidSelection(target, ContLocalUIInteraction.Get().selectionsInProgress) == false) {
            Debug.Log("Invalid selection attempted!");
            return;
        }

        //The selection is valid at this point so we can submit it to the LocalUIInteraction's built-up list
        ContLocalUIInteraction.Get().ReceiveNextSelection(target);
    }

    public Target(FnValidSelection _IsValidSelection) {
        IsValidSelection = _IsValidSelection;
    }

    public static FnValidSelection AND(FnValidSelection fn1, FnValidSelection fn2) {
        return (object o, Selections selections) => fn1(o, selections) && fn2(o, selections);
    }

    public static bool TRUE(object obj, Selections selections) { return true; }
}
