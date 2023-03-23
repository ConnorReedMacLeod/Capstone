using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InputReplaceEmptyPos : MatchInput {
    public Position posEmpty;
    public Chr chrReplacingWith;

    //For creating a new skill selection collection to be filled out in the selection process
    public InputReplaceEmptyPos(Player plyrActing, Position _posEmpty) : base(plyrActing) {
        posEmpty = _posEmpty;

        Debug.Assert(posEmpty.positiontype != Position.POSITIONTYPE.BENCH);
    }

    //For deserializing a network-provided serialized empty-character-slot replacement selection.
    // The serialization array's elements are as follows:
    // 0: The MatchInputType (as an enum)
    // 1: The player set to act
    // 2: The empty position to be filled
    // 3: The character chosen to fill that position
    public InputReplaceEmptyPos(int[] arnSerializedSelections) : base(arnSerializedSelections) {

        Debug.Assert(arnSerializedSelections.Length == 4,
            "Received " + arnSerializedSelections.Length + " selections when we need to receive an MatchInputType, a Player, a Coords, and a Chr");

        //Verify we are decoding an input that matches our MatchInputType
        Debug.Assert((int)GetMatchInputType() == arnSerializedSelections[0]);

        plyrActing = Serializer.DeserializePlayer((byte)arnSerializedSelections[1]);
        posEmpty = Serializer.DeserializePosition(arnSerializedSelections[2]);
        chrReplacingWith = Serializer.DeserializeChr(arnSerializedSelections[3]);

    }

    public InputReplaceEmptyPos(InputReplaceEmptyPos other) : base(other) {
        posEmpty = other.posEmpty;
        chrReplacingWith = other.chrReplacingWith;
    }

    public InputReplaceEmptyPos GetCopy() {
        return new InputReplaceEmptyPos(this);
    }

    public override MatchInputType GetMatchInputType() {
        return MatchInputType.ReplaceOpenPos;
    }

    public override int[] Serialize() {

        int[] arnSerializedSelections = new int[4];

        //We'll just do some quick checks to make sure we have the correct data that we need to serialize
        Debug.Assert(posEmpty.chrOnPosition == null);
        Debug.Assert(chrReplacingWith != null);

        //For all serialized inputs, we will start our serialization array off with an int/enum representing the type of input we're recording
        arnSerializedSelections[0] = (int)GetMatchInputType();

        //Now we can start serializing the actual data for this input

        //First, add the player who is set to act
        arnSerializedSelections[1] = Serializer.SerializeByte(plyrActing);

        //Second, add the empty position that needs to be filled
        arnSerializedSelections[2] = Serializer.SerializeByte(posEmpty);

        //Third, add the character being swapped in
        arnSerializedSelections[3] = Serializer.SerializeByte(chrReplacingWith);

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

        Debug.LogFormat("Executing a InputReplaceEmptyPos for {0}", chrReplacingWith);

        //We'll push a clause to swap in our replacing character to the vacant spot we've been asked to fill
        ClauseReplaceEmptyPos clauseReplaceEmptyPos = new ClauseReplaceEmptyPos(posEmpty, chrReplacingWith);

        ContSkillEngine.PushSingleClause(clauseReplaceEmptyPos);

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


    public void cbOnClickSwitchableChr(Object tar, params object[] args) {

        Chr chrClicked = ((ViewChr)tar).mod;

        if(chrClicked.pbCanSwapIn.Get() == false) {
            Debug.LogErrorFormat("Tried to select character {0} to swap in that isn't allowed to", chrClicked);

            return;
        }

        SelectReplacingChr(chrClicked);

        //Since we've found a legal character to swap in, we can submit this input to the network sender
        NetworkMatchSender.Get().SendNextInput(this);
    }

    //Set up any UI for prompting the selection of a chr to choose to replace some empty position
    public override void StartManualInputProcess(LocalInputHuman localinputhuman) {

        Debug.LogError("TODO - make swappable-in characters not selectable to see their skills");
        Debug.Log("Starting manual input for replaceemptypos");

        List<Chr> lstChrsCanSwapIn = ContPositions.Get().GetAlliedBenchChrs(plyrActing).Where(chr => chr.pbCanSwapIn.Get()).ToList();

        bool bCanSwapBenchPlyr0 = false;
        bool bCanSwapBenchPlyr1 = false;

        //Highlight each of these chrs
        foreach(Chr chr in lstChrsCanSwapIn) {
            chr.view.DecideIfHighlighted(ViewChr.SelectabilityState.ALLYSELECTABLE);
            chr.view.subMouseClick.Subscribe(cbOnClickSwitchableChr);


            if (chr.plyrOwner.id == 0) {
                bCanSwapBenchPlyr0 = true;
            } else if (chr.plyrOwner.id == 1) {
                bCanSwapBenchPlyr1 = true;
            }
            
        }

        string sCameraLocation = "Home";

        if(bCanSwapBenchPlyr0 && bCanSwapBenchPlyr1) {
            sCameraLocation = "ZoomedOut";
        }else if (bCanSwapBenchPlyr0) {
            sCameraLocation = "BenchLeft";
        }else if (bCanSwapBenchPlyr1) {
            sCameraLocation = "BenchRight";
        }

        Match.Get().cameraControllerMatch.SetTargetLocation(sCameraLocation);

    }

    //Clean up any UI for prompting the selection of a bench character to replace an empty pos
    public override void EndManualInputProcess(LocalInputHuman localinputhuman) {

        Debug.Log("Ending manual input for replaceemptypos");

        List<Chr> lstChrsCanSwapIn = ContPositions.Get().GetAlliedBenchChrs(plyrActing).Where(chr => chr.pbCanSwapIn.Get()).ToList();

        //Unhighlight each of these chrs
        foreach(Chr chr in lstChrsCanSwapIn) {
            chr.view.DecideIfHighlighted(ViewChr.SelectabilityState.NONE);
            chr.view.subMouseClick.UnSubscribe(cbOnClickSwitchableChr);
        }
    }
}
