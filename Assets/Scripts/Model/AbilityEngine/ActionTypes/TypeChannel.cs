using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeChannel : TypeAction {

    public const int nActionPointCost = 1;

    public SoulChannel soulBehaviour;
    public int nStartChannelTime;

    public TypeChannel(Action act, int _nStartChannelTime, SoulChannel _soulBehaviour) : base(act) {

        nStartChannelTime = _nStartChannelTime;

        //If we've been given special soul effect, then use it
        if (_soulBehaviour != null) {
            soulBehaviour = _soulBehaviour;
        } else {
            //Otherwise just make a blank one
            soulBehaviour = new SoulChannel(act);
        }
    }

    public override TYPE Type() {
        return TYPE.CHANNEL;
    }
    public override int GetActionPointCost() {
        return nActionPointCost;
    }

    public override void UseAction() {

        //It's a bit weird to reduce your action cost on a channel since you're
        //gonna be switching states, but it should be done for the sake of completeness
        PayActionPoints();

        //Move to a Channel State
        act.chrSource.SetStateReadiness(new StateChanneling(act.chrSource, nStartChannelTime, new SoulChannel(soulBehaviour, act)));

        //Pay the fatigue cost for the action
        act.PayFatigue();
    }
}
