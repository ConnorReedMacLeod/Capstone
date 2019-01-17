using System.Collections;
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
    }

    public override TYPE Type() {
        return TYPE.CHANNELING;
    }

    //To be called as part of a stun, before transitioning to the stunned state
    public override void InterruptChannel() {

        //In preperation for cancelling the channel, nullify the action the channel would take when
        //it would be removed naturally (via completion)
        soulBehaviour.funcOnRemoval = null;
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

    }

    public override void OnLeave() {

        chrOwner.soulContainer.RemoveSoul(soulBehaviour);

    }

}
