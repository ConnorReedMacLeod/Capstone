using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClauseTagChrMelee : ClauseTagChr {

    public override List<Chr> ApplySelectionFiltering(List<Chr> lstTargets) {
        return lstTargets.Where(c => c.bBlocker).ToList<Chr>();
    }

    public override List<Chr> DisambiguateFinalTargetting(List<Chr> lstTargets, SelectionSerializer.SelectionInfo selectionInfo) {
        //Scan through the potential blockers (only blockers should remain at this point) and choose only the blocker 
        // on the same team as was originally selected

        return lstTargets.Where(c => c.plyrOwner.id == ((SelectionSerializer.SelectionChr)selectionInfo).chrOwner.plyrOwner.id).ToList<Chr>();
    }

    public ClauseTagChrMelee(Clause _clause) : base(_clause) {

    }
}
