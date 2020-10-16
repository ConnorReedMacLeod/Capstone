using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This should just be in charge of maintaining the state of the local ability selection process
// It can then submit its selection to the master client, and respond to feedback signals from that master
public class ContAbilitySelection : Singleton<ContAbilitySelection> {

    public bool bSelectingAbility;
    public float fMaxSelectionTime;

    public enum DELAYOPTIONS {
        FAST, MEDIUM, INF
    };

    public const float fDelayChooseActionFast = 5.0f;
    public const float fDelayChooseActionMedium = 30.0f;
    public const float fDelayChooseActionInf = 9999999.0f;

    public float fSelectionTimer;

    //Stores the broadcasted selection information for what action should be used by the next acting character
    // (regardless of it's our local player's turn to move or not) - This should only be read from (the master
    //  network will be the one writing to this field)
    public SelectionSerializer.SelectionInfo infoSelectionFromMaster;

    //As a fix for an infinite loop in the AI controller (which really should be fixed at some point), keep track of how many bad inputs we've been given
    public int nBadSelectionsGiven;


    public void SetMaxSelectionTime(DELAYOPTIONS delay) {
        switch(delay) {
        case DELAYOPTIONS.FAST:
            fMaxSelectionTime = fDelayChooseActionFast;
            break;

        case DELAYOPTIONS.MEDIUM:
            fMaxSelectionTime = fDelayChooseActionMedium;
            break;

        case DELAYOPTIONS.INF:
            fMaxSelectionTime = fDelayChooseActionInf;
            break;
        }
    }

    public override void Init() {

        //SetMaxSelectionTime(DELAYOPTIONS.MEDIUM);

        EndSelection();

    }

    public void StartSelection() {

        Chr chrCurActing = ContTurns.Get().GetNextActingChr();

        if(chrCurActing == null) {
            Debug.LogError("Error! Can't select actions if no character is set to act");
        }

        //Ensure only the currently acting character can select actions
        chrCurActing.UnlockTargetting();

        //At this point, we can start the selection process, and notify the controller
        // of the owner of the currently acting character
        bSelectingAbility = true;
        nBadSelectionsGiven = 0;
        chrCurActing.plyrOwner.inputController.StartSelection();

    }

    public void EndSelection() {
        //TODONOW - consider what even needs to be done here.  It's not clear that this will
        //  only ever be interacted with by a human.  I also don't see what LockTargetting() should
        //  really do since this should be a human-interaction based action.  Locking only makes sense for the surface
        //  interactions with the game and not any game-altering effects.

        if(bSelectingAbility == true) {
            //If this flag is still raised, then we were still in the middle of trying to submit an ability
            // when we got the message 
        }

        Chr chrCurActing = ContTurns.Get().GetNextActingChr();

        if(chrCurActing != null) {
            chrCurActing.LockTargetting();

            //Clear out the selection information stored in the provided input
            chrCurActing.plyrOwner.inputController.ResetTargets();

            InputHuman.subAllHumanEndSelection.NotifyObs(chrCurActing);
        }



        bSelectingAbility = false;
        fSelectionTimer = 0.0f;
    }


    //Check what input has been stored in the provided SelectionInfo for the next acting character
    public void SubmitAbility(SelectionSerializer.SelectionInfo infoSelectionSubmitted) {

        Chr chrActing = ContTurns.Get().GetNextActingChr();

        if(infoSelectionSubmitted.chrOwner.plyrOwner.id != chrActing.plyrOwner.id) {
            Debug.LogError("Error! Recieved ability selection for player " + infoSelectionSubmitted.chrOwner.plyrOwner.id + " even though it's character " +
                chrActing.sName + "'s turn to select an ability");
        } else if(infoSelectionSubmitted.chrOwner.globalid != chrActing.globalid) {
            Debug.LogError("Error! Recieved ability selection for character " + infoSelectionSubmitted.chrOwner.globalid + " even though it's character " +
                chrActing.sName + "'s turn to select an ability");
        }


        // confirm that the target is valid
        //(checks actionpoint usage, cd, mana, targetting)
        if(infoSelectionSubmitted.CanActivate() == false || infoSelectionSubmitted.actUsed.CanPayMana() == false) {

            //If the selection was invalid for some reason, either send it back to the selector to choose again,
            // or just reset them to a rest action

            //This is somewhat of a bandaid to fix some infinite looping in the AI ability selection code
            nBadSelectionsGiven++;
            if(nBadSelectionsGiven >= 5) {

                Debug.LogError("Too many bad selections given - assigning a rest action");

                //Override the submitted selection to just be the current character choosing a rest action
                infoSelectionSubmitted = SelectionSerializer.MakeRestSelection(ContTurns.Get().GetNextActingChr());

            } else {

                //If the targetting isn't valid, get the input for the current character, and let them know they
                // submitted a bad ability
                ContTurns.Get().GetNextActingChr().plyrOwner.inputController.GaveInvalidTarget();

                //Just return and wait for a better selection
                return;
            }
        }

        //If we get this far, then the selecting is valid

        //Turn off the flag that we're still in the process of selecting an ability
        bSelectingAbility = false;

        //Submit the ability selection to the master
        ClientNetworkController.Get().SendTurnPhaseFinished(infoSelectionSubmitted.Serialize());

    }

    public void ReceiveSelectionFromMaster(int nSerializedSelection) {
        Debug.Assert(ContTurns.Get().curStateTurn == ContTurns.STATETURN.EXECUTEACTIONS);

        Chr chrActing = ContTurns.Get().GetNextActingChr();

        //Save the result that the master broadcasted out
        infoSelectionFromMaster = SelectionSerializer.Deserialize(chrActing, nSerializedSelection);

        //Ensure the passed action is valid
        Debug.Assert(infoSelectionFromMaster.CanActivate());

        //Stop the selection process (if it's still ongoing) since the decision has already been finalized by the master
        EndSelection();

        //Since the master has sent out the selection information to everyone, we're good to start processing the effect
        ContAbilityEngine.Get().ProcessStacks();

    }


}
