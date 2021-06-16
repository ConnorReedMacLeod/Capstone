using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClauseTagSkill : ClauseTag<Skill> {

    public enum TYPE { SELF, MELEE, RANGED, SWEEPING, ALLY, ENEMY, NONSELF };

    public TYPE type;

    public ClauseTagSkill(Clause _clause) : base(_clause) {

    }


}
