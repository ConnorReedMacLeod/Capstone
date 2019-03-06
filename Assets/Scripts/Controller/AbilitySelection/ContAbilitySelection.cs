using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContAbilitySelection : MonoBehaviour {

    public bool bSelectingAbility;
    public float fMaxSelectionTime;
    public float fSelectionTimer;

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

    public void Start() {

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
        bSelectingAbility = false;
        fSelectionTimer = 0.0f;
    }

    public void SubmitAbility(int indexAbility, int[] indexTargetting) {

        //Actually fill in the character's selected ability
        ContTurns.Get().GetNextActingChr().nUsingAction = indexAbility;

        // fill in the targetting info for the indexAbility'th ability
        // with the indexTargetting targets
        ContTurns.Get().GetNextActingChr().arActions[indexAbility].SetTargettingArgs(indexTargetting);

        // then confirm that the target is valid
        //(checks actionpoint usage, mana, cd, targetting)
        if (!ContTurns.Get().GetNextActingChr().arActions[indexAbility].CanActivate()) {

            //If the targetting isn't valid, get the input for the current character, and let them know they
            // submitted a bad ability
            ContTurns.Get().GetNextActingChr().plyrOwner.inputController.GaveInvalidTarget();

            ContTurns.Get().GetNextActingChr().ResetSelectedAction();

            //Just return and wait for a better selection
            return;
        }

        ContTurns.Get().GetNextActingChr().bSetAction = true;
        EndSelection();

        //If we've successfully selected an action, call the ProcessStack function (on an empty stack)
        //which will put an execExecuteAction on the stack
        ContAbilityEngine.Get().ProcessStacks();

    }

    public void Update() {

        if (bSelectingAbility) {
            fSelectionTimer += Time.deltaTime;

            if(fSelectionTimer >= fMaxSelectionTime) {
                //Then the time has expired 
                Debug.Log("TIME IS UP! NO ABILITY SELECTED! MOVING ON");

                //Consider if we need to have some cancel-targetting call here

                Debug.Assert(ContTurns.Get().GetNextActingChr().bSetAction == false);

                EndSelection();

                ContAbilityEngine.Get().ProcessStacks();

            }
        }

    }


}
