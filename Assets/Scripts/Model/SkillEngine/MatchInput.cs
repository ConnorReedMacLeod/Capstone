using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MatchInput {

    //Store which player is responsible for filling in and executing this input
    public int iPlayerActing;

    //Each input type has to provide a way to serialize and deserialize itself
    public abstract int[] Serialize();
    public MatchInput(int[] arnSerializedSelections) {

    }

    //Each input type should extend these to provide some process by which the MatchInput information is filled out
    //  Additional functions to help with the real-time interaction with this process will likely be needed
    public abstract void StartManualInputProcess();
    public abstract void EndManualInputProcess();

    //Each input type should extend this to define what this input should actually do to affect the game state
    public abstract IEnumerator Execute();

    //Each input type will need to define if it would be valid to execute in its currently filled out state
    public abstract bool CanLegallyExecute();

    //Each input type will need to provide a way to set itself to a random selection (In case this fails, outside sources can't call it,
    //  it should only be consulted by FillRandomly())
    protected abstract void AttemptFillRandomly();

    //This will fill out the matchinput with a random selection (defaulting to some failsafe input if we fail to successfully randomize it)
    public void FillRandomly() {
        //First, randomize ourselves with our extended type-specific randomization process
        AttemptFillRandomly();

        //If this resulted in an illegally filled input, then hard-reset to a default legal selection
        if(CanLegallyExecute() == false) {
            ResetToDefaultInput();
        }
    }

    //Each input type will need to provide a way to reset to an input that will be guaranteed to be legal (as a failsafe)
    public abstract void ResetToDefaultInput();

    public MatchInput(int _iPlayerActing) {
        iPlayerActing = _iPlayerActing;
    }

    public MatchInput(MatchInput other) {
        iPlayerActing = other.iPlayerActing;
    }

    // If we have partially filled out some of the required input but then cancel the process, we need to be able to clear out that partial input
    //  to clean out that data so that a fresh selection process can start without any lingering stale data.  
    public abstract void ResetPartialSelection();
}
