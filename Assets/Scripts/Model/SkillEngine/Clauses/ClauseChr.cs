﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClauseChr : Clause {

    public Property<List<ClauseTagChr>> plstTags;

    public ClauseTagChr GetBaseTag() {
        return plstTags.Get()[0];
    }

    public List<Chr> GetSelectableUniverse() {
        //Returns all possible entities of our type (Chr)
        return Chr.lstChrInPlay;
    }

    public List<Chr> GetSelectable() {
        List<Chr> lstSelectable = GetSelectableUniverse();

        List<ClauseTagChr> lstTags = plstTags.Get();
        //Apply each of our tags filtering one-by-one to trim the universe down 
        //  to what the clause can properly select
        for(int i = 0; i < lstTags.Count; i++) {
            lstSelectable = lstTags[i].ApplySelectionFiltering(lstSelectable);
        }

        return lstSelectable;
    }

    //Can interpret any serialized targetting info as needed
    public List<Chr> GetFinalTargets(SelectionSerializer.SelectionChr selectionInfo) {

        //Ask the base tag to interpret selection info
        return GetBaseTag().DisambiguateFinalTargetting(GetSelectable(), selectionInfo);

    }

    public override bool IsSelectable(SelectionSerializer.SelectionInfo selectionInfo) {
        //Determines if this is an allowable selection to make
        Chr chrTargetAttempt = ((SelectionSerializer.SelectionChr)selectionInfo).chrSelected;

        if(chrTargetAttempt == null) Debug.LogError("ChrTarget was given as null");

        foreach(Chr c in GetSelectable()) {
            if(c == chrTargetAttempt) {
                return true;
            }
        }

        return false;
    }

    public override bool HasFinalTarget(SelectionSerializer.SelectionInfo selectionInfo) {
        //Determines if the selection would result in at least one character being affected by the skill's selection
        return GetFinalTargets((SelectionSerializer.SelectionChr)selectionInfo).Count >= 1;
    }

    public abstract void ClauseEffect(Chr chrSelected);

    public override void Execute() {
        List<Chr> lstTargets = GetFinalTargets((SelectionSerializer.SelectionChr)GetSelectionInfo());

        for(int i = 0; i < lstTargets.Count; i++) {

            //Execute the effect of this clause on this particular target
            ClauseEffect(lstTargets[i]);

        }
    }

    public ClauseChr(Skill _skill) : base(_skill) {
        targetType = TargetType.CHR;
    }

}

