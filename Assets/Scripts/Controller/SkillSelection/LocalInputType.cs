using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LocalInputType {

    public Player plyrOwner;

    public enum InputType { NONE, HUMAN, AI, SCRIPTED }

    public abstract InputType GetInputType();

    public virtual bool CanProceedWithSkillSelection() {
        // By default, we don't allow manual selection of any of our character skills
        // unless this is a local controller

        return false;
    }

    public abstract void StartSelection(MatchInput matchInput);
    public abstract void EndSelection(MatchInput matchInput);


    public void SelectionTimedOut() {
        Debug.Log("Warning: Master told us we timed out on our skill selection");
        EndSelection(ContSkillEngine.Get().matchinputToFillOut);
    }

    public void SetOwner(Player _plyrOwner) {
        plyrOwner = _plyrOwner;

    }


}
