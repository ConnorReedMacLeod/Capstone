﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReplaceEmptyPos : MatchInput {
    public Position posEmpty;
    public Chr chrReplacingWith;

    //For creating a new skill selection collection to be filled out in the selection process
    public InputReplaceEmptyPos(Player plyrActing, Position _posEmpty) : base(plyrActing) {
        posEmpty = _posEmpty;

        Debug.Assert(posEmpty.positiontype != Position.POSITIONTYPE.BENCH);
    }

    //For deserializing a network-provided serialized replacement selection.  This will just be a two
    // element array that holds the serializiation of the empty position and the character to fill that position
    public InputReplaceEmptyPos(int[] arnSerializedSelections) : base(arnSerializedSelections) {

        Debug.Assert(arnSerializedSelections.Length == 2,
            "Received " + arnSerializedSelections.Length + " selections when we only need to receive a two characters");


        posEmpty = ContPositions.Get().GetPosition(Position.UnserializeCoords(arnSerializedSelections[0]));
        chrReplacingWith = Serializer.DeserializeChr(arnSerializedSelections[1]);

    }

    public InputReplaceEmptyPos(InputReplaceEmptyPos other) : base(other) {
        posEmpty = other.posEmpty;
        chrReplacingWith = other.chrReplacingWith;
    }

    public InputReplaceEmptyPos GetCopy() {
        return new InputReplaceEmptyPos(this);
    }

    public override int[] Serialize() {

        int[] arnSerializedSelections = new int[2];

        //All we need to do is serialize the empty position, and the one we're replacing them with
        Debug.Assert(posEmpty.chrOnPosition == null);
        Debug.Assert(chrReplacingWith != null);

        //First, add the empty position that needs to be filled
        arnSerializedSelections[0] = Position.SerializeCoords(posEmpty.coords);

        //Then, add the character being swapped in
        arnSerializedSelections[1] = Serializer.SerializeByte(chrReplacingWith);

        return arnSerializedSelections;
    }

    public override string ToString() {
        return string.Format("{0} to be replaced by {1}", posEmpty.ToString(), chrReplacingWith == null ? "<no selection>" : chrReplacingWith.ToString());
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

    public void SelectReplacingChr(Chr _chrReplacingWith) {

        if(_chrReplacingWith.position.positiontype != Position.POSITIONTYPE.BENCH) {
            Debug.LogErrorFormat("Can't select {0} to replace an empty position, since they aren't on the bench", _chrReplacingWith);
            return;
        }

        chrReplacingWith = _chrReplacingWith;

        Debug.LogFormat("Selecting {0} to replace the empty position, {1}", chrReplacingWith, posEmpty);
    }


    public class ClauseReplaceEmptyPos : Clause {

        public Position posEmpty;
        public Chr chrReplacingWith;

        public ClauseReplaceEmptyPos(Position _posEmpty, Chr _chrReplacingWith) {
            posEmpty = _posEmpty;
            chrReplacingWith = _chrReplacingWith;
        }

        public override string GetDescription() {
            return string.Format("{0} replacing empty {1}", chrReplacingWith, posEmpty);
        }

        public override void Execute() {

            ContSkillEngine.PushSingleExecutable(new ExecMoveChar(null, chrReplacingWith, posEmpty));

        }
    }

    public override IEnumerator Execute() {

        //We'll push a clause to swap in our replacing character to the vacant spot we've been asked to fill
        ClauseReplaceEmptyPos clauseReplaceEmptyPos = new ClauseReplaceEmptyPos(posEmpty, chrReplacingWith);

        //Do a small delay for animations - note this uses ContTime's WaitForSeconds so that we adhere to any time-scale modifications like pausing
        yield return ContTime.Get().WaitForSeconds(ContTime.fDelayTurnSkill);

    }

    public override bool CanLegallyExecute() {
        if(posEmpty == null) return false;
        if(posEmpty.positiontype == Position.POSITIONTYPE.BENCH) return false;
        if(posEmpty.chrOnPosition != null) return false;
        if(chrReplacingWith == null) return false;

        if(chrReplacingWith.position.positiontype != Position.POSITIONTYPE.BENCH) {
            Debug.Log("Tried to select " + chrReplacingWith + " to swap in, but this character isn't on the bench");
            return false;
        }

        return true;
    }

    protected override void AttemptFillRandomly() {

        List<Chr> lstBenchedChrs = ChrCollection.Get().GetBenchChrsOwnedBy(Match.Get().arPlayers[posEmpty.PlyrIdOwnedBy()]);

        int nNumBenchedChars = lstBenchedChrs.Count;
        int nCurSelectionAttempt = 0;

        //Randomly select a character to swap in
        int iChrToSwapIn = ContRandomization.Get().GetRandom(0, nNumBenchedChars);

        //We'll cycle through the benched characters until we find one that works
        while(nCurSelectionAttempt < nNumBenchedChars) {
            nCurSelectionAttempt++;

            //Attempt to set the corresponding character as the one to swap in
            SelectReplacingChr(lstBenchedChrs[iChrToSwapIn % nNumBenchedChars]);

            //Check if that character would be a legal swap-in
            if(IsValidSelection()) {
                Debug.LogFormat("Randomly choosing {0} to swap in", chrReplacingWith);
                return;
            }
        }

        //If we tried many times and couldn't get a valid selection, then we'll just reset the default input
        ResetToDefaultInput();
    }

    //Sets the target of this input to be any random benched character (EVEN if they would norally not be allowed to switch in)
    public void SelectRandomBenchedChr() {

        List<Chr> lstBenchedChrs = ChrCollection.Get().GetBenchChrsOwnedBy(Match.Get().arPlayers[posEmpty.PlyrIdOwnedBy()]);

        int iRandomChr = ContRandomization.Get().GetRandom(0, lstBenchedChrs.Count);

        SelectReplacingChr(lstBenchedChrs[iRandomChr]);
    }

    public override void ResetToDefaultInput() {
        //In the event that we were somehow unable to validly select a character to swap in, then we'll
        //  just forcefully select a random one on the bench (even if they're normally somehow blocked from swapping in)
        SelectRandomBenchedChr();
    }

    // Clear out all non-essential data that could have been partially filled out from an incomplete selections process
    public override void ResetPartialSelection() {
        //Keep the empty pos the same

        //Clear out any selected chrReplacing since that's really what's being selected with this input
        chrReplacingWith = null;
    }

    //Set up any UI for prompting the selection of a chr to choose to replace some empty position
    public override void StartManualInputProcess(LocalInputHuman localinputhuman) {

        Debug.Log("Starting manual input for replaceemptypos");
        //TODONOW
    }


    //Clean up any UI for prompting the selection of a skill and re-lock the ability for the local player to go through the
    //   target selection process
    public override void EndManualInputProcess(LocalInputHuman localinputhuman) {

        Debug.Log("Ending manual input for replaceemptypos");
        //TODONOW
    }
}
