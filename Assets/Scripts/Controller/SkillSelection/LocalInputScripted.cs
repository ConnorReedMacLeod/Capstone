using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalInputScripted : LocalInputType {


    public int[] arScriptedTargettingIndices;                         //Holds the current index of the script we're using for each character's next skill
    public KeyValuePair<int, Selections>[,] arTargettingScript;
    public const int MAXTARGETATTEMPTS = 5;

    public override void StartSelection() {
        base.StartSelection();

        //Give a small delay before we return the skill selection
        // so that we can give a chance to clear the stack out
        ContTime.Get().Invoke(Mathf.Min(ContSkillSelection.Get().fMaxSelectionTime / 2, 1.5f), SubmitNextSkill);

    }

    public void SetTargettingScript(KeyValuePair<int, Selections>[,] _arTargettingScript) {

        arTargettingScript = _arTargettingScript;

        arScriptedTargettingIndices = new int[arTargettingScript.Length];

    }

    public void SubmitNextSkill() {

        //Save the character who we'll be selecting skills for
        Chr chrToAct = ContTurns.Get().chrNextReady;

        Debug.Assert(chrToAct != null, "Scripted input was asked to submit an skill for a character, but no character is acting");
        Debug.Assert(chrToAct.plyrOwner.id == plyrOwner.id, "Scripted input was asked to submit an skill for a character is doesn't own");

        KeyValuePair<int, Selections> nextSelection;
        int nTargetsTried = 0;

        Selections selections;

        //Keep looking until we find a valid skill selection
        while(true) {

            //Double check that the index we're on for this character is before the end of that character's script
            if(arScriptedTargettingIndices[chrToAct.id] >= arTargettingScript.GetLength(1)) {
                Debug.LogError("ERROR - not enough targetting information stored in this script for this character - resetting");
                arScriptedTargettingIndices[chrToAct.id] = 0;
            }

            //Get the current targetting information, then increase the index for next time
            nextSelection = arTargettingScript[chrToAct.id, arScriptedTargettingIndices[chrToAct.id]];
            arScriptedTargettingIndices[chrToAct.id]++;
            nTargetsTried++;

            selections = nextSelection.Value;

            Debug.Log(chrToAct.sName + " wants chosen to use " + selections.ToString());

            //Test to see if this skill would be valid
            if(selections.IsValidSelection() == false) {
                Debug.Log("The skill selection would not be legal");

                if(nTargetsTried >= MAXTARGETATTEMPTS) {
                    //If we've tried too many skills with no success, just end our selections
                    // by setting our skill as a rest

                    selections.ResetToRestSelection();

                    break;

                } else {
                    //Otherwise, just try selecting the next skill
                    continue;
                }
            } else {
                //If the selection is valid
                Debug.Log("Automatic selection is valid");

                //Our selection information has already been saved


                break;
            }

        }

        //At this point, we will have selected a skill/targetting and saved the information in infoSelection
        Debug.Log(chrToAct.sName + " has automatically chosen to use " + selections.ToString());

        ContSkillSelection.Get().SubmitSkill(selections, this);


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
