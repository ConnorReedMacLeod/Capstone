using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClauseTag<T> {

    //A reference to the clause this is attached to
    public Clause clause;

    //This controls what is selectable in the initial selection process
    public abstract List<T> ApplySelectionFiltering (List<T> lstTargets);

    //This controls what is targettable upon being given a list of targetted characters, as well as any selectionInfo
    // NOTE - This is passed a base selectionInfo, but since only a few tags will actually peek into this selectionInfo, 
    //        they can cast it to the appropriate derived type anyway as they need
    public virtual List<T> ApplyTargettingFiltering(List<T> lstTargets, SelectionSerializer.SelectionInfo selectionInfo) {
        //By default, most tags will just filter their targets in the same way as their selections - some cases
        //  like sweeping may need to expand this list or reselect things as needed
        return ApplySelectionFiltering(lstTargets);
    }


}
