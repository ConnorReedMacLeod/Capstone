using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClauseTag<T> {

    //TODO:: Think about if it's more efficient to just create a mapping of T->Bool and then unflagging each possible 
    //       target as it gets filtered out as being untargettable.  Reduces the possibility of needing to reinitialize lists constantly with LINQ

    //A reference to the clause this is attached to
    public Clause clause;

    //This controls what is selectable in the initial selection process (while you are deciding what targets could be selected,
    //  before executing the ability)
    public abstract List<T> ApplySelectionFiltering (List<T> lstTargets);


    // NOTE: This is relevant only for Base Tags
    //Use the selection info from the player to choose what the final targetting should be. 
    // This is mainly relevant for abilities (channels) that could have a window between selection and execution, 
    // where the selected target becomes invalid at the time of execution.  This means that the ability needs to decide
    // what interprettation of the original selection should be used to determine the final target.  
    public virtual List<T> DisambiguateFinalTargetting(List<T> lstTargets, SelectionSerializer.SelectionInfo selectionInfo) {
        Debug.LogError("Attempted to disambiguate final targetting with a non-base tag!");
        return null;
    }

    public ClauseTag(Clause _clause){
        clause = _clause;
    }


}
