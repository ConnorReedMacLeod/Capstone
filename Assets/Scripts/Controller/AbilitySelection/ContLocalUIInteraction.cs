using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class handles the UI interaction flow for clicking on characters/skills/selections
//
// It maintains the state of the interaction (i.e. if we're not currently selecting any
//  character, if we're selecting a character and showing their skills, if we're selecting
//  the target for a particular skill (and which type of entity we're trying to select dependent
//  on the ability.
//
// Will often consult the LocalInputType to see what interactions are possible to proceed
//  with given the control-level we have for the local player (i.e., if the local
//  player is human then we have permission to move ahead with selecting abilities.  If
//  the local player is an AI, then we should only be able to click characters and hover over
//  skills to see information on them, but not to take any action with them.
public class ContLocalUIInteraction : Singleton<ContLocalUIInteraction> {

    public StateTarget curState;

    public Chr chrSelected;

    public Action actSelected;

    //Note - for now, we're assuming we'll only ever target one thing (or, more broadly,
    //  require one click for finalizing selection of an ability's targets).  If this ever changes,
    //  we can make a list of needed selection types and record the chosen selections here

    public static Subject subAllStartTargetting = new Subject(Subject.SubType.ALL);
    public static Subject subAllFinishTargetting = new Subject(Subject.SubType.ALL);

    // Start a new round of targetting
    public void ResetTar() {
        //Clear any previous targetting information we had
        actSelected = null;

    }

    // Ends targetting
    public void CancelTar() {

        //Potentially don't fully reset to the idle state if we're just selecting
        // a character, but not selecting targets for an ability of theirs

        SetState(new StateTargetIdle());

        //Let everything know that targetting has ended
        subAllFinishTargetting.NotifyObs(this);
    }

    public void SelectCharacter(Chr _chrSelected) {

        //We've clicked on a character to select them
        chrSelected = _chrSelected;

        SetState(new StateTargetSelected());

    }

    public void StartTargetting(Action _actSelected) {

        // When we've clicked an action, try to use that action
        // There's a bunch of checks we have to do for this though first to ensure we should be selecting this ability

        // If this ability isn't owned by a locally-controlled client, then reject this selection
        if(_actSelected.chrSource.plyrOwner.inputController == null) {

            Debug.Log("We can't select actions for a character we don't locally control");
            return;
        }

        // Check if it's actually got a LocalInputType that will allow us to select an ability
        if(_actSelected.chrSource.plyrOwner.inputController.CanProceedWithSkillSelection()) {

            Debug.Log("This character is owner by a local player, but selection of abilities is not available currently");
            return;
        }

        // Check if it's on cooldown
        if(_actSelected.nCurCD > 0) {
            Debug.Log("We can't use an ability that's on cooldown");
            return;
        }

        if(!_actSelected.chrSource.curStateReadiness.CanSelectAction(_actSelected)) {
            Debug.Log("We can't use an action right now cause our state doesn't allow it");
            return;
        }

        actSelected = _actSelected;


        //If we've gotten this far, then the ability should be valid (maybe not mana - need to consult the target first),
        //  so we need to transition into the state that listens for that type of target

        switch(_actSelected.GetTargetType()) {

        case Clause.TargetType.CHR:
            SetState(new StateTargetChr());
            break;

        case Clause.TargetType.PLAYER:
            SetState(new StateTargetTeam());
            break;

        default:
            Debug.LogError("Unsupported selection type of " + _actSelected.GetTargetType());
            break;
        }


    }

    public void FinishTargetting(SelectionSerializer.SelectionInfo infoSelected) {

        //Ensure the submission matches out local information
        Debug.Assert(chrSelected == infoSelected.chrOwner);
        Debug.Assert(actSelected == infoSelected.actUsed);

        //Only allow manual selections when the local player is human
        Debug.Assert(Match.Get().GetLocalPlayer().curInputType == Player.InputType.HUMAN);
        Debug.Assert(chrSelected.plyrOwner.id == ClientNetworkController.Get().nLocalPlayerID, "Error - can only submit abilities for locally-owned human's characters");

        ContAbilitySelection.Get().SubmitAbility(infoSelected, chrSelected.plyrOwner.inputController);

        // Can now go back idle and wait for the next targetting
        SetState(new StateTargetIdle());

        //Let everything know that targetting has ended
        subAllFinishTargetting.NotifyObs(this);
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

        SetState(new StateTargetIdle());

    }


    public ContLocalUIInteraction() {

        actSelected = null;
    }
}
