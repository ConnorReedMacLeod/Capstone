using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClauseTagChrRanged : ClauseTagChr {

    public override List<Chr> ApplySelectionFiltering(List<Chr> lstTargets) {
        return lstTargets;
    }

    public override List<Chr> DisambiguateFinalTargetting(List<Chr> lstTargets, SelectionSerializer.SelectionInfo selectionInfo) {
        //For ranged, only accept the character that we specifically initially selected

        return lstTargets.Where(c => c.id == ((SelectionSerializer.SelectionChr)selectionInfo).chrOwner.id).ToList<Chr>();
    }

    public ClauseTagChrRanged(Clause _clause) : base(_clause) {

    }
}
