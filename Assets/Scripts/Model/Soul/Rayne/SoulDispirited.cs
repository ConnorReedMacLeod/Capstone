using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulDispirited : SoulChr {

    public int[] arnCostDebuff;

    //Maintain a list of all of the cost modifiers we've applied
    public LinkedListNode<Property<Mana>.Modifier>[] arnodeCostModifier;


    public SoulDispirited(Chr _chrSource, Chr _chrTarget, Skill _skillSource) : base(_chrSource, _chrTarget, _skillSource) {

        sName = "Dispirited";

        bVisible = true;
        bDuration = true;
        pnMaxDuration = new Property<int>(4);

        //Increase the cost by one effort
        arnCostDebuff = new int[] { 0, 0, 0, 0, 1 };

        arnodeCostModifier = new LinkedListNode<Property<Mana>.Modifier>[Chr.nStandardCharacterSkills];

    }

    public override void ApplicationEffect() {

        //Loop through each skill on the targetted character
        for(int i = 0; i < Chr.nStandardCharacterSkills; i++) {

            ApplyCostIncreaseToSkill(i);
        }


    }

    public void ApplyCostIncreaseToSkill(int iSkill) {
        //TODO BUG (FUTURE) - eventually when you can adapt/switch skills, this will only affect the 
        //                    skills at the time this soul effect was applied.  So if you switch to a skill
        //                    then that skill (even if a cantrip) won't have its cost increased.  Will have to 
        //                    a trigger listener for a skill switch event that will remove swap the applied cost
        //                    modifier for the old skill and apply it to the newly swapped in skill

        //No need to try to reduce the cost of an skill that is null - likely won't come up once characters get the full amount of skills
        if(chrTarget.arSkillSlots[iSkill].skill == null) {
            return;
        }

        Property<Mana>.Modifier costIncrease =
                (mana) => {
                    if(chrTarget.arSkillSlots[iSkill].skill.typeUsage.Type() == TypeUsage.TYPE.CANTRIP) {
                        //Increase the cost if the skill is a cantrip
                        return new Mana(LibFunc.AddArray<int>(mana.arMana, arnCostDebuff, (x, y) => (x + y)));
                    } else {
                        //Otherwise, keep the cost the same
                        return mana;
                    }
                };

        LinkedListNode<Property<Mana>.Modifier> costChange = chrTarget.arSkillSlots[iSkill].skill.ChangeCost(costIncrease);

        arnodeCostModifier.SetValue(costChange, iSkill);

        //UNNEEDED CURRENTLY - ONLY FOR AFFECTING THE FIRST USED SKILL
        //chrTarget.subPostExecuteSkill.Subscribe(OnSkillUsage);
    }

    public SoulDispirited(SoulDispirited other, Chr _chrTarget = null) : base(other, _chrTarget) {

        arnCostDebuff = new int[Mana.nManaTypes];
        System.Array.Copy(other.arnCostDebuff, arnCostDebuff, other.arnCostDebuff.Length);
        arnodeCostModifier = new LinkedListNode<Property<Mana>.Modifier>[Chr.nStandardCharacterSkills];
        System.Array.Copy(other.arnodeCostModifier, arnodeCostModifier, other.arnodeCostModifier.Length);

    }

    public override void RemoveEffect() {
        //When removed we'll clear all the cost modifiers we've applied
        for(int i = 0; i < Chr.nStandardCharacterSkills; i++) {
            chrTarget.arSkillSlots[i].skill.manaCost.pManaCost.RemoveModifier(arnodeCostModifier[i]);
        }

        //chrTarget.subPostExecuteSkill.UnSubscribe(OnSkillUsage);
    }

    // **** CURRENTLY UNUSED EFFECT TO MAKE NEXT SKILL COST 1 MORE ******

    public void OnSkillUsage(Object target, params object[] args) {

        //Ignore if the skill used wasn't used by the character who has this soul effect
        if(((Skill)args[0]).chrOwner != this.chrTarget) return;

        //Check if the skill that was just used is a character skill - not a generic (rest)
        if(((Skill)args[0]).IsStandardSkill()) {

            //if the used skill was a character skill, then we can dispell this effect
            // sicne we only want to make the first used skill cost more
            soulContainer.RemoveSoul(this);
        }
    }

}



