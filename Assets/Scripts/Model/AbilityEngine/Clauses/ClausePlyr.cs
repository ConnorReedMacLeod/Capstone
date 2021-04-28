using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClausePlyr : Clause {

    public Property<List<ClauseTagPlayer>> plstTags;


    public ClauseTagPlayer GetBaseTag() {
        return plstTags.Get()[0];
    }

    public List<Player> GetSelectableUniverse() {
        //Returns all possible entities of our type (Player)
        return Player.lstAllPlayers;
    }

    public List<Player> GetSelectable() {
        List<Player> lstSelectable = GetSelectableUniverse();

        List<ClauseTagPlayer> lstTags = plstTags.Get();
        //Apply each of our tags filtering one-by-one to trim the universe down 
        //  to what the clause can properly select
        for(int i = 0; i < lstTags.Count; i++) {
            lstSelectable = lstTags[i].ApplySelectionFiltering(lstSelectable);
        }

        return lstSelectable;
    }

    //Can interpret any serialized targetting info as needed
    public List<Player> GetFinalTargets(SelectionSerializer.SelectionPlayer selectionInfo) {

        //Ask the base tag to interpret selection info
        return GetBaseTag().DisambiguateFinalTargetting(GetSelectable(), selectionInfo);

    }

    public override bool IsSelectable(SelectionSerializer.SelectionInfo selectionInfo) {
        //Determines if this is an allowable selection to make
        Player plyrTargetAttempt = ((SelectionSerializer.SelectionPlayer)selectionInfo).plyrSelected;

        if(plyrTargetAttempt == null) Debug.LogError("PlyrSelected was given as null");

        foreach(Player p in GetSelectable()) {
            if(p == plyrTargetAttempt) {
                return true;
            }
        }

        return false;
    }


    public override bool HasFinalTarget(SelectionSerializer.SelectionInfo selectionInfo) {
        //Determines if the selection would result in at least one character being affected by the ability's selection
        return GetFinalTargets((SelectionSerializer.SelectionPlayer)selectionInfo).Count >= 1;
    }

    public abstract void ClauseEffect(Player plyrSelected);

    public override void Execute() {
        List<Player> lstTargets = GetFinalTargets((SelectionSerializer.SelectionPlayer)GetSelectionInfo());

        for(int i = 0; i < lstTargets.Count; i++) {

            //Execute the effect of this clause on this particular target
            ClauseEffect(lstTargets[i]);

        }
    }

    public ClausePlyr(Action _action) : base(_action) { }


}

