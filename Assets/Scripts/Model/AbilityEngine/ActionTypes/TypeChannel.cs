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
        if(_soulBehaviour != null) {
            soulBehaviour = _soulBehaviour;
        } else {
            //Otherwise just make a blank one
            Debug.Log("Warning - making a blank channel soul behaviour");
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

    public override void UseAction() {

        //Store a copy of the current SelectionInfo so that we can use it later when the channel finishes (or triggers in some way)
        infoStoredSelection = base.GetSelectionInfo().GetCopy();

        //We don't need to perform any real action on starting channeling other than changing our readiness state so that the 
        // soulchannel effect can be applied (and do any on-application effects if necessary)
        act.chrSource.SetStateReadiness(new StateChanneling(act.chrSource, nStartChannelTime, new SoulChannel(soulBehaviour, act)));



    }

    public override SelectionSerializer.SelectionInfo GetSelectionInfo() {
        //Return the selection info that's been stored
        return infoStoredSelection;
    }

    public void ClearStoredSelectionInfo() {
        infoStoredSelection = null;
    }
}
