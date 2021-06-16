using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalInputScripted : LocalInputType {


    public int[] arScriptedTargettingIndices;                         //Holds the current index of the script we're using for each character's next skill
    public KeyValuePair<int, int>[,] arTargettingScript;
    public const int MAXTARGETATTEMPTS = 5;

    public override void StartSelection() {
        base.StartSelection();

        //Give a small delay before we return the skill selection
        // so that we can give a chance to clear the stack out
        ContTime.Get().Invoke(Mathf.Min(ContSkillSelection.Get().fMaxSelectionTime / 2, 1.5f), SubmitNextSkill);

    }

    public void SetTargettingScript(KeyValuePair<int, int>[,] _arTargettingScript) {

        arTargettingScript = _arTargettingScript;

        arScriptedTargettingIndices = new int[arTargettingScript.Length];

    }

    public void SubmitNextSkill() {

        //Save the character who we'll be selecting skills for
        Chr chrToAct = ContTurns.Get().chrNextReady;

        Debug.Assert(chrToAct != null, "Scripted input was asked to submit an skill for a character, but no character is acting");
        Debug.Assert(chrToAct.plyrOwner.id == plyrOwner.id, "Scripted input was asked to submit an skill for a character is doesn't own");

        KeyValuePair<int, int> nextSelection;
        int nTargetsTried = 0;

        SelectionSerializer.SelectionInfo infoSelection;

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

            infoSelection = SelectionSerializer.Deserialize(chrToAct, nextSelection.Value);

            Debug.Log(chrToAct.sName + " wants chosen to use " + infoSelection.ToString());

            //Test to see if this skill would be valid
            if(infoSelection.CanSelect() == false ||
                infoSelection.skillUsed.CanPayMana() == false) {
                Debug.Log("The skill selection would not be legal");

                if(nTargetsTried >= MAXTARGETATTEMPTS) {
                    //If we've tried too many skills with no success, just end our selections
                    // by setting our skill as a rest


                    infoSelection = SelectionSerializer.MakeRestSelection(chrToAct);

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
        Debug.Log(chrToAct.sName + " has automatically chosen to use " + infoSelection.ToString());


        //We now need to ready enough effort mana to pay for the skill
        AutoPayCost(infoSelection.skillUsed.parCost.Get());

        ContSkillSelection.Get().SubmitSkill(infoSelection, this);


    }

    //Figures out and allocates non-effort mana to convert to cover the mana costs
    public void AutoPayCost(int[] arCost) {

        int nEffortToPay = arCost[(int)Mana.MANATYPE.EFFORT] - plyrOwner.mana.nManaPool;
        int nCurMana = 0;

        //Initially, try to pay with mana that isn't in the cost we need to pay for
        while(nCurMana < (int)Mana.MANATYPE.EFFORT) {

            //Check if we've allocated enough effort
            if(nEffortToPay <= 0) return;

            if(arCost[nCurMana] > 0) {
                //Then this type of mana is in the cost, so skip to the next type
                nCurMana++;
            } else {

                if(plyrOwner.mana.HasMana(nCurMana)) {
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
        while(nCurMana < (int)Mana.MANATYPE.EFFORT) {

            //Check if we've allocated enough effort
            if(nEffortToPay <= 0) return;

            //Check if we would have at least 1 mana left over after paying for the skill
            if(plyrOwner.mana.HasMana(nCurMana, 1 + arCost[nCurMana])) {
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


    public static SelectionSerializer.SelectionInfo MakeRandomSelection(Chr chrSource, Skill skillUsed) {

        switch(skillUsed.GetTargetType()) {

        case Clause.TargetType.CHR:
            return new SelectionSerializer.SelectionChr(chrSource, skillUsed, Chr.GetRandomChr(), 0, 0);//Just setting the extra bytes to 0

        case Clause.TargetType.SKILL:
            return new SelectionSerializer.SelectionSkill(chrSource, skillUsed, Chr.GetRandomChr(), chrSource.GetRandomActiveSkill(), 0);

        case Clause.TargetType.PLAYER:
            return new SelectionSerializer.SelectionPlayer(chrSource, skillUsed, Player.GetTargetByIndex(Random.Range(0, Player.MAXPLAYERS)), 0, 0);

        case Clause.TargetType.SPECIAL:
            return new SelectionSerializer.SelectionSpecial(chrSource, skillUsed, 0, 0, 0); // Yea, this doesn't really make much sense unless you do something bigger


        }

        Debug.LogError("Unrecognized targetting type of " + skillUsed);

        return null;
    }


    public static void SetRandomSkills(LocalInputScripted input) {

        int nScriptLength = 100;
        KeyValuePair<int, int>[,] arListRandomSelections = new KeyValuePair<int, int>[Player.MAXCHRS, nScriptLength];

        for(int i = 0; i < Player.MAXCHRS; i++) {

            Chr chr = input.plyrOwner.arChr[i];

            for(int j = 0; j < nScriptLength; j++) {

                //Select a random skill to be used
                Skill skillRandom = chr.GetRandomSkill();

                //Need to create an InfoSelection of the appropriate type
                //Then need to randomly fill this InfoSelection with targetting information
                int nRandomSerialization = MakeRandomSelection(chr, skillRandom).Serialize();

                //TODO:: Update this to be able to select random skills for skill-based targetting (adapt is a particularly complex form of this

                arListRandomSelections[i, j] = new KeyValuePair<int, int>(skillRandom.skillslot.iSlot, nRandomSerialization);
            }
        }

        input.SetTargettingScript(arListRandomSelections);

    }
}
