using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SelectionSerializer {

    public static byte GetByte(int nPos, int nSerialized) {
        //Positions are (0, 1, 2, 3)

        return (byte)((nSerialized & (255 << (8 * (3 - nPos)))) >> (8 * (3 - nPos)));
    }

    public static int Serialize(byte b1, byte b2, byte b3, byte b4) {
        return (b1 << 24) + (b2 << 16) + (b3 << 8) + b4;
    }

    public static Skill PeekSkill(Chr chrOwner, int nSerialized) {
        return DeserializeSkill(chrOwner, GetByte(0, nSerialized));
    }

    public static byte SerializeByte(Chr chr) {
        return (byte)chr.globalid;
    }
    public static Chr DeserializeChr(byte b) {
        return Chr.lstAllChrs[b];
    }

    public static byte SerializeByte(Skill skill) {
        return (byte)skill.skillslot.iSlot;
    }
    public static Skill DeserializeSkill(Chr chrOwner, byte b) {
        return chrOwner.arSkillSlots[b].skill;
    }

    public static byte SerializeByte(Player plyr) {
        return (byte)plyr.id;
    }

    public static Player DeserializePlayer(byte b) {
        return Player.arAllPlayers[b];
    }

}



public class Selections {

    public Skill skillSelected;
    public List<object> lstSelections;

    //For creating a new Selections collection to be filled out in the selection process
    public Selections(Skill _skSelected) {
        skillSelected = _skSelected;
        lstSelections = new List<object>();
    }

    //For deserializing a master-provided set of serialized selections into their corresponding objects
    // By convention, the leading entry of the array corresponds to the chosen skill, with the remaining
    // entries corresponding to selections for that skill's targets
    public Selections(int[] arnSerializedSelections) {


        skillSelected = DeserializeSkill(arnSerializedSelections[0]);

        Debug.Assert(skillSelected.lstTargets.Count == arnSerializedSelections.Length - 1,
            "Received " + (arnSerializedSelections.Length - 1) + " selections for a skill requiring " + skillSelected.lstTargets.Count);

        lstSelections = new List<object>();

        //For each required target, have it decode the master-provided serialization
        for(int i = 0; i < skillSelected.lstTargets.Count; i++) {
            // Ask the corresponding Target to decode the serialized int we've been provided
            // i+1 since the first entry of the serialized array refers to the chosen skill, and not the selections
            lstSelections.Add(skillSelected.lstTargets[i].Unserialize((int)arnSerializedSelections[i + 1]));
        }
    }

    public Selections(Selections other) {
        skillSelected = other.skillSelected;
        lstSelections = other.lstSelections;
    }

    public Selections GetCopy() {
        return new Selections(this);
    }

    public static Selections GetRestSelection(Chr chr) {
        return new Selections(chr.skillRest);
    }

    public void ResetToRestSelection() {
        skillSelected = skillSelected.chrOwner.skillRest;
        lstSelections = new List<object>();
    }

    public static int SerializeSkill(Skill skill) {
        return SelectionSerializer.Serialize((byte)skill.chrOwner.globalid, (byte)skill.skillslot.iSlot, 0, 0);
    }

    public static Skill DeserializeSkill(int nSerializedSkill) {
        Chr chrUsing = SelectionSerializer.DeserializeChr(SelectionSerializer.GetByte(0, nSerializedSkill));
        int iSkillSlot = SelectionSerializer.GetByte(1, nSerializedSkill);

        return chrUsing.arSkillSlots[iSkillSlot].skill;
    }

    public int GetSerializedSkill() {
        return SerializeSkill(skillSelected);
    }

    public int[] GetSerialization() {

        int[] arnSerializedSelections = new int[skillSelected.lstTargets.Count + 1];

        //First, add the serialization of the use skill
        arnSerializedSelections[0] = GetSerializedSkill();

        //Then add all the selections aftererward
        for(int i = 0; i < skillSelected.lstTargets.Count; i++) {
            //For each Target, ask it how we should serialize the selected object we have stored
            // Note - i+1, since we're adding all selections after the used skill
            arnSerializedSelections[i + 1] = skillSelected.lstTargets[i].Serialize(lstSelections[i]);
        }

        return arnSerializedSelections;
    }

    public override string ToString() {
        string s = skillSelected.ToString() + " - ";

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

    public bool IsValidSelection() {

        for(int i = 0; i < skillSelected.lstTargets.Count; i++) {
            if(skillSelected.lstTargets[i].IsValidSelection(lstSelections[i], this) == false) {
                //If any of the stored selections are invalid, then this isn't an initially-viable selection
                return false;
            }
        }

        return true;
    }

    public bool IsGoodEnoughToExecute() {
        //TODO - consider what should constitute a valid-enough selection so as to still
        // be worth executing even if not all selections may still be valid
        // for now - just passing off to IsValidSelection to check if everything is still valid
        return IsValidSelection();
    }


    public void AddSelection(object objSelected) {

        if(GetNextRequiredTarget().IsValidSelection(objSelected, this) == false) {
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
        for(int i = 0; i < skillSelected.lstTargets.Count; i++) {
            AddPotentiallyInvalidSelection(skillSelected.lstTargets[i].GetRandomSelectable());
        }

    }

}
