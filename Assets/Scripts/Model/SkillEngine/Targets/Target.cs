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
    public List<object> GetSelectable(Selections selectionsSoFar) {

        return GetSelactableUniverse().Where(obj => IsValidSelection(obj, selectionsSoFar)).ToList();

    }

    public abstract int Serialize(object objToSerialize);
    public abstract object Unserialize(int nSerialized);

    public Target(FnValidSelection _IsValidSelection) {
        IsValidSelection = _IsValidSelection;
    }
}
