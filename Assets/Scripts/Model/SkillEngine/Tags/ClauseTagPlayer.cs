using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClauseTagPlayer : ClauseTag<Player> {

    public enum TYPE { ALLY, ENEMY };

    public TYPE type;

    public ClauseTagPlayer(Clause _clause) : base(_clause) {

    }
}

