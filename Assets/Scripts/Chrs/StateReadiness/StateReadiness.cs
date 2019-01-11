using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateReadiness {

    public Chr chrOwner;

    public StateReadiness(Chr _chrOwner) {

        chrOwner = _chrOwner;

    }

    public virtual bool CanSelectAction() {
        //By default, you can't select an action

        return false;
    }

    //Called at the beginning of turn to reduce fatigue
	public virtual void Recharge() {
        //By default, we just reduce fatigue by 1 (with the beginning of turn flag)

        chrOwner.ChangeFatigue(-1, true);

    }

    //TODO:: Maybe there's a better way of doing this?
    public virtual void InterruptChannel() {
        //By default, do nothing, since we're not channeling

        Debug.Log("Can't interrupt since we're not channeling right now");
    }

    public virtual void ChangeChanneltime(int _nChange) {
        //By default, do nothing, since we're not channeling

        Debug.Log("Can't reduce channeltime since we're not channeling right now");

    }

    public virtual void OnEnter() { }
    public virtual void OnLeave() { }


}

