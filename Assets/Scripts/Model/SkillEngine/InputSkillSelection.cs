using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSkillSelection : MatchInput {
    public Chr chrActing;
    public Skill skillSelected;
    public List<object> lstSelections;

    //For creating a new skill selection collection to be filled out in the selection process
    public InputSkillSelection(int _iPlayerActing, Chr _chrActing, Skill _skSelected) : base(_iPlayerActing) {
        chrActing = _chrActing;
        skillSelected = _skSelected;
        lstSelections = new List<object>();
    }

    //For deserializing a network-provided serialized skill selection (including targets) into their corresponding objects
    // By convention, the array leads with the acting character's global id, then the chosen skill slot, with the remaining
    // entries corresponding to selections for that skill's targets
    public InputSkillSelection(int[] arnSerializedSelections) : base(arnSerializedSelections) {

        chrActing = Chr.GetTargetByIndex(arnSerializedSelections[0]);
        skillSelected = Serializer.DeserializeSkill(arnSerializedSelections[1]);

        Debug.Assert(skillSelected.lstTargets.Count == arnSerializedSelections.Length - 2,
            "Received " + (arnSerializedSelections.Length - 2) + " selections for a skill requiring " + skillSelected.lstTargets.Count);

        lstSelections = new List<object>();

        //For each required target, have it decode the network-provided serialization
        for (int i = 0; i < skillSelected.lstTargets.Count; i++) {
            // Ask the corresponding Target to decode the serialized int we've been provided
            // i+1 since the first entry of the serialized array refers to the chosen skill, and not the selections
            lstSelections.Add(skillSelected.lstTargets[i].Unserialize((int)arnSerializedSelections[i + 2], lstSelections));
        }
    }

    public InputSkillSelection(InputSkillSelection other) : base(other) {
        chrActing = other.chrActing;
        skillSelected = other.skillSelected;
        lstSelections = other.lstSelections;
    }

    public InputSkillSelection GetCopy() {
        return new InputSkillSelection(this);
    }

    public static InputSkillSelection GetRestSelection(Chr chr) {
        return new InputSkillSelection(chr.plyrOwner.id, chr, chr.skillRest);
    }

    public void ResetToRestSelection() {
        skillSelected = skillSelected.chrOwner.skillRest;
        lstSelections = new List<object>();
    }

    public int GetSerializedSkill() {
        return Serializer.SerializeSkill(skillSelected);
    }

    public override int[] Serialize() {

        int[] arnSerializedSelections = new int[skillSelected.lstTargets.Count + 2];

        //First, add the character who's set to be acting
        arnSerializedSelections[0] = TarChr.SerializeChr(chrActing);

        //Second, add the serialization of the use skill
        arnSerializedSelections[1] = Serializer.SerializeSkill(skillSelected);

        //Then add all the selections afterward
        for (int i = 0; i < skillSelected.lstTargets.Count; i++) {
            //For each Target, ask it how we should serialize the selected object we have stored
            // Note - i+1, since we're adding all selections after the used skill
            arnSerializedSelections[i + 2] = skillSelected.lstTargets[i].Serialize(lstSelections[i]);
        }

        return arnSerializedSelections;
    }

    public override string ToString() {
        string s = chrActing.ToString() + " " + skillSelected.ToString() + " - ";

        for (int i = 0; i < skillSelected.lstTargets.Count; i++) {

            //If we have a selection for this slot, fill it in with the description of that target,
            //   otherwise, just leave it blank
            if (i < lstSelections.Count) {
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
        if (GetIndexOfNextRequiredTarget() == 0) {
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
        for (int i = 0; i < skillSelected.lstTargets.Count; i++) {
            if (skillSelected.lstTargets[i].IsValidSelection(lstSelections[i], this) == false) {
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


    public void AddSelection(object objSelected) {

        if (GetNextRequiredTarget().IsValidSelection(objSelected, this) == false) {
            Debug.LogError("Error! Tried to add selection for index " + GetIndexOfNextRequiredTarget() + " that was invalid");
            return;
        }

        lstSelections.Add(objSelected);
    }

    //A version of AddSelection that doesn't freak out over an invalid selection (likely be a scripted AI)
    public void AddPotentiallyInvalidSelection(object objSelected) {
        lstSelections.Add(objSelected);
    }

    public void FillWithRandomSelections() {

        //Ask each Target to randomly select a completely random choice
        //  Note - this bypasses the standard AddSelection method since it is okay to add invalid selections here
        for (int i = 0; i < skillSelected.lstTargets.Count; i++) {
            AddPotentiallyInvalidSelection(skillSelected.lstTargets[i].GetRandomSelectable());
        }
    }

    // Gives the nth most recent selection (n=0 gives most recent)
    public object GetNthPreviousSelection(int n) {

        Debug.Assert(n < lstSelections.Count);

        return lstSelections[lstSelections.Count - 1 - n];
    }



    public override IEnumerator Execute() {

        Debug.Log("Executing " + skillSelected.ToString());

        //For a standard skill usage, we need to use the skill using the stored selections we have accrued
        skillSelected.UseSkill();

        //Note - We are currently having each clause of a skill grab input directly from the networkreceiver buffer, but it would also
        //       be possible to copy clauses and embed the selections directly into them.  

        yield return new WaitForSeconds(0.1f);
    }
}
