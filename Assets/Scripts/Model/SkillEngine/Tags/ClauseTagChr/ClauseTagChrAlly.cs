using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClauseTagChrAlly : ClauseTagChr {

    public override List<Chr> ApplySelectionFiltering(List<Chr> lstTargets) {
        return lstTargets.Where(c => c.plyrOwner.id == clause.skill.chrOwner.plyrOwner.id).ToList<Chr>();
    }

    public ClauseTagChrAlly(Clause _clause) : base(_clause) {

    }
}
