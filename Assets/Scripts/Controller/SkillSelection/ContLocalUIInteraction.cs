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

    //The current character-selection state (If we're selecting any character and, if so, which character)
    public StateTarget curState;

    public bool bCanSelectCharacters;

    public Chr chrSelected;

    public static Subject subAllStartManualSelections = new Subject(Subject.SubType.ALL);
    public static Subject subAllFinishManualSelections = new Subject(Subject.SubType.ALL);

    //We hold a locally-filled out InputSkillSelection that is initialized based on ContSkillEngine's matchinputToFillOut
    public InputSkillSelection selectionsInProgress;


    // Cancel the selections phase early (without fully selecting all targets for the skil)
    public void CancelSelectionsProcess() {

        //In case we've reserved any mana for mana costs, let's un-reserve that amount
        selectionsInProgress.chrActing.plyrOwner.manapool.ResetReservedMana();

        //Now end the selections process normally
        ExitSelectionsProcess();
    } 

    // Ends the selections phase
    public void ExitSelectionsProcess() {

        //Potentially don't fully reset to the idle state if we're just selecting
        // a character, but not selecting targets for a skill of theirs

        //Re-enable general selections
        bCanSelectCharacters = true;

        //Clear out our locally-progressing skill selections
        selectionsInProgress = null;

        SetState(new StateTargetIdle());

        //Let everything know that targetting has ended
        subAllFinishManualSelections.NotifyObs(this);
    }


    public void ChooseSkillToSelect(Skill _skillSelected) {

        // When we've clicked a skill, try to use that skill
        // There's a bunch of checks we have to do for this though first to ensure we should be selecting this skill

        // Check if we're already in the process of selecting for a skill
        if(selectionsInProgress != null) {
            Debug.Log("We are already in the process of selecting targets for a skill, so we can't start the selections process for another skill");
            return;
        }

        // If this skill isn't owned by a locally-controlled client, then reject this selection
        if (_skillSelected.chrOwner.plyrOwner.inputController == null) {

            Debug.Log("We can't select skills for a character we don't locally control");
            return;
        }

        // Check if it's actually got a LocalInputType that will allow us to select a skill
        // (i.e., if we have asked this locally-controlled client to select a skill for us
        if (_skillSelected.chrOwner.plyrOwner.inputController.CanProceedWithSkillSelection() == false) {

            Debug.Log("This character is owned by a local player, but selecting targets for a new skill is not available currently");
            return;
        }

        if (((InputSkillSelection)ContSkillEngine.Get().matchinputToFillOut).chrActing != _skillSelected.chrOwner) { 
            Debug.Log("Can't start selections for a character who's not expected to be acting now");
            return;
        }

        //If the skill we've selected doesn't belong to the next acting character, then we can't proceed with selecting it
        if (selectionsInProgress.chrActing != _skillSelected.chrOwner) {
            Debug.Log("Can't select skills for a character who isn't currently acting");
            return;
        }

        // Check if it's on cooldown
        if(_skillSelected.skillslot.IsOffCooldown() == false) {
            Debug.Log("We can't use a skill that's on cooldown");
            return;
        }

        if(!_skillSelected.chrOwner.curStateReadiness.CanSelectSkill(_skillSelected)) {
            Debug.Log("We can't use a skill right now cause the character's readiness state doesn't allow it"); //(like trying to use a second active)
            return;
        }

        //Initiallize our local selectionsInProgress to be choosing selections for the targets of the provided skill
        selectionsInProgress = new InputSkillSelection(_skillSelected.chrOwner.plyrOwner.id, _skillSelected.chrOwner, _skillSelected.skillslot);

        //Lock character selections (can unlock as needed depending on which targetting type we need to do)
        bCanSelectCharacters = false;

        //Set our character-highlighting state to just be idle
        SetState(new StateTargetIdle());

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

        Debug.Log("Saving selection");
        //Save the selection
        selectionsInProgress.AddSelection(objSelection);

        Debug.Log("Entering Next Selection State after receiving a valid selection");
        //Move on to the next phase of selection
        EnterNextSelectionState();

    }

    public void EnterNextSelectionState() {

        //First, leave our current selection state (if this isn't the first target)
        if(selectionsInProgress.GetIndexOfNextRequiredTarget() != 0) {
            //Trigger our 'OnLeave' method for our current targetting state
            selectionsInProgress.GetMostRecentCompletedTarget().EndLocalSelection();
        }

        //Then, check if there are any targets we still need to fill in with selections
        if(selectionsInProgress.HasAllStoredSelections()) {
            Debug.Log("All required selections stored - should move to finishing targetting");
            FinishSelections();
            return;
        }


        //If we still have selections to fill in, then call the 'OnEnter' method for the
        // next selection state
        selectionsInProgress.GetNextRequiredTarget().StartLocalSelection(selectionsInProgress);

    }


    public void FinishSelections() {

        //Only allow manual selections when the local player is human
        Debug.Assert(ContTurns.Get().GetNextActingChr().plyrOwner.curInputType == Player.InputType.HUMAN,
            "Error - can only submit skills for locally-owned >human<'s characters");

        Debug.Assert(ClientNetworkController.Get().IsPlayerLocallyControlled(ContTurns.Get().GetNextActingChr().plyrOwner),
            "Error - can only submit skills for >locally-owned< human's characters");


        //By this point, we have built up the matchinputToFillOut into a valid selection of targets, so let's submit it
        NetworkSender.Get().SendNextInput(ContSkillEngine.Get().matchinputToFillOut);

        //Clean up the selection process (clears out the stored selections structure, sends notifications, etc.)
        ExitSelectionsProcess();
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
