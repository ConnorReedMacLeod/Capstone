using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClauseTagChr : ClauseTag<Chr> {

    public enum TYPE { SELF, MELEE, RANGED, SWEEPING, ALLY, ENEMY, NONSELF };

    public TYPE type;

}
