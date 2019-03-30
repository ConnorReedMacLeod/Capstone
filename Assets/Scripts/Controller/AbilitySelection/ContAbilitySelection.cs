using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContAbilitySelection : MonoBehaviour {

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

    public static ContAbilitySelection instance;

    public static ContAbilitySelection Get() {
        if (instance == null) {
            GameObject go = GameObject.FindGameObjectWithTag("Controller");
            if (go == null) {
                Debug.LogError("ERROR! NO OBJECT HAS A Controller TAG!");
            }
            instance = go.GetComponent<ContAbilitySelection>();
            if (instance == null) {
                Debug.LogError("ERROR! Controller TAGGED OBJECT DOES NOT HAVE A ContTarget COMPONENT!");
            }
            instance.Start();
        }
        return instance;
    }

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

    public void Start() {

        //Set our delay time to sync up with the constant
        SetMaxSelectionTime(DELAYOPTIONS.MEDIUM);

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
        chrCurActing.plyrOwner.inputController.StartSelection();

    }

    public void EndSelection() {

        Chr chrCurActing = ContTurns.Get().GetNextActingChr();

        if (chrCurActing != null) {
            chrCurActing.LockTargetting();
        }

        bSelectingAbility = false;
        fSelectionTimer = 0.0f;
    }

    public void SubmitAbility(int indexAbility, int[] indexTargetting) {

        Chr chrActing = ContTurns.Get().GetNextActingChr();

        // confirm that the target is valid
        //(checks actionpoint usage, cd, mana, targetting)
        if (chrActing.arActions[indexAbility].CanActivate(indexTargetting) == false ||
            chrActing.arActions[indexAbility].CanPayMana() == false) {
        
            //If the targetting isn't valid, get the input for the current character, and let them know they
            // submitted a bad ability
            ContTurns.Get().GetNextActingChr().plyrOwner.inputController.GaveInvalidTarget();

            //Just return and wait for a better selection
            return;
        }

        //If we get this far, then the selecting is valid

        //Save the validly selected abilities
        nSelectedAbility = indexAbility;
        lstSelectedTargets = indexTargetting;

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
