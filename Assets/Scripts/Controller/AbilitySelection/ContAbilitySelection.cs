using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public int nSelectedAbility;
    public int[] lstSelectedTargets;

    //As a fix for an infinite loop in the AI controller (which really should be fixed at some point), keep track of how many bad inputs we've been given
    public int nBadSelectionsGiven;


    public void SetMaxSelectionTime(DELAYOPTIONS delay) {
        switch (delay) {
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

        Chr chrCurActing = ContTurns.Get().GetNextActingChr();

        if (chrCurActing != null) {
            chrCurActing.LockTargetting();

            //Clear out the selection information stored in the provided input
            chrCurActing.plyrOwner.inputController.ResetTargets();

            InputHuman.subAllHumanEndSelection.NotifyObs(chrCurActing);
        }



        bSelectingAbility = false;
        fSelectionTimer = 0.0f;
    }


    //Check what input has been stored in the provided InputAbilitySelection for the next acting character
    public void SubmitAbility(InputAbilitySelection input) {

        Chr chrActing = ContTurns.Get().GetNextActingChr();

        if(input.plyrOwner.id != chrActing.plyrOwner.id) {
            Debug.LogError("Error! Recieved ability selection for player " + input.plyrOwner.id + " even though its character " +
                chrActing.sName + "'s turn to select an ability");
        }else if(input.nSelectedChrId != chrActing.globalid) {
            Debug.LogError("Error! Recieved ability selection for character " + input.nSelectedChrId + " even though its character " +
                chrActing.sName + "'s turn to select an ability");
        }


        // confirm that the target is valid
        //(checks actionpoint usage, cd, mana, targetting)
        if (chrActing.arActions[input.nSelectedAbility].CanActivate(input.arTargetIndices) == false ||
            chrActing.arActions[input.nSelectedAbility].CanPayMana() == false) {

            //This is somewhat of a bandaid to fix some infinite looping in the AI ability selection code
            nBadSelectionsGiven++;
            if(nBadSelectionsGiven >= 5) {

                Debug.LogError("Too many bad selections given - assigning a rest action");
                nSelectedAbility = Chr.idResting;
                lstSelectedTargets = input.arTargetIndices;

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
        nSelectedAbility = input.nSelectedAbility;
        lstSelectedTargets = input.arTargetIndices;

        EndSelection();

        //If we've successfully selected an action, call the ProcessStack function (on an empty stack)
        //which will put an execExecuteAction on the stack
        ContAbilityEngine.Get().ProcessStacks();

    }

    public void Update() {

        if (bSelectingAbility) {
            fSelectionTimer += ContTime.Get().fDeltaTime;

            if (fSelectionTimer >= fMaxSelectionTime) {
                //Then the time has expired 
                Debug.Log("TIME IS UP! NO ABILITY SELECTED! MOVING ON");

                //Consider if we need to have some cancel-targetting call here

                //Set the used action to be a resting action
                nSelectedAbility = Chr.idResting;

                EndSelection();

                ContAbilityEngine.Get().ProcessStacks();

            }
        }

    }


}
