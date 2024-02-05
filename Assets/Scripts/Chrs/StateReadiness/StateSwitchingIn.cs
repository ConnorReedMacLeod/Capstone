﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This gets applied to characters when they're swapping in from the bench.
//  - Prevents any skills from targetting this character
//  - Prevents the character from readying (even at 0 fatigue)
//  - Prevents almost all executables from affecting the character (unless they are from soul effects already on the character)
public class StateSwitchingIn : StateReadiness {

    public int nSwitchingInDuration;

    public LinkedListNode<Property<Chr.CanBeSelectedBy>.Modifier> modifierCannotBeSelected;

    public StateSwitchingIn(Chr _chrOwner, int _nSwitchingInDuration) : base(_chrOwner) {

        nSwitchingInDuration = _nSwitchingInDuration;

    }

    public override TYPE Type() {
        return TYPE.SWITCHINGIN;
    }

    public override int GetPriority() {
        //Since fatigue and switchin time both decrease each turn, the Chr will have to wait for the longer of the two to be able to act
        return Mathf.Max(chrOwner.nFatigue, chrOwner.nSwitchingInTime);
    }

    public override void Recharge() {
        //TODO Should we be doing the base Recharge stuff like reducing fatigue?
        base.Recharge();

        //By default, we just reduce fatigue by 1 (with the beginning of turn flag)
        ContSkillEngine.Get().AddExec(new ExecChangeSwitchInTime(null, chrOwner, -1));
        
    }

    public override void ReadyIfNoFatigue() {
        //Don't need to do anything since we shouldn't be readying with a switching in character
    }

    private Property<Chr.CanBeSelectedBy>.Modifier GetCannotBeSelectedModifier() {
        //We apply the CanBeSelectedBy function below us to the provided arguments, and then
        // basically ignore its result and just return false anyway since no targetting should be possible
        return (Chr.CanBeSelectedBy fnCanBeSelectedByBelow) => ((TarChr tar, InputSkillSelection selectionsSoFar, bool bCanRegularlySelect) => 
        fnCanBeSelectedByBelow(tar, selectionsSoFar, bCanRegularlySelect) && false
        );
    }


    public override void OnEnter() {

        //When switching in, we'll apply a targetting-override for the character to stop them from being targetted by any new targetted skills
        modifierCannotBeSelected = chrOwner.pOverrideCanBeSelectedBy.AddModifier(GetCannotBeSelectedModifier());

        //Notify anyone (mostly UI stuff) that the switching in duration has changed
        chrOwner.subSwitchingInChange.NotifyObs();
        
        //Set the initial switch-in duration
        ContSkillEngine.Get().AddExec(new ExecChangeSwitchInTime(null, chrOwner, nSwitchingInDuration));

    }

    public override void OnLeave() {

        //When we've finished switching in (or potentially have gone back to the bench, we can remove our targetting-override modifier
        chrOwner.pOverrideCanBeSelectedBy.RemoveModifier(modifierCannotBeSelected);

    }
}