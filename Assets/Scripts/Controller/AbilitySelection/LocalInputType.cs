using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LocalInputType : MonoBehaviour {

    public Player plyrOwner;
    public bool bCurrentlySelectingSkill;

    public virtual bool CanProceedWithSkillSelection(Chr chrSelected) {
        // By default, we don't allow manual selection of character skills via the UI
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
        Debug.Log("Warning: Master deemed the sent selectionInfo was invalid - must select another");

        //We should resume looking for input for selecting an ability
        StartSelection();
    }

    public void SetOwner(Player _plyrOwner) {
        plyrOwner = _plyrOwner;
    }


}
