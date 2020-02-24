using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeChannel : TypeAction {

    public const int nActionPointCost = 1;

    public SoulChannel soulBehaviour;
    public int nStartChannelTime;
    public SelectionSerializer.SelectionInfo infoStoredSelection;

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

    public override string getName() {
        return "Channel";
    }

    public override TYPE Type() {
        return TYPE.CHANNEL;
    }
    public override int GetActionPointCost() {
        return nActionPointCost;
    }

    public override void PayActionPoints() {
        base.PayActionPoints();

        //After paying the action point cost (even though it likely shouldn't matter)
        // we can then transition our readiness state to a channel state
        act.chrSource.SetStateReadiness(new StateChanneling(act.chrSource, nStartChannelTime, new SoulChannel(soulBehaviour, act)));
    }

    public override void UseAction() {

        //When initially using a channel, we don't need to perform any action, so
        // just store the current selections so that we can use them when we complete the channel
        // and execute the effect
        // Note - we just use the base implementation's selection information location
        infoStoredSelection = base.GetSelectionInfo().GetCopy();

    }

    public override SelectionSerializer.SelectionInfo GetSelectionInfo() {
        //Return the selection info that's been stored
        return infoStoredSelection;
    }
}
