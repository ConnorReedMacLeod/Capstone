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

    //Store the ability selection set by the local client to be sent to the master when finalized,
    // and stores the broadcasted selection info that has been sent out by the master once approved to be executed
    public SelectionSerializer.SelectionInfo infoSelection;

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

    //TODONOW - Consider putting this at the beginning of the ExecuteAction Phase,
    //          since then we know that we've completely finished any possible selection
    //          (either successfully or unsuccessfully)
    public void EndSelection() {

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


    //Check what input has been stored in the provided InputAbilitySelection for the next acting character
    public void SubmitAbility(SelectionSerializer.SelectionInfo infoSelection) {

        Chr chrActing = ContTurns.Get().GetNextActingChr();

        if(input.plyrOwner.id != chrActing.plyrOwner.id) {
            Debug.LogError("Error! Recieved ability selection for player " + input.plyrOwner.id + " even though its character " +
                chrActing.sName + "'s turn to select an ability");
        } else if(input.nSelectedChrId != chrActing.globalid) {
            Debug.LogError("Error! Recieved ability selection for character " + input.nSelectedChrId + " even though its character " +
                chrActing.sName + "'s turn to select an ability");
        }


        // confirm that the target is valid
        //(checks actionpoint usage, cd, mana, targetting)
        if(chrActing.arActions[input.nSelectedAbility].CanActivate(input.infoSelection) == false ||
            chrActing.arActions[input.nSelectedAbility].CanPayMana() == false) {

            //This is somewhat of a bandaid to fix some infinite looping in the AI ability selection code
            nBadSelectionsGiven++;
            if(nBadSelectionsGiven >= 5) {

                Debug.LogError("Too many bad selections given - assigning a rest action");

                //Reset the infoSelection to just be the current character choosing a rest action
                infoSelection = SelectionSerializer.MakeRestSelection(ContTurns.Get().GetNextActingChr());

                EndSelection();

                ContAbilityEngine.Get().ProcessStacks();
                return;
            }

            //If the targetting isn't valid, get the input for the current character, and let them know they
            // submitted a bad ability
            ContTurns.Get().GetNextActingChr().plyrOwner.inputController.GaveInvalidTarget();

            //Just return and wait for a better selection
            return;
        }

        //If we get this far, then the selecting is valid

        //Save the validly selected abilities
        infoSelection = input.infoSelection;

        EndSelection();

        //Subtmit the ability selection to the master
        ClientNetworkController.Get().SendTurnPhaseFinished(input.infoSelection);

        //UPDATE - We used to just process the stacks ourselves, but now we'll wait to the master to tell us
        // when it's safe to start processing the stacks since all players are ready
        //If we've successfully selected an action, call the ProcessStack function (on an empty stack)
        //which will put an execExecuteAction on the stack
        //ContAbilityEngine.Get().ProcessStacks();

    }

    public void OnTimedOut() {
        Debug.Log("No Ability Selected in time");

        //Reset the infoSelection to just be the current character choosing a rest action
        infoSelection = SelectionSerializer.MakeRestSelection(ContTurns.Get().GetNextActingChr());
        bSelectingAbility = false;
    }

    public void OnSubmittedAbility() {

    }


}
