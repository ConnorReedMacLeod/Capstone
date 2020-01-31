using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClauseChr : Clause {

    public Property<List<ClauseTagChr>> plstTags;

    //Stores the character that is currently being processed by this clause
    public Chr chrCurrentProcessingTarget;

    public List<Chr> GetSelectableUniverse() {
        //Returns all possible entities of our type (Chr)
        return Chr.lstChrInPlay;
    }

    public List<Chr> GetSelectable() {
        List<Chr> lstSelectable = GetSelectableUniverse();

        List<ClauseTagChr> lstTags = plstTags.Get();
        //Apply each of our tags filtering one-by-one to trim the universe down 
        //  to what the clause can properly select
        for (int i = 0; i < lstTags.Count; i++) {
            lstSelectable = lstTags[i].ApplySelectionFiltering(lstSelectable);
        }

        return lstSelectable;
    }

    //Can interpret any serialized targetting info as needed
    public List<Chr> GetFinalTargets(SelectionSerializer.SelectionChr selectionInfo) {

        List<Chr> lstSelectable = GetSelectableUniverse();

        List<ClauseTagChr> lstTags = plstTags.Get();
        //Apply each of our tags filtering one-by-one to trim the universe down 
        //  to what the clause can properly select
        for (int i = 0; i < lstTags.Count; i++) {
            lstSelectable = lstTags[i].ApplyTargettingFiltering(lstSelectable, selectionInfo);
        }

        return lstSelectable;

    }

    public abstract void ClauseEffect(Chr chrSelected);

    public override void Execute() {
        List<Chr> lstTargets = GetFinalTargets();

        for (int i = 0; i < lstTargets.Count; i++) {

            //Execute the effect of this clause on this particular target
            ClauseEffect(lstTargets[i]);
            
        }
    }

    public ClauseChr(Action _action) : base(_action) { }

    public ClauseChr(ClauseChr other) : base(other) {
        plstTags = new Property<List<ClauseTagChr>>(other.plstTags);
    }
}

