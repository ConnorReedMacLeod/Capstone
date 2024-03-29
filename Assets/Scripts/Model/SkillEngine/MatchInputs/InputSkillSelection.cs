﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSkillSelection : MatchInput {
    public Chr chrActing;
    public SkillSlot skillslotSelected;

    //Note: we strictly only store the skillslot that was selected (since this will always be usable
    //      even if the skill that was in the slot was swapped to something unexpected before execution).  
    //      We'll still provide the interface to just directly refer to the skill itself since that's convenient
    public Skill skillSelected {
        get { return skillslotSelected.skill; }
    }

    public List<object> lstSelections;

    //For creating a new skill selection collection to be filled out in the selection process
    public InputSkillSelection(Player plyrActing, Chr _chrActing, SkillSlot _skillslotSelected) : base(plyrActing) {
        chrActing = _chrActing;
        skillslotSelected = _skillslotSelected;
        lstSelections = new List<object>();
    }

    //For deserializing a network-provided serialized skill selection (including targets) into their corresponding objects.
    // The serialization array's elements are as follows:
    // 0: The MatchInputType (as an enum)
    // 1: The player set to act
    // 2: The character set to act
    // 3: The skillslot that has been selected to be used
    // 4...: Any extra targetting arguments that the skill needs to execute
    public InputSkillSelection(int[] arnSerializedSelections) : base(arnSerializedSelections) {

        //Verify we are decoding an input that matches our MatchInputType
        Debug.Assert((int)GetMatchInputType() == arnSerializedSelections[0]);

        plyrActing = Serializer.DeserializePlayer((byte)arnSerializedSelections[1]);
        chrActing = Serializer.DeserializeChr(arnSerializedSelections[2]);
        skillslotSelected = Serializer.DeserializeSkillSlot(arnSerializedSelections[3]);

        Debug.Assert(skillSelected.lstTargets.Count == arnSerializedSelections.Length - 4,
            "Received " + (arnSerializedSelections.Length - 4) + " selections for a skill requiring " + skillSelected.lstTargets.Count);

        lstSelections = new List<object>();

        //For each required target, have it decode the network-provided serialization
        for(int i = 0; i < skillSelected.lstTargets.Count; i++) {
            Debug.Log("Adding serialization entry " + (i + 4) + " of skill " + skillSelected.sName);
            // Ask the corresponding Target to decode the serialized int we've been provided
            // i+1 since the first entry of the serialized array refers to the chosen skill, and not the selections
            lstSelections.Add(skillSelected.lstTargets[i].Unserialize((int)arnSerializedSelections[i + 4], lstSelections));
        }
    }

    public InputSkillSelection(InputSkillSelection other) : base(other) {
        chrActing = other.chrActing;
        skillslotSelected = other.skillslotSelected;
        lstSelections = other.lstSelections;
    }

    public InputSkillSelection GetCopy() {
        return new InputSkillSelection(this);
    }

    public override MatchInputType GetMatchInputType() {
        return MatchInputType.SkillSelection;
    }

    public override int[] Serialize() {

        int[] arnSerializedSelections = new int[skillSelected.lstTargets.Count + 4];

        //For all serialized inputs, we will start our serialization array off with an int representing the type of input we're recording
        arnSerializedSelections[0] = (int)GetMatchInputType();

        //Now we can start serializing the actual data for this input

        //First, add the player who's set to be acting
        arnSerializedSelections[1] = Serializer.SerializeByte(plyrActing);

        //Second, add the character who's set to be acting
        arnSerializedSelections[2] = Serializer.SerializeByte(chrActing);

        //Third, add the serialization of the use skill
        arnSerializedSelections[3] = Serializer.SerializeByte(skillslotSelected);

        //Then add all the selections afterward
        for(int i = 0; i < skillSelected.lstTargets.Count; i++) {
            //For each Target, ask it how we should serialize the selected object we have stored
            // Note - i+1, since we're adding all selections after the used skill
            arnSerializedSelections[i + 4] = skillSelected.lstTargets[i].Serialize(lstSelections[i]);
        }

        return arnSerializedSelections;
    }

    public override string ToString() {
        string s = chrActing.ToString() + " " + skillSelected.ToString() + " - ";

        for(int i = 0; i < skillSelected.lstTargets.Count; i++) {

            //If we have a selection for this slot, fill it in with the description of that target,
            //   otherwise, just leave it blank
            if(i < lstSelections.Count) {
                s += lstSelections[i].ToString() + ", ";
            } else {
                s += "(Unselected), ";
            }
        }
        return s;
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

        return skillSelected.CanSelect(this);
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


    // Gives the nth most recent selection (n=0 gives most recent)
    public object GetNthPreviousSelection(int n) {

        Debug.Assert(n < lstSelections.Count);

        return lstSelections[lstSelections.Count - 1 - n];
    }



    public override IEnumerator Execute() {

        //For a standard skill usage, we need to use the skill using the stored selections we have accrued
        skillSelected.UseSkill();

        //Note - We are currently having each clause of a skill grab input directly from the networkreceiver buffer, but it would also
        //       be possible to copy clauses and embed the selections directly into them.  

        //Do a small delay for skill animations - note this uses ContTime's WaitForSeconds so that we adhere to any time-scale modifications like pausing
        yield return ContTime.Get().WaitForSeconds(ContTime.fDelayTurnSkill);

    }

    public override bool CanLegallyExecute() {
        if(chrActing == null) return false;
        if(skillslotSelected == null) return false;

        if(skillslotSelected.chrOwner != chrActing) {
            Debug.Log("Tried to select " + skillslotSelected.chrOwner + "'s " + skillslotSelected + " but we want " + chrActing + " to act");
            return false;
        }

        //As long as we can execute the filled out skill with this selection, then we're good enough to execute this selection
        if(skillSelected.CanSelect(this) == false) return false;

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
            skillslotSelected = chrActing.arSkillSlots[ContRandomization.Get().GetRandom(0, Chr.nEquippedCharacterSkills)];
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
    //  target selection process for a skill
    public override void StartManualInputProcess(LocalInputHuman localinputhuman) {

        Debug.Log("Starting manual input for skillselection");

        localinputhuman.bCurrentlySelectingSkill = true;

        ContTurns.Get().chrNextReady.subBecomesActiveForHumans.NotifyObs();
    }


    //Clean up any UI for prompting the selection of a skill and re-lock the ability for the local player to go through the
    //   target selection process
    public override void EndManualInputProcess(LocalInputHuman localinputhuman) {

        Debug.Log("Ending manual input for skillselection");

        localinputhuman.bCurrentlySelectingSkill = false;

        ContTurns.Get().chrNextReady.subEndsActiveForHumans.NotifyObs();
    }
}
