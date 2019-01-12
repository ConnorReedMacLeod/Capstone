using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulDispirited : Soul {

    public int[] arnCostDebuff;

    //Maintain a list of all of the cost modifiers we've applied
    public LinkedListNode<Property<int[]>.Modifier>[] arnodeCostModifier;



    //Not currently useing this - was used for if this only makes the next ability you use cost more
    public void OnAbilityUsage(Object target, params object[] args) {

        //Ignore if the action used wasn't used by the character who has this soul effect
        if (((Action)args[0]).chrSource != this.chrTarget) return;

        //Check if the action that was just used is a character action - not a generic (block/rest)
        if (((Action)args[0]).id < Chr.nCharacterActions) {

            //Otherwise dispell the debuff
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

        arnodeCostModifier = new LinkedListNode<Property<int[]>.Modifier>[Chr.nCharacterActions];

        funcOnApplication = () => {

            //Loop through each ability on the targetted character
            for (int i = 0; i < Chr.nCharacterActions; i++) {

                LibFunc.Get<Action> getAction = LibFunc.ReturnSnapShot<Action>(chrTarget.arActions[i]);

                Property<int[]>.Modifier costIncrease =
                    (arCost) => {
                        if (getAction().type == ActionTypes.TYPE.CANTRIP) {
                        //Increase the cost if the ability is a cantrip
                        return LibFunc.AddArray<int>(arCost, arnCostDebuff, (x, y) => (x + y));
                        } else {
                        //Otherwise, keep the cost the same
                        return arCost;
                        }
                    };

                arnodeCostModifier[i] = chrTarget.arActions[i].ChangeCost(costIncrease);

            }

            //chrTarget.subPostExecuteAbility.Subscribe(OnAbilityUsage);
        };

        funcOnRemoval = () => {
            //When removed we'll clear all the cost modifiers we've applied
            for (int i = 0; i < Chr.nCharacterActions; i++) {
                chrTarget.arActions[i].parCost.RemoveModifier(arnodeCostModifier[i]);
            }

            //chrTarget.subPostExecuteAbility.UnSubscribe(OnAbilityUsage);
        };

    }
}