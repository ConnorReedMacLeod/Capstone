using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalInputAI : LocalInputType {


    public override InputType GetInputType() {
        return InputType.AI;
    }

    public override void StartSelection() {
        base.StartSelection();

        //Give a small delay before we return the skill selection
        // so that we can give a chance to clear the stack out
        ContTime.Get().Invoke(Mathf.Min(ContTime.Get().fMaxSelectionTime / 2, 1.5f), SubmitNextSkill);

    }

    public void SubmitNextSkill() {

        Debug.Assert(ContSkillEngine.Get().matchinputToFillOut != null, "AI input was asked to submit an input, but we're not locally waiting on any input");
        Debug.AssertFormat(ContSkillEngine.Get().matchinputToFillOut.iPlayerActing == plyrOwner.id,
            "Scripted input was asked to submit an input for player {0}, but this controller is for player {1}",
            ContSkillEngine.Get().matchinputToFillOut.iPlayerActing, plyrOwner.id);

        //Just randomly select an input that would work for the pending match input
        ContSkillEngine.Get().matchinputToFillOut.FillRandomly();


        //Double check that the input that we're planning to submit is actually valid
        if (ContSkillEngine.Get().matchinputToFillOut.CanLegallyExecute() == false) {
            Debug.LogErrorFormat("Warning - resetting input to default since an illegal input {0} was attempted", ContSkillEngine.Get().matchinputToFillOut);
            //If it wasn't legal, then reset it to some default failsafe that is guaranteed to be legal
            ContSkillEngine.Get().matchinputToFillOut.ResetToDefaultInput();
        }

        //By this point, we have a valid input, so let's submit it
        NetworkMatchSender.Get().SendNextInput(ContSkillEngine.Get().matchinputToFillOut);

    }
}
