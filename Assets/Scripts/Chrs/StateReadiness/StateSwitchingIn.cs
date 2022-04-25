using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This gets applied to characters when they're swapping in from the bench.
//  - Prevents any skills from targetting this character
//  - Prevents the character from readying (even at 0 fatigue)
//  - Prevents almost all executables from affecting the character (unless they are from soul effects already on the character)
public class StateSwitchingIn : StateReadiness {

    public int nSwitchingInDuration;

    public StateSwitchingIn(Chr _chrOwner, int _nSwitchingInDuration) : base(_chrOwner) {

        nSwitchingInDuration = _nSwitchingInDuration;

    }

    public override TYPE Type() {
        return TYPE.SWITCHINGIN;
    }

    public override void Recharge() {
        base.Recharge();

        //In addition to the base recharge, we'll also reduce the switching in timer
        nSwitchingInDuration--;

        if(nSwitchingInDuration <= 0) {
            chrOwner.SetStateReadiness(new StateFatigued(chrOwner));
        }
    }

    public override void ReadyIfNoFatigue() {
        //Don't need to do anything since we shouldn't be readying with a switching in character
    }

    public override void OnEnter() {

    }

    public override void OnLeave() {

    }
}