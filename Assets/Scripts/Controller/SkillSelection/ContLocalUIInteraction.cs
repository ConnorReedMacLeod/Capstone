using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class handles the UI interaction flow for clicking on characters/skills/selections
//
// It maintains the state of the interaction (i.e. if we're not currently selecting any
//  character, if we're selecting a character and showing their skills, if we're selecting
//  the targets for a particular skill (and which types of entities we're trying to select dependent
//  on the skill's targetting requirements).
//
// Will often consult the LocalInputType to see what interactions are possible to proceed
//  with given the control-level we have for the local player (i.e., if the local
//  player is human then we have permission to move ahead with selecting skills.  If
//  the local player is an AI, then we should only be able to click characters and hover over
//  skills to see information on them, but not to take any action with them.)
public class ContLocalUIInteraction : Singleton<ContLocalUIInteraction> {

    //The current character-selection state
    public StateTarget curState;

    public bool bCanSelectCharacters;

    public Chr chrSelected;

    public Selections selectionsInProgress;

    public static Subject subAllStartManualSelections = new Subject(Subject.SubType.ALL);
    public static Subject subAllFinishManualSelections = new Subject(Subject.SubType.ALL);

    // Start a new round of targetting
    public void ResetStoredSelections() {
        //Clear any previous targetting information we had
        selectionsInProgress = null;

    }

    // Ends targetting
    public void CancelTar() {

        //Potentially don't fully reset to the idle state if we're just selecting
        // a character, but not selecting targets for a skill of theirs

        //Re-enable general selections
        bCanSelectCharacters = true;

        SetState(new StateTargetIdle());

        //Let everything know that targetting has ended
        subAllFinishManualSelections.NotifyObs(this);
    }


    public void StartSelections(Skill _skillSelected) {

        // When we've clicked a skill, try to use that skill
        // There's a bunch of checks we have to do for this though first to ensure we should be selecting this skill

        // If this skill isn't owned by a locally-controlled client, then reject this selection
        if(_skillSelected.chrOwner.plyrOwner.inputController == null) {

            Debug.Log("We can't select skills for a character we don't locally control");
            return;
        }

        // Check if it's actually got a LocalInputType that will allow us to select a skill
        if(_skillSelected.chrOwner.plyrOwner.inputController.CanProceedWithSkillSelection() == false) {

            Debug.Log("This character is owner by a local player, but selection of skills is not available currently");
            return;
        }

        // Check if it's on cooldown
        if(_skillSelected.skillslot.IsOffCooldown() == false) {
            Debug.Log("We can't use a skill that's on cooldown");
            return;
        }

        if(!_skillSelected.chrOwner.curStateReadiness.CanSelectSkill(_skillSelected)) {
            Debug.Log("We can't use a skill right now cause the character's readiness state doesn't allow it");
            return;
        }

        //Start off a new selections structure to be filled in during the selection process
        selectionsInProgress = new Selections(_skillSelected);

        //Lock character selections (can unlock as needed depending on which targetting type we need to do)
        bCanSelectCharacters = false;

        //Transition to the appropriate state for gettings the selections for the first required target (likely mana cost payment)

        EnterNextSelectionState();


    }

    //The current selection 'state' should call this function as part of its submission process
    //  once it's found its selection
    public void ReceiveNextSelection(object objSelection) {

        if(selectionsInProgress.GetNextRequiredTarget().IsValidSelection(objSelection, selectionsInProgress) == false) {
            Debug.LogError("Received invalid target for " + selectionsInProgress.GetIndexOfNextRequiredTarget() + "th target");
            return;
        }

        //Save the selection
        selectionsInProgress.AddSelection(objSelection);

        //Move on to the next phase of selection
        EnterNextSelectionState();

    }

    public void EnterNextSelectionState() {

        //First, leave our current selection state (if this isn't the first target)
        if(selectionsInProgress.GetIndexOfNextRequiredTarget() != 0) {
            //Trigger our 'OnLeave' method for our current targetting state
            selectionsInProgress.GetMostRecentCompletedTarget().OnLocalEndSelection();
        }

        //Then, check if there are any targets we still need to fill in with selections
        if(selectionsInProgress.HasAllStoredSelections()) {
            Debug.Log("All required selections stored - should move to finishing targetting");
            FinishSelections();
            return;
        }


        //If we still have selections to fill in, then call the 'OnEnter' method for the
        // next selection state
        selectionsInProgress.GetNextRequiredTarget().OnLocalStartSelection();

    }


    public void FinishSelections() {

        //Only allow manual selections when the local player is human
        Debug.Assert(Match.Get().GetLocalPlayer().curInputType == Player.InputType.HUMAN,
            "Error - can only submit skills for locally-owned >human<'s characters");

        Debug.Assert(ClientNetworkController.Get().IsPlayerLocallyControlled(chrSelected.plyrOwner),
            "Error - can only submit skills for >locally-owned< human's characters");

        ContSkillSelection.Get().SubmitSkill(selectionsInProgress, chrSelected.plyrOwner.inputController);

        // Can now go back idle and wait for the next targetting (with a defaulted non-locked Idle state)
        SetState(new StateTargetIdle());

        //Let everything know that targetting has ended
        subAllFinishManualSelections.NotifyObs(this);
    }

    public void SetState(StateTarget newState) {

        if(curState != null) {
            Debug.Log("Leaving State " + curState.ToString());
            curState.OnLeave();
        }

        curState = newState;

        if(curState != null) {
            curState.OnEnter();
            Debug.Log("Entering State " + curState.ToString());
        }
    }

    public override void Init() {

        bCanSelectCharacters = true;
        SetState(new StateTargetIdle());

    }


    public ContLocalUIInteraction() {

        selectionsInProgress = null;
    }
}
