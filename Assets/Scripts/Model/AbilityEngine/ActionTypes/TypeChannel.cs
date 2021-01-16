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

        //Note that the soulbehaviour will be invisible and have infinite duration - it will just be removed
        //  by some non-time related trigger (typically the character transitioning away from a Channeling State)
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

        Debug.Log("Pushing a begin channel clause");

        ContAbilityEngine.PushSingleClause(new ClauseBeginChannel(act));
    }

    public override SelectionSerializer.SelectionInfo GetSelectionInfo() {
        //Return the selection info that's been stored
        return infoStoredSelection;
    }

    public void ClearStoredSelectionInfo() {
        infoStoredSelection = null;
    }

    class ClauseBeginChannel : ClauseSpecial {

        public ClauseBeginChannel(Action _act) : base(_act) {
        }

        public override string GetDescription() {
            return string.Format("Transition to a channeling state");
        }

        public override void ClauseEffect() {

            ContAbilityEngine.PushSingleExecutable(new ExecBeginChannel(action.chrSource, action.chrSource, action));

        }

    };

}
