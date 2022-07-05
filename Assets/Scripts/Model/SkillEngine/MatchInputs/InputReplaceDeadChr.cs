using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReplaceDeadChr : MatchInput {
    public Chr chrDead;
    public Chr chrReplacingWith;

    //For creating a new skill selection collection to be filled out in the selection process
    public InputReplaceDeadChr(int _iPlayerActing, Chr _chrDead) : base(_iPlayerActing) {
        chrDead = _chrDead;
    }

    //For deserializing a network-provided serialized replacement selection.  This will just be a two
    // element array that holds the serializiation of the character who died and the character replacing them
    public InputReplaceDeadChr(int[] arnSerializedSelections) : base(arnSerializedSelections) {



        Debug.Assert(arnSerializedSelections.Length == 2,
            "Received " + arnSerializedSelections.Length + " selections when we only need to receive a two characters");


        chrDead = Serializer.DeserializeChr(arnSerializedSelections[0]);
        chrReplacingWith = Serializer.DeserializeChr(arnSerializedSelections[1]);

    }

    public InputReplaceDeadChr(InputReplaceDeadChr other) : base(other) {
        chrDead = other.chrDead;
        chrReplacingWith = other.chrReplacingWith;
    }

    public InputReplaceDeadChr GetCopy() {
        return new InputReplaceDeadChr(this);
    }

    public override int[] Serialize() {

        int[] arnSerializedSelections = new int[2];

        //All we need to do is serialize the character who's died, and the one we're replacing them with
        Debug.Assert(chrDead != null);
        Debug.Assert(chrReplacingWith != null);

        //First, add the character who died
        arnSerializedSelections[0] = Serializer.SerializeByte(chrDead);

        //Then, add the character being swapped in
        arnSerializedSelections[1] = Serializer.SerializeByte(chrReplacingWith);

        return arnSerializedSelections;
    }

    public override string ToString() {
        return string.Format("{0} to be replaced by {1}", chrDead.ToString(), chrReplacingWith == null ? "<no selection>" : chrReplacingWith.ToString());
    }

    public int GetIndexOfNextRequiredTarget() {
        return lstSelections.Count;
    }

    public Target GetMostRecentCompletedTarget() {
        if(GetIndexOfNextRequiredTarget() == 0) {
            Debug.LogError("Can't get the most recently completed target, since no targets have been completed yet");
            return null;
        }
        return skillSelected.lstTargets[GetIndexOfNextRequiredTarget() - 1];
    }

    public Target GetNextRequiredTarget() {
        return skillSelected.lstTargets[GetIndexOfNextRequiredTarget()];
    }

    public bool HasAllStoredSelections() {
        return GetIndexOfNextRequiredTarget() == skillSelected.lstTargets.Count;
    }

    public bool HasLegallyFilledTargets() {
        for(int i = 0; i < skillSelected.lstTargets.Count; i++) {
            if(skillSelected.lstTargets[i].CanSelect(lstSelections[i], this) == false) {
                //If any of the stored selections are invalid, then this isn't an initially-viable selection
                return false;
            }
        }

        return true;
    }

    public bool IsValidSelection() {

        return chrReplacingWith != null && chrReplacingWith.position.positiontype == Position.POSITIONTYPE.BENCH;
    }

    public bool IsGoodEnoughToExecute() {
        //TODO - consider what should constitute a valid-enough selection so as to still
        // be worth executing even if not all selections may still be valid
        // for now - just passing off to IsValidSelection to check if everything is still valid
        return IsValidSelection();
    }

    public void SetSkillSelection(SkillSlot skillslot) {
        skillslotSelected = skillslot;
    }

    public void AddSelection(object objSelected) {

        if(GetNextRequiredTarget().CanSelect(objSelected, this) == false) {
            Debug.LogError("Error! Tried to add selection for index " + GetIndexOfNextRequiredTarget() + " that was invalid");
            return;
        }

        Debug.LogFormat("Adding selection {0}", objSelected);

        lstSelections.Add(objSelected);
    }

    //A version of AddSelection that doesn't freak out over an invalid selection (likely be a scripted AI)
    public void AddPotentiallyInvalidSelection(object objSelected) {
        lstSelections.Add(objSelected);
    }

    public void FillWithRandomSelections() {

        //Ask each Target to randomly select a completely random choice
        //  Note - this bypasses the standard AddSelection method since it is okay to add invalid selections here
        for(int i = 0; i < skillSelected.lstTargets.Count; i++) {
            AddPotentiallyInvalidSelection(skillSelected.lstTargets[i].GetRandomSelectable());
        }
    }

    // Gives the nth most recent selection (n=0 gives most recent)
    public object GetNthPreviousSelection(int n) {

        Debug.Assert(n < lstSelections.Count);

        return lstSelections[lstSelections.Count - 1 - n];
    }



    public override IEnumerator Execute() {

        //First, we'll push a swap clause for the character we want to replace our dead one
        skillSelected.UseSkill();


        //Then, before that executes, we'll formally kill the character and clear out their character slot



        //Do a small delay for skill animations - note this uses ContTime's WaitForSeconds so that we adhere to any time-scale modifications like pausing
        yield return ContTime.Get().WaitForSeconds(ContTime.fDelayTurnSkill);

    }

    public override bool CanLegallyExecute() {
        if(chrDead == null) return false;
        if(chrReplacingWith == null) return false;

        if(chrReplacingWith.position.positiontype != Position.POSITIONTYPE.BENCH) {
            Debug.Log("Tried to select " + chrReplacingWith + " to swap in, but this character isn't on the bench");
            return false;
        }

        return true;
    }

    protected override void AttemptFillRandomly() {
        chrActing = ContTurns.Get().GetNextActingChr();

        int nMaxSelectionAttempts = 5;
        int nCurSelectionAttempt = 0;

        //We'll attempt a few selections to see if we can find something legal 
        while(nCurSelectionAttempt < nMaxSelectionAttempts) {
            nCurSelectionAttempt++;

            //Select a random skill we have that's off cooldown
            skillslotSelected = chrActing.arSkillSlots[Random.Range(0, Chr.nEquippedCharacterSkills)];
            lstSelections = new List<object>();

            //If the skill can't be activated for whatever reason (like being a passive), then skip to the next attempt
            if(skillslotSelected.chrOwner.curStateReadiness.CanSelectSkill(skillslotSelected.skill) == false) continue;

            //If the skill is on cooldown, then we'll skip to the next attempt
            if(skillslotSelected.IsOffCooldown() == false) continue;

            //For each target we have to fill out, get a random selectable for its targetting type
            bool bFailedSelection = false;

            for(int i = 0; i < skillslotSelected.skill.lstTargets.Count; i++) {
                //Debug.LogFormat("Skill {0} is asking for selections for its {1}th target, {2}", skillSelected, i, skillSelected.lstTargets[i]);
                if(skillSelected.lstTargets[i].HasAValidSelectable(this) == false) {
                    bFailedSelection = true;
                    break;
                }
                //If there's at least something selectable, then pick one of them randomly
                lstSelections.Add(skillSelected.lstTargets[i].GetRandomValidSelectable(this));
            }

            if(bFailedSelection) {
                //If we failed finding a selection for some skill, then continue on in the loop to find a different skill selection
                //Before we move on to the next selection attempt, clear out any reserved mana amounts
                chrActing.plyrOwner.manapool.ResetReservedMana();
                continue;
            }

            //If we reached this far without failing a selection, then we should have a fully filled out random selection so we can return
            Debug.AssertFormat(CanLegallyExecute(), "{0} is an invalid random selection", ToString());
            return;
        }

        //If we tried many times and couldn't get a valid selection, then we'll just reset the default input
        ResetToDefaultInput();
    }



    public void ResetToRestSelection() {
        chrActing = ContTurns.Get().GetNextActingChr();
        skillslotSelected = chrActing.skillRest.skillslot;
        lstSelections = new List<object>();
    }

    public override void ResetToDefaultInput() {
        //Just set ourselves to a rest action since that's guaranteed to be a legal selection no matter what
        ResetToRestSelection();
    }

    // Clear out all non-essential data that could have been partially filled out from an incomplete selections process
    public override void ResetPartialSelection() {
        //Keep the acting character the same

        //Unreserve any reserved mana we have set aside while selecting mana costs for this skill
        chrActing.plyrOwner.manapool.ResetReservedMana();

        //Reset the skill choice (since the player may want to change their choice)
        skillslotSelected = null;

        //Reset all the selections for the skill's targets
        lstSelections = null;
    }

    //Set up any UI for prompting the selection of a skill and unlock the capability for the local player to go through the 
    //  target selection process
    public override void StartManualInputProcess() {

        //Debug.Log("Starting manual input for skillselection");
        //In this case, we're just going to pass off control to the local controller by letting it know we
        //  want to be selecting a skill
        chrActing.plyrOwner.inputController.StartSelection();
    }


    //Clean up any UI for prompting the selection of a skill and re-lock the ability for the local player to go through the
    //   target selection process
    public override void EndManualInputProcess() {

        //Debug.Log("Ending manual input for skillselection");
        //Have the localinputController clean up it's selection-related UI
        chrActing.plyrOwner.inputController.EndSelection();
    }
}
