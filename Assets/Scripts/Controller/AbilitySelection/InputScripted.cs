using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputScripted : InputAbilitySelection {

    public int[] arIndexTargetting;                         //Holds the current index of the script we're using for each character's next action
    public KeyValuePair<int, int[]>[][] arTargettingScript;

    public override void StartSelection() {

        //Give a small delay before we return the ability selection
        // so that we can give a chance to clear the stack out
        Invoke("SubmitNextAbility", 0.5f);

    }

    public void SetTargettingScript(KeyValuePair<int, int[]>[][] _arTargettingScript) {

        arTargettingScript = _arTargettingScript;

        arIndexTargetting = new int[arTargettingScript.Length];

    }

    public override void GaveInvalidTarget() {
        Debug.LogError("ERROR! - The scripted player input gave an invalid targetting selection - trying next available");

        SubmitNextAbility();
    }

    public void SubmitNextAbility() {

        int nActingid = ContTurns.Get().GetNextActingChr().id;

        //Double check that the index we're on for this character is before the end of that character's script
        if(arIndexTargetting[nActingid] >= arTargettingScript[nActingid].Length) {
            Debug.LogError("ERROR - not enough targetting information stored in this script for this character");
        }

        //Get the current targetting information, then increase the index for next time
        KeyValuePair<int, int[]> nextSelection = arTargettingScript[nActingid][arIndexTargetting[nActingid]];
        arIndexTargetting[nActingid]++;

        ContAbilitySelection.Get().SubmitAbility(nextSelection.Key, nextSelection.Value);


    }
}
