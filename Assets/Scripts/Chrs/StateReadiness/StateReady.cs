using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateReady : StateReadiness {

    public int nCurSkillsLeft;     //The number of skills left in a turn that the character can use (cantrips cost 0)
    public int nQueuedFatigue;      //The amount of fatigue that will be added to the characters fatigue when they're done acting for the turn


    public StateReady(Chr _chrOwner, int _nCurActionsLeft) : base(_chrOwner) {

        nCurSkillsLeft = _nCurActionsLeft;

    }

    public override TYPE Type() {
        return TYPE.READY;
    }

    public override void Recharge() {
        if(chrOwner.bDead) {
            Debug.Log("Tried to recharge, but " + chrOwner.sName + " is dead");
            return;
        }

        Debug.Log("We shouldn't be able to recharge while in the ready state");
    }

    public override bool CanSelectSkill(Skill skill) {
        //We actually can select another skill if we're in the Ready state

        if(!skill.type.Usable()) {
            //Then this type of skill cannot be activated

            return false;
        }

        if(skill.type.GetSkillPointCost() > nCurSkillsLeft) {
            //Then we don't have enough skill activations left for this character to use the skill

            return false;
        }

        return true;
    }


    public override void OnEnter() {

    }

    public override void OnLeave() {



    }
}
