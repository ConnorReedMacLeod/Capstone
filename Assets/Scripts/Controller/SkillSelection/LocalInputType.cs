using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LocalInputType : MonoBehaviour {

    public Player plyrOwner;
    public bool bCurrentlySelectingSkill;

    public virtual bool CanProceedWithSkillSelection() {
        // By default, we don't allow manual selection of any of our character skills
        // if this player is an AI/scripted

        return false;
    }

    public virtual void StartSelection() {
        bCurrentlySelectingSkill = true;
    }

    public virtual void CompletedSelection() {
        bCurrentlySelectingSkill = false;
    }

    public virtual void EndSelection() {

        bCurrentlySelectingSkill = false;
    }

    public void SelectionTimedOut() {
        Debug.Log("Warning: Master told us we timed out on our skill selection");
        EndSelection();
    }

    public void GaveInvalidTarget() {
        Debug.Log("Warning: The attempt selectionInfo was invalid - must select another");

        //We should resume looking for input for selecting a skill
        StartSelection();
    }

    public void SetOwner(Player _plyrOwner) {
        plyrOwner = _plyrOwner;

    }


}
