using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClauseTagChrEnemy : ClauseTagChr {

    public override List<Chr> ApplySelectionFiltering(List<Chr> lstTargets) {
        return lstTargets.Where(c => c.plyrOwner.id != clause.skill.chrSource.plyrOwner.id).ToList<Chr>();
    }

    public ClauseTagChrEnemy(Clause _clause) : base(_clause) {

    }
}
