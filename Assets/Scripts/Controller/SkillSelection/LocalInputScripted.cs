using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalInputScripted : LocalInputType {


    public int nInputIndex;                         //The current index we've reached in our list of inputs we'll provide
    public List<MatchInput> lstInputScript;              //The list of inputs that we'll give in order

    public InputType inputtypeToSwitchTo;

    public override InputType GetInputType() {
        return InputType.SCRIPTED;
    }

    public override void StartSelection() {
        base.StartSelection();

        //Give a small delay before we return the skill selection
        // so that we can give a chance to clear the stack out
        ContTime.Get().Invoke(Mathf.Min(ContTime.Get().fMaxSelectionTime / 2, 1.5f), SubmitNextSkill);

    }

    public void SetTargettingScript(List<MatchInput> _lstInputScript) {

        lstInputScript = _lstInputScript;

        //Start reading inputs from the beginning of the script
        nInputIndex = 0;

    }

    public void SubmitNextSkill() {

        Debug.Assert(ContSkillEngine.Get().matchinputToFillOut != null, "Scripted input was asked to submit an input, but we're not locally waiting on any input");
        Debug.AssertFormat(ContSkillEngine.Get().matchinputToFillOut.iPlayerActing == plyrOwner.id, 
            "Scripted input was asked to submit an input for player {0}, but this script is for player {1}",
            ContSkillEngine.Get().matchinputToFillOut.iPlayerActing, plyrOwner.id);


        //If we already used all of the inputs in our script, then we should change our inputtype to something more flexible
        if (nInputIndex >= lstInputScript.Count) {
            Debug.LogFormat("Finished scripted input for Player {0} - switching to type {1}", plyrOwner, inputtypeToSwitchTo);

            plyrOwner.SetInputType(inputtypeToSwitchTo);

            plyrOwner.inputController.StartSelection();

            return;
        }

        //If we still have inputs in our script, take the appropriate one
        ContSkillEngine.Get().matchinputToFillOut = lstInputScript[nInputIndex];

        //Double check that the input that we're planning to submit is actually valid
        if (ContSkillEngine.Get().matchinputToFillOut.CanLegallyExecute() == false) {
            Debug.LogErrorFormat("Warning - resetting input to default since an illegal input {0} was attempted", ContSkillEngine.Get().matchinputToFillOut);
            //If it wasn't legal, then reset it to some default failsafe that is guaranteed to be legal
            ContSkillEngine.Get().matchinputToFillOut.ResetToDefaultInput();
        }

        //Since we've grabbed this input, advance our index to be ready for the next requested input
        nInputIndex++;

        //By this point, we have a valid input, so let's submit it
        NetworkMatchSender.Get().SendNextInput(ContSkillEngine.Get().matchinputToFillOut);

    }


}
