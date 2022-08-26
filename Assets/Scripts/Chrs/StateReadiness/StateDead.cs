using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateDead : StateReadiness {

    public StateDead(Chr _chrOwner) : base(_chrOwner) {

    }

    public override TYPE Type() {
        return TYPE.DEAD;
    }

    public override void ReadyIfNoFatigue() {
        //Can't ready when dead
    }

    public override bool CanSelectSkill(Skill skill) {
        //Can't select a skill while dead
        return false;
    }

    public override int GetPriority() {
        //return an impossibly high priority value so we can't be selected
        return 999999;
    }

    public override void Recharge() {
        //Nothing should be recharged when dead

    }

    public override void OnEnter() {
        //Not much we have to do here - we'll already be handling much of the
        // death-cleanup in our chr's methods.  If we leave our channeling state to enter
        // this one, then we'll auto-cancel that channel though

    }
}
