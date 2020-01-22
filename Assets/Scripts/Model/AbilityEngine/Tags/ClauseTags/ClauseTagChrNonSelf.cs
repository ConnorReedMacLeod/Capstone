using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetterTagChrNonSelf : ClauseTagChr {

    public override List<Chr> ApplySelectionFiltering(List<Chr> lstTargets) {
        return lstTargets.Where(c => c.id != clause.action.chrSource.id).ToList<Chr>();
    }
}
