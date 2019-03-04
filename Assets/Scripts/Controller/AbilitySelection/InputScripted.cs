using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputScripted : InputAbilitySelection {

    public int[] arIndexTargetting;                         //Holds the current index of the script we're using for each character's next action
    public KeyValuePair<int, int[]>[,] arTargettingScript;

    public override void StartSelection() {

        //Give a small delay before we return the ability selection
        // so that we can give a chance to clear the stack out
        Invoke("SubmitNextAbility", 0.5f);

    }

    public void SetTargettingScript(KeyValuePair<int, int[]>[,] _arTargettingScript) {

        arTargettingScript = _arTargettingScript;

        arIndexTargetting = new int[arTargettingScript.Length];

    }

    public override void GaveInvalidTarget() {
        Debug.LogError("ERROR! - The scripted player input gave an invalid targetting selection - trying next available");

        SubmitNextAbility();
    }

    public void SubmitNextAbility() {

        int nActingChrid = ContTurns.Get().GetNextActingChr().id;

        //Double check that the index we're on for this character is before the end of that character's script
        if(arIndexTargetting[nActingChrid] >= arTargettingScript.GetLength(1)) {
            Debug.LogError("ERROR - not enough targetting information stored in this script for this character");
        }

        //Get the current targetting information, then increase the index for next time
        KeyValuePair<int, int[]> nextSelection = arTargettingScript[nActingChrid, arIndexTargetting[nActingChrid]];
        arIndexTargetting[nActingChrid]++;

        Debug.Log(ContTurns.Get().GetNextActingChr().sName + " has automatically chosen to use " +
            ContTurns.Get().GetNextActingChr().arActions[nextSelection.Key].sName + " with target index " + nextSelection.Value[0]);

        //We now need to ready enough effort mana to pay for the ability
        AutoPayCost(ContTurns.Get().GetNextActingChr().arActions[nextSelection.Key].parCost.Get());


        ContAbilitySelection.Get().SubmitAbility(nextSelection.Key, nextSelection.Value);


    }

    //Figures out and allocates non-effort mana to convert to cover the mana costs
    public void AutoPayCost(int[] arCost) {

        int nEffortToPay = arCost[(int)Mana.MANATYPE.EFFORT] - plyrOwner.mana.nManaPool;
        int nCurMana = 0;

        //Initially, try to pay with mana that isn't in the cost we need to pay for
        while(nCurMana < (int)Mana.MANATYPE.EFFORT) {

            //Check if we've allocated enough effort
            if (nEffortToPay <= 0) return;
                
            if (arCost[nCurMana] > 0) {
                //Then this type of mana is in the cost, so skip to the next type
                nCurMana++;
            } else {

                if (plyrOwner.mana.HasMana(nCurMana)) {
                    //If we can pay this type, then pay it
                    plyrOwner.mana.AddToPool(nCurMana);
                    Debug.Log("Automatically allocated a " + (Mana.MANATYPE)nCurMana + " to pay for effort");
                    nEffortToPay--;
                    //and don't change the nCurMana, so we can keep paying this type
                } else {
                    nCurMana++;
                }
            }

        }


        //If needed, we'll allocate mana types that we are paying, but that we have excess of
        while (nCurMana < (int)Mana.MANATYPE.EFFORT) {

            //Check if we've allocated enough effort
            if (nEffortToPay <= 0) return;

            //Check if we would have at least 1 mana left over after paying for the ability
            if (plyrOwner.mana.HasMana(nCurMana, 1 + arCost[nCurMana])) {
                //If we can pay this type, then pay it
                plyrOwner.mana.AddToPool(nCurMana);
                Debug.Log("Automatically allocated a " + (Mana.MANATYPE)nCurMana + " to pay for effort");
                nEffortToPay--;
                //and don't change the nCurMana, so we can keep paying this type
            } else {
                nCurMana++;
            }

        }

        //If we reach here, then we don't have enough mana (though we've allocated some effort - not ideal)
        Debug.Log("Not enough mana in the pool to allocate");
    }


    public static void SetRandomAbilities(InputScripted input) {

        int nAbilities = 100;
        KeyValuePair<int, int[]>[,] arListRandomSelections = new KeyValuePair<int, int[]>[Player.MAXCHRS, nAbilities];

        for (int i = 0; i < Player.MAXCHRS; i++) {
            for(int j=0; j< nAbilities; j++) {
                int nAbility = Random.Range(0, Chr.NUSABLEACTIONS);
                int [] arnTargetIndex = new int[] { Random.Range(0, Player.MAXCHRS * Player.MAXPLAYERS) };
                arListRandomSelections[i, j] = new KeyValuePair<int, int[]>(nAbility, arnTargetIndex);
            }
        }

        input.SetTargettingScript(arListRandomSelections);



    }
}
