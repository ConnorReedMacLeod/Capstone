using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClauseTagChrNonSelf : ClauseTagChr {

    public override List<Chr> ApplySelectionFiltering(List<Chr> lstTargets) {
        return lstTargets.Where(c => c.id != clause.action.chrSource.id).ToList<Chr>();
    }

    public ClauseTagChrNonSelf(Clause _clause) : base(_clause) {

    }
}
