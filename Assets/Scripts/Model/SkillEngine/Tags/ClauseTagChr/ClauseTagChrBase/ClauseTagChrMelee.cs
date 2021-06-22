using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClauseTagChrMelee : ClauseTagChr {

    public override List<Chr> ApplySelectionFiltering(List<Chr> lstTargets) {
        return lstTargets.Where(c => c.position.positiontype == Position.POSITIONTYPE.FRONTLINE).ToList<Chr>();
    }

    public override List<Chr> DisambiguateFinalTargetting(List<Chr> lstTargets, SelectionSerializer.SelectionInfo selectionInfo) {
        //For ranged, only accept the character that we specifically initially selected

        return lstTargets.Where(c => c.globalid == ((SelectionSerializer.SelectionChr)selectionInfo).chrSelected.globalid).ToList<Chr>();
    }

    public ClauseTagChrMelee(Clause _clause) : base(_clause) {

    }
}
