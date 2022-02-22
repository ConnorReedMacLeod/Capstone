using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LocalInputType {

    public Player plyrOwner;
    public bool bCurrentlySelectingSkill;

    public enum InputType { NONE, HUMAN, AI, SCRIPTED }

    public abstract InputType GetInputType();


    public virtual void Init() {
        //Nothing needs to be done by default - override as necessary
    }

    public virtual bool CanProceedWithSkillSelection() {
        // By default, we don't allow manual selection of any of our character skills
        // unless this is a local controller

        return false;
    }

    public virtual void StartSelection() {
        bCurrentlySelectingSkill = true;
    }

    public virtual void EndSelection() {

        bCurrentlySelectingSkill = false;
    }

    public void SelectionTimedOut() {
        Debug.Log("Warning: Master told us we timed out on our skill selection");
        EndSelection();
    }

    public void SetOwner(Player _plyrOwner) {
        plyrOwner = _plyrOwner;

    }


}
