using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClauseTagChrNonSelf : ClauseTagChr {

    public override List<Chr> ApplySelectionFiltering(List<Chr> lstTargets) {
        return lstTargets.Where(c => c.globalid != clause.action.chrSource.globalid).ToList<Chr>();
    }

    public ClauseTagChrNonSelf(Clause _clause) : base(_clause) {

    }
}
