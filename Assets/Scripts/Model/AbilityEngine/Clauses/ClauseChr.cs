using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClauseChr : Clause {

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

    public override void Execute() {
        List<Chr> lstTargets = GetFinalTargets();

        for (int i = 0; i < lstTargets.Count; i++) {
            //Supply a reference to the current processed character
            chrCurrentProcessingTarget = lstTargets[i];

            for (int j = 0; j < lstExec.Count; j++) {

                //Make a new copy of the mold of the current executable
                Executable execCopy = lstExec[j].MakeCopy();

                //Ensure its targetting information is properly filled out
                execCopy.SetTarget();

                //Push the new copy onto the stack
                ContAbilityEngine.Get().AddExec(execCopy);
            }

            //Clear out that reference as soon as we've finished processing this character
            chrCurrentProcessingTarget = null;
        }
    }

    public ClauseChr(Action _action) : base(_action) { }

    public ClauseChr(ClauseChr other) : base(other) {
        plstTags = new Property<List<ClauseTagChr>>(other.plstTags);
    }

    public override Clause MakeCopy() {
        return new ClauseChr(this);
    }
}

