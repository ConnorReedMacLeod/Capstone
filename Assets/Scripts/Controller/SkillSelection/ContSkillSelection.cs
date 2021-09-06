using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This should just be in charge of submitting and recieving signals about skill selection
//  (this doesn't care about the details of the selection itself - let the skill engine and local selection controller worry
//    about this details.  This will submit selections to the master client, and respond to feedback signals from that master
public class ContSkillSelection : Singleton<ContSkillSelection> {


    public float fMaxSelectionTime;

    public enum DELAYOPTIONS {
        FAST, MEDIUM, INF
    };

    public const float fDelayChooseSkillFast = 5.0f;
    public const float fDelayChooseSkillMedium = 30.0f;
    public const float fDelayChooseSkillInf = 9999999.0f;

    //As a fix for an infinite loop in the AI controller (which really should be fixed at some point), keep track of how many bad inputs we've been given
    public int nBadSelectionsGiven;

    public void SetMaxSelectionTime(DELAYOPTIONS delay) {
        switch(delay) {
        case DELAYOPTIONS.FAST:
            fMaxSelectionTime = fDelayChooseSkillFast;
            break;

        case DELAYOPTIONS.MEDIUM:
            fMaxSelectionTime = fDelayChooseSkillMedium;
            break;

        case DELAYOPTIONS.INF:
            fMaxSelectionTime = fDelayChooseSkillInf;
            break;
        }
    }


    //Stores the broadcasted selection information for what skill should be used by the next acting character
    // (regardless of it's our local player's turn to move or not) - This should only be read from (the master
    //  network will be the one writing to this field)
    public Selections selectionsFromMaster;

    public override void Init() {

    }

    //Check what input has been stored in the provided SelectionInfo for the next acting character
    public void SubmitSkill(Selections selections, LocalInputType inputSentFrom) {

        Chr chrActing = ContTurns.Get().GetNextActingChr();

        if(selections.skillSelected.chrOwner.plyrOwner.id != chrActing.plyrOwner.id) {
            Debug.LogError("Error! Recieved skill selection for player " + selections.skillSelected.chrOwner.plyrOwner.id + " even though it's character " +
                chrActing.sName + "'s turn to select a skill");
        } else if(selections.skillSelected.chrOwner.globalid != chrActing.globalid) {
            Debug.LogError("Error! Recieved skill selection for character " + selections.skillSelected.chrOwner.globalid + " even though it's character " +
                chrActing.sName + "'s turn to select a skill");
        }


        // confirm that the target is valid
        if(selections.IsValidSelection() == false) {

            //If the selection was invalid for some reason, either send it back to the selector to choose again,
            // or just reset them to a rest skill

            //This is somewhat of a bandaid to fix some infinite looping in the AI skill selection code
            nBadSelectionsGiven++;
            if(nBadSelectionsGiven >= 5) {

                Debug.LogError("Too many bad selections given - assigning a rest skill");

                //Override the submitted selection to just be the current character choosing a rest skill
                selections.ResetToRestSelection();

            } else {

                //If the targetting isn't valid, get the input for the current character, and let them know they
                // submitted a bad skill selection
                ContTurns.Get().GetNextActingChr().plyrOwner.inputController.GaveInvalidTarget();

                //Just return and wait for a better selection
                return;
            }
        }

        //If we get this far, then the selecting is valid

        //Let the LocalInput know that its submission was valid (as far as we can tell) and that the
        // selection process was completed
        inputSentFrom.CompletedSelection();

        Debug.Log("Client is about to send " + selections.ToString());

        //Submit the skill selection to the master
        ClientNetworkController.Get().SendTurnPhaseFinished(selections.GetSerialization());

    }

    public void ReceiveSelectionFromMaster(int[] arnSerializedSelections) {
        Debug.Assert(ContTurns.Get().curStateTurn == ContTurns.STATETURN.EXECUTESKILL);

        //Save the result that the master broadcasted out
        selectionsFromMaster = new Selections(arnSerializedSelections);

        Debug.Log("Client received selection of " + selectionsFromMaster.ToString());

        //Ensure the passed skill is valid
        if(selectionsFromMaster.IsValidSelection() == false) {
            Debug.LogError("Received invalid selection from master! : " + selectionsFromMaster);
            selectionsFromMaster.ResetToRestSelection();
        }

        //Stop the selection process (if it's still ongoing) since the decision has already been finalized by the master
        Match.Get().GetLocalPlayer().inputController.EndSelection();

    }

    public void ResetStoredSelection() {
        Debug.Log("Resetting stored selection from master");
        selectionsFromMaster = null;
    }

    public void StartSelection() {

        Chr chrCurActing = ContTurns.Get().GetNextActingChr();

        if(chrCurActing == null) {
            Debug.Log("No character is set to act this turn - just move to the next phase");

            ClientNetworkController.Get().SendTurnPhaseFinished();
            return;
        }

        //At this point, we can start the selection process, and notify the controller
        // of the owner of the currently acting character

        if(chrCurActing.plyrOwner.inputController == null) {
            //This character is controlled by someone else on the network, so we're good to just send a turn-phase ending signal since it's
            //   a different player's job to submit the skill and targetting selection information for their character
            Debug.Log("This character isn't owned locally - passing priority");

            ClientNetworkController.Get().SendTurnPhaseFinished();
            return;
        }

        //Get the controller of the character and let them know they can start selecting their skill and their targets for that skill
        chrCurActing.plyrOwner.inputController.StartSelection();

    }


    public void EndSelection() {

        Chr chrCurActing = ContTurns.Get().GetNextActingChr();

        if(chrCurActing != null) {

            LocalInputHuman.subAllHumanEndSelection.NotifyObs(chrCurActing);
        }

    }

}
