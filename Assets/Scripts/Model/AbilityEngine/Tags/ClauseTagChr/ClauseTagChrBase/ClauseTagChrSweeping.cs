using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClauseTagChrSweeping : ClauseTagChr {

    public override List<Chr> ApplySelectionFiltering(List<Chr> lstTargets) {
        return lstTargets;
    }

    public override List<Chr> DisambiguateFinalTargetting(List<Chr> lstTargets, SelectionSerializer.SelectionInfo selectionInfo) {
        //For sweeping, accept all targets that are on the same team as the original target (even if the selected target becomes invalid)

        return lstTargets.Where(c => c.plyrOwner.id == ((SelectionSerializer.SelectionChr)selectionInfo).chrOwner.plyrOwner.id).ToList<Chr>();
    }

    public ClauseTagChrSweeping(Clause _clause) : base(_clause) {

    }
}
