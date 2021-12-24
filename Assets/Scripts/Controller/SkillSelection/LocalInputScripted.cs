using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalInputScripted : LocalInputType {


    public int nInputIndex;                         //The current index we've reached in our list of inputs we'll provide
    public List<MatchInput> lstInputScript;              //The list of inputs that we'll give in order

    public override void StartSelection() {
        base.StartSelection();

        //Give a small delay before we return the skill selection
        // so that we can give a chance to clear the stack out
        ContTime.Get().Invoke(Mathf.Min(ContSkillSelection.Get().fMaxSelectionTime / 2, 1.5f), SubmitNextSkill);

    }

    public void SetTargettingScript(List<MatchInput> _lstInputScript) {

        lstInputScript = _lstInputScript;

        //Start reading inputs from the beginning of the script
        nInputIndex = 0;

    }

    public void SubmitNextSkill() {
        MatchInput matchinputPending = ContSkillEngine.Get().matchinputToFillOut;

        Debug.Assert(ContSkillEngine.Get().matchinputToFillOut != null, "Scripted input was asked to submit an input, but we're not locally waiting on any input");
        Debug.AssertFormat(ContSkillEngine.Get().matchinputToFillOut.iPlayerActing == plyrOwner.id, 
            "Scripted input was asked to submit an input for player {0}, but this script is for player {1}",
            ContSkillEngine.Get().matchinputToFillOut.iPlayerActing, plyrOwner.id);

        //If we still have inputs in our script, then grab the next one
        if (nInputIndex < lstInputScript.Count) {

            ContSkillEngine.Get().matchinputToFillOut = lstInputScript[nInputIndex];
            
        } else {
            //If we already used all of the inputs in our script, then set our input to a random one
            ContSkillEngine.Get().matchinputToFillOut.FillRandomly();
        
        }

        //Double check that the input that we're planning to submit is actually valid
        if(ContSkillEngine.Get().matchinputToFillOut.CanLegallyExecute() == false) {
            Debug.LogErrorFormat("Warning - resetting input to default since an illegal input {0} was attempted", ContSkillEngine.Get().matchinputToFillOut);
            //If it wasn't legal, then reset it to some default failsafe that is guaranteed to be legal
            ContSkillEngine.Get().matchinputToFillOut.ResetToDefaultInput();
        }

        //Since we've grabbed this input, advance our index to be ready for the next requested input
        nInputIndex++;

        if (nInputIndex == lstInputScript.Count) {
            //If we've reached the end of our script, figure out how we should continue;
            OnFinishedScript();
        }
        
        //By this point, we have a valid input, so let's submit it


    }

    public virtual void OnFinishedScript() {
        //For when we've finished submitting all the inputs we have stored in our script - extend as needed
        //For now, we're just letting a scripted input continue to just provide random selections if its run out
    }

    public static void SetRandomSkills(LocalInputScripted input) {

        int nScriptLength = 100;
        KeyValuePair<int, Selections>[,] arListRandomSelections = new KeyValuePair<int, Selections>[Player.MAXCHRS, nScriptLength];

        for(int i = 0; i < Player.MAXCHRS; i++) {

            Chr chr = input.plyrOwner.arChr[i];

            for(int j = 0; j < nScriptLength; j++) {

                //Select a random skill to be used
                Skill skillRandom = chr.GetRandomSkill();

                //Generate random selections for the skill
                Selections selectionsRandom = new Selections(skillRandom);
                selectionsRandom.FillWithRandomSelections();

                arListRandomSelections[i, j] = new KeyValuePair<int, Selections>(skillRandom.skillslot.iSlot, selectionsRandom);
            }
        }

        input.SetTargettingScript(arListRandomSelections);

    }
}
