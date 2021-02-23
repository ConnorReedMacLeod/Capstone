using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulDispirited : Soul {

    public int[] arnCostDebuff;

    //Maintain a list of all of the cost modifiers we've applied
    public LinkedListNode<Property<int[]>.Modifier>[] arnodeCostModifier;


    public SoulDispirited(Chr _chrSource, Chr _chrTarget, Action _actSource) : base(_chrSource, _chrTarget, _actSource) {

        sName = "Dispirited";

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(4);

        //Increase the cost by one effort
        arnCostDebuff = new int[] { 0, 0, 0, 0, 1 };

        arnodeCostModifier = new LinkedListNode<Property<int[]>.Modifier>[Chr.nLoadoutSkills];

    }

    public override void ApplicationEffect() {

        //Loop through each ability on the targetted character
        for(int i = 0; i < Chr.nLoadoutSkills; i++) {

            ApplyCostIncreaseToSkill(i);
        }


    }

    public void ApplyCostIncreaseToSkill(int iSkill) {
        //TODO BUG (FUTURE) - eventually when you can adapt/switch abilities, this will only affect the 
        //                    abilities at the time this soul effect was applied.  So if you switch to an ability
        //                    then that ability (even if a cantrip) won't have its cost increased.  Will have to 
        //                    a trigger listener for an ability switch event that will remove swap the applied cost
        //                    modifier for the old ability and apply it to the newly swapped in ability

        //No need to try to reduce the cost of an ability that is null - likely won't come up once characters get the full amount of abilities
        if(chrTarget.arSkills[iSkill] == null) {
            return;
        }

        Property<int[]>.Modifier costIncrease =
                (arCost) => {
                    if(chrTarget.arSkills[iSkill].type.Type() == TypeAction.TYPE.CANTRIP) {
                        //Increase the cost if the ability is a cantrip
                        return LibFunc.AddArray<int>(arCost, arnCostDebuff, (x, y) => (x + y));
                    } else {
                        //Otherwise, keep the cost the same
                        return arCost;
                    }
                };

        LinkedListNode<Property<int[]>.Modifier> costChange = chrTarget.arSkills[iSkill].ChangeCost(costIncrease);

        arnodeCostModifier.SetValue(costChange, iSkill);

        //UNNEEDED CURRENTLY - ONLY FOR AFFECTING THE FIRST USED ABILITY
        //chrTarget.subPostExecuteAbility.Subscribe(OnAbilityUsage);
    }

    public SoulDispirited(SoulDispirited other, Chr _chrTarget = null) : base(other) {
        if(_chrTarget != null) {
            //If a Target was provided, then we'll use that
            chrTarget = _chrTarget;
        } else {
            //Otherwise, just copy from the other object
            chrTarget = other.chrTarget;
        }

        arnCostDebuff = new int[Mana.nManaTypes];
        System.Array.Copy(other.arnCostDebuff, arnCostDebuff, other.arnCostDebuff.Length);
        arnodeCostModifier = new LinkedListNode<Property<int[]>.Modifier>[Chr.nLoadoutSkills];
        System.Array.Copy(other.arnodeCostModifier, arnodeCostModifier, other.arnodeCostModifier.Length);

    }

    public override void RemoveEffect() {
        //When removed we'll clear all the cost modifiers we've applied
        for(int i = 0; i < Chr.nLoadoutSkills; i++) {
            chrTarget.arSkills[i].parCost.RemoveModifier(arnodeCostModifier[i]);
        }

        //chrTarget.subPostExecuteAbility.UnSubscribe(OnAbilityUsage);
    }

    // **** CURRENTLY UNUSED EFFECT TO MAKE NEXT ABILITY COST 1 MORE ******

    public void OnAbilityUsage(Object target, params object[] args) {

        //Ignore if the action used wasn't used by the character who has this soul effect
        if(((Action)args[0]).chrSource != this.chrTarget) return;

        //Check if the action that was just used is a character action - not a generic (block/rest)
        if(((Action)args[0]).id < Chr.nActiveCharacterSkills) {

            //if the used action was a character action, then we can dispell this effect
            // sicne we only want to make the first used skill cost more
            soulContainer.RemoveSoul(this);
        }
    }

}



