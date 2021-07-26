using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClauseTagChrSelf : ClauseTagChr {

    public override List<Chr> ApplySelectionFiltering(List<Chr> lstTargets) {
        return lstTargets.Where(c => c.globalid == clause.skill.chrOwner.globalid).ToList<Chr>();
    }

    public override List<Chr> DisambiguateFinalTargetting(List<Chr> lstTargets, SelectionSerializer.SelectionInfo selectionInfo) {
        //Since the [Self] tag will always be filtering down the possible universe of targets to only 
        // themself (or no one if they are not a valid target), then we can just return the original lst of possible targets
        Debug.Assert(lstTargets.Count <= 1);
        return lstTargets;
    }

    public ClauseTagChrSelf(Clause _clause) : base(_clause) {

    }
}
