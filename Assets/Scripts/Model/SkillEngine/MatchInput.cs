using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MatchInput {

    //Store which player is responsible for filling in and executing this input
    public int iPlayerActing;

    //Each input type should extend this to provide some process by which the MatchInput information is filled out
    public abstract IEnumerator GatherManualInput();

    //Each input type should extend this to define what this input should actually do to affect the game state
    public abstract IEnumerator Execute();

    public MatchInput(int _iPlayerActing) {
        iPlayerActing = _iPlayerActing;
    }

    public MatchInput(MatchInput other) {
        iPlayerActing = other.iPlayerActing;
    }
}
