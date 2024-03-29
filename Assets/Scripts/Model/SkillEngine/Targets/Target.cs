﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Target {

    public Skill skill;

    public int iTargetIndex;

    public delegate bool FnValidSelection(object objSelected, InputSkillSelection selectionsSoFar);

    protected FnValidSelection IsValidSelection;

    public InputSkillSelection selectionsSoFar;

    public virtual bool CanSelect(object objSelected, InputSkillSelection selectionsSoFar) {
        //By default, just call our stored IsValidSelection function.  Extenders of this may need to ask the object being selected
        //  if it needs to override the ability to be selected
        return IsValidSelection(objSelected, selectionsSoFar);
    }

    //Return a list of all entities of the corresponding type for this target
    public abstract IEnumerable<object> GetSelectableUniverse();

    //Return a list of all valid entities that could be selected our of the universe of the corresponding type
    public List<object> GetValidSelectable(InputSkillSelection selectionsSoFar) {

        return GetSelectableUniverse().Where(obj => CanSelect(obj, selectionsSoFar)).ToList();

    }

    public virtual bool HasAValidSelectable(InputSkillSelection selectionsSoFar) {
        return GetValidSelectable(selectionsSoFar).Count != 0;
    }

    //Get a random valid selection for this type of target (for AI purposes mainly)
    public object GetRandomValidSelectable(InputSkillSelection selectionsSoFar) {

        List<object> lstPossibleValidSelections = GetValidSelectable(selectionsSoFar);

        int nRandomIndex = Random.Range(0, lstPossibleValidSelections.Count);

        return lstPossibleValidSelections[nRandomIndex];

    }

    //Get a random **possibly invalid** selection for this type of target (currently used for a simple AI with a randomized script of selections)
    public virtual object GetRandomSelectable() {

        return LibRandom.GetRandomElementOfList<object>(GetSelectableUniverse().ToList());
    }

    public abstract int Serialize(object objToSerialize);
    public abstract object Unserialize(int nSerialized, List<object> lstSelectionsSoFar);

    // Essentially these are State's OnEnter and OnLeave triggers for when the local player
    //  is filling out the required selections for their chosen skill.
    // Can set up any needed UI elements/highlighting to facilitate selections, and can set up
    //  any custom subscriptions to those UI elements
    // By default, they will always

    protected virtual void OnStartLocalSelection() {
        //Don't need to do anything by default
    }

    protected virtual void ShiftCameraToTarget() {
        //By default, don't do any camera shifting, I guess
    }

    public void StartLocalSelection(InputSkillSelection _selectionsSoFar) {
        //Temporarily store the currently made selections so far in case we need to retrieve them to assist in our targetting
        selectionsSoFar = _selectionsSoFar;

        ContGlobalInteractions.subGlobalRightClick.Subscribe(cbCancelSelectionProcess);
        OnStartLocalSelection();

        ShiftCameraToTarget();
    }
    protected virtual void OnEndLocalSelection() {
        //Don't need to do anything by default
    }
    public void EndLocalSelection() {

        //Clean up any local-setup for chosing this target (like spawned UI)
        OnEndLocalSelection();
        ContGlobalInteractions.subGlobalRightClick.UnSubscribe(cbCancelSelectionProcess);

        //Clear out the temporary local storage of the ongoing selections
        selectionsSoFar = null;
    }

    public void cbCancelSelectionProcess(Object target, params object[] args) {

        //End the local selection process for this target, including 
        // cleaning up any local-setup for chosing this target (like spawned UI)
        EndLocalSelection();
        ContLocalUIInteraction.Get().CancelSelectionsProcess();
    }

    //Sets the TargetDescription message (derived types can set it to a default description for the type)
    public abstract void InitTargetDescription();

    public string sTargetDescription;

    //Get a string representing a selection made to fill this target that we can use to describe this selection in the history log
    public abstract string GetHistoryDescription(object objTarget);

    //Each derived target type should subscribe/unsubscribe this to UI events for selecting their targets,
    //  then they can extract the model represented by the selected UI View component and pass it to AttemptSelection
    public abstract void cbClickSelectable(Object target, params object[] args);

    public void AttemptSelection(object objSelected) {
        Debug.Log("Attempted to select " + objSelected);
        if(CanSelect(objSelected, ContLocalUIInteraction.Get().selectionsInProgress) == false) {
            Debug.Log("Invalid selection attempted!");
            return;
        }

        //The selection is valid at this point so we can submit it to the LocalUIInteraction's built-up list
        ContLocalUIInteraction.Get().ReceiveNextSelection(objSelected);
    }

    public Target(Skill _skill, FnValidSelection _IsValidSelection) {
        skill = _skill;
        IsValidSelection = _IsValidSelection;

        //Save the index that this target will be placed at in the skill's target list
        iTargetIndex = skill.lstTargets.Count;
    }

    public static FnValidSelection AND(FnValidSelection fn1, FnValidSelection fn2) {
        return (object o, InputSkillSelection selections) => fn1(o, selections) && fn2(o, selections);
    }

    public static bool TRUE(object obj, InputSkillSelection selections) { return true; }
}
