﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChanneling : StateReadiness {

    public int nChannelTime;  

    public SoulChannel soulBehaviour; //Handles all customized behaviour of what the channel effect should do

    public StateChanneling(Chr _chrOwner, int _nChannelTime, SoulChannel _soulBehaviour) : base(_chrOwner) {

        nChannelTime = _nChannelTime;

        //Double check that the soul isn't visible - should just be a hidden implementation
        Debug.Assert(_soulBehaviour.bVisible == false);
        soulBehaviour = _soulBehaviour;

        Debug.Log("soulBehaviour's action is initially " + soulBehaviour.act);
    }

    public override TYPE Type() {
        return TYPE.CHANNELING;
    }


    public override int GetPriority() {
        //The priority of a channeling character should also include the channel time remaining
        return nChannelTime + base.GetPriority();
    }
    //To be called as part of a stun, before transitioning to the stunned state
    public override void InterruptChannel() {

        Debug.Log("Interuptting an ability with " + soulBehaviour.act);

        //Change the function's onRemoval effect to its interrupted function
        soulBehaviour.funcOnRemoval = soulBehaviour.OnInterruptedCompletion;
    }

    public override void ChangeChanneltime(int _nChange) {
        //We can actually reduce the channel time if we're in this state

        if (_nChange + nChannelTime < 0) {
            nChannelTime = 0;
        } else {
            nChannelTime += _nChange;
        }

        //If, for any reason, we've now been put to 0 channeltime, then our channel completes
        // and we transition to the fatigued state
        if(nChannelTime == 0) {
            ContAbilityEngine.Get().AddExec(new ExecCompleteChannel() {

                chrSource = null, //This is a game action, so there's no source
                chrTarget = chrOwner

            });
        }

    }


    public override void Recharge() {
        //If we're channeling, instead of reducing fatigue, we only reduce the channel time
        ContAbilityEngine.Get().AddExec(new ExecChangeChannel() {
            chrSource = null, //This is a game action, so there's no source
            chrTarget = chrOwner,

            nAmount = -1,
        });

    }

    public override void OnEnter() {

        chrOwner.soulContainer.ApplySoul(soulBehaviour);

        //Once we're in this state, let people know that the channel time has taken effect
        chrOwner.subChannelTimeChange.NotifyObs();

    }

    public override void OnLeave() {

        chrOwner.soulContainer.RemoveSoul(soulBehaviour);

    }

}