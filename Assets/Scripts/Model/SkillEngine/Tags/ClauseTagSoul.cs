using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClauseTagSoul : ClauseTag<SoulChr> {

    public enum TYPE { SELF, MELEE, RANGED, SWEEPING, ALLY, ENEMY, NONSELF };

    public TYPE type;

    public ClauseTagSoul(Clause _clause) : base(_clause) {

    }

}