using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulDispirited : Soul {

    public int[] arnCostDebuff;

    //Maintain a list of all of the cost modifiers we've applied
    public LinkedListNode<Property<int[]>.Modifier>[] arnodeCostModifier;


    public void OnAbilityUsage(Object target, params object[] args) {

        //Check if the action that was just used is a character action - not a generic (block/rest)
        if (((Action)args[0]).id < Chr.nCharacterActions) {
            //Then dispell the debuff
            soulContainer.RemoveSoul(this);
        }


    }

    public SoulDispirited(Chr _chrSource, Chr _chrTarget) : base(_chrSource, _chrTarget) {

        sName = "Dispirited";

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(4);

        //Increase the cost by one effort
        arnCostDebuff = new int[] { 0, 0, 0, 0, 1 };
    }

    public override void funcOnApplication() {

        //Loop through each ability on the targetted character
        for (int i = 0; i < Chr.nCharacterActions; i++) {
            arnodeCostModifier[i] = chrTarget.arActions[i].parCost.AddModifier(
                (arCost) => {
                    if (chrTarget.arActions[i].type == Action.ActionType.CANTRIP) { 
                        //Increase the cost if the ability is a cantrip
                        return LibFunc.AddArray<int>(arCost, arnCostDebuff, (x, y) => (x + y));
                    } else {
                        //Otherwise, keep the cost the same
                        return arCost;
                    }
                });

        }

        chrTarget.subPostExecuteAbility.Subscribe(OnAbilityUsage);
    }

    public override void funcOnRemoval() {

        //When removed we'll clear all the cost modifiers we've applied
        for (int i = 0; i < Chr.nCharacterActions; i++) {
            chrTarget.arActions[i].parCost.RemoveModifier(arnodeCostModifier[i]);
        }

        chrTarget.subPostExecuteAbility.UnSubscribe(OnAbilityUsage);

    }

}