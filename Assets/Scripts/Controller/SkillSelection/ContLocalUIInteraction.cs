﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class handles the UI interaction flow for clicking on characters/skills/selections
//
// It maintains the state of the interaction (i.e. if we're not currently selecting any
//  character, if we're selecting a character and showing their skills, if we're selecting
//  the target for a particular skill (and which type of entity we're trying to select dependent
//  on the skill.
//
// Will often consult the LocalInputType to see what interactions are possible to proceed
//  with given the control-level we have for the local player (i.e., if the local
//  player is human then we have permission to move ahead with selecting skills.  If
//  the local player is an AI, then we should only be able to click characters and hover over
//  skills to see information on them, but not to take any action with them.
public class ContLocalUIInteraction : Singleton<ContLocalUIInteraction> {

    public StateTarget curState;

    public Chr chrSelected;

    public Skill skillSelected;

    //Note - for now, we're assuming we'll only ever target one thing (or, more broadly,
    //  require one click for finalizing selection of an skill's targets).  If this ever changes,
    //  we can make a list of needed selection types and record the chosen selections here

    public static Subject subAllStartManualTargetting = new Subject(Subject.SubType.ALL);
    public static Subject subAllFinishManualTargetting = new Subject(Subject.SubType.ALL);

    // Start a new round of targetting
    public void ResetTar() {
        //Clear any previous targetting information we had
        skillSelected = null;

    }

    // Ends targetting
    public void CancelTar() {

        //Potentially don't fully reset to the idle state if we're just selecting
        // a character, but not selecting targets for a skill of theirs

        SetState(new StateTargetIdle());

        //Let everything know that targetting has ended
        subAllFinishManualTargetting.NotifyObs(this);
    }

    public void SelectCharacter(Chr _chrSelected) {

        //We've clicked on a character to select them
        chrSelected = _chrSelected;

        SetState(new StateTargetSelected());

    }

    public void StartTargetting(Skill _skillSelected) {

        // When we've clicked a skill, try to use that skill
        // There's a bunch of checks we have to do for this though first to ensure we should be selecting this skill

        // If this skill isn't owned by a locally-controlled client, then reject this selection
        if(_skillSelected.chrSource.plyrOwner.inputController == null) {

            Debug.Log("We can't select skills for a character we don't locally control");
            return;
        }

        // Check if it's actually got a LocalInputType that will allow us to select a skill
        if(_skillSelected.chrSource.plyrOwner.inputController.CanProceedWithSkillSelection() == false) {

            Debug.Log("This character is owner by a local player, but selection of skills is not available currently");
            return;
        }

        // Check if it's on cooldown
        if(_skillSelected.skillslot.IsOffCooldown() == false) {
            Debug.Log("We can't use a skill that's on cooldown");
            return;
        }

        if(!_skillSelected.chrSource.curStateReadiness.CanSelectSkill(_skillSelected)) {
            Debug.Log("We can't use a skill right now cause our state doesn't allow it");
            return;
        }

        skillSelected = _skillSelected;


        //If we've gotten this far, then the skill should be valid (maybe not mana - need to consult the target first),
        //  so we need to transition into the state that listens for that type of target

        switch(skillSelected.GetTargetType()) {

        case Clause.TargetType.CHR:
            SetState(new StateTargetChr());
            break;

        case Clause.TargetType.PLAYER:
            SetState(new StateTargetTeam());
            break;

        case Clause.TargetType.SPECIAL:
            //TODO - eventually figure out how to handle specialized custom selections for skills
            //        i.e., bringing up a custom menu to select an option from
            Debug.Log("Choosing a special skill - serialize and submit it right away");

            //Make a SelectionInfo package for this skill
            SelectionSerializer.SelectionSpecial infoSelectionSpec =
                new SelectionSerializer.SelectionSpecial(
                    skillSelected.chrSource,
                    skillSelected);

            if(infoSelectionSpec.CanSelect() == false) {
                Debug.Log("This special skill isn't valid to select");
                SetState(new StateTargetIdle());

            } else {
                FinishTargetting(infoSelectionSpec);
            }


            break;
        default:
            Debug.LogError("Unsupported selection type of " + skillSelected.GetTargetType());
            break;
        }


    }

    public void FinishTargetting(SelectionSerializer.SelectionInfo infoSelected) {

        //Ensure the submission matches our local information
        Debug.Assert(chrSelected == infoSelected.chrOwner);
        Debug.Assert(skillSelected == infoSelected.skillUsed);

        //Only allow manual selections when the local player is human
        Debug.Assert(Match.Get().GetLocalPlayer().curInputType == Player.InputType.HUMAN,
            "Error - can only submit skills for locally-owned human's characters");

        Debug.Assert(ClientNetworkController.Get().IsPlayerLocallyControlled(chrSelected.plyrOwner),
            "Error - can only submit skills for locally-owned human's characters");

        ContSkillSelection.Get().SubmitSkill(infoSelected, chrSelected.plyrOwner.inputController);

        // Can now go back idle and wait for the next targetting
        SetState(new StateTargetIdle());

        //Let everything know that targetting has ended
        subAllFinishManualTargetting.NotifyObs(this);
    }

    public void SetState(StateTarget newState) {

        if(curState != null) {
            //Debug.Log("Leaving State " + curState.ToString());
            curState.OnLeave();
        }

        curState = newState;

        if(curState != null) {
            curState.OnEnter();
            //Debug.Log("Entering State " + curState.ToString());
        }
    }

    public override void Init() {

        SetState(new StateTargetIdle());

    }


    public ContLocalUIInteraction() {

        skillSelected = null;
    }
}
