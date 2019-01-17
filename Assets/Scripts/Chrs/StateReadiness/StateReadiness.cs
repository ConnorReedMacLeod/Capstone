using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:: Consider if this system of introducing a base method that only one subclass actually
//        implements is actually a good practice
public abstract class StateReadiness {

    public enum TYPE { READY, FATIGUED, STUNNED, CHANNELING};

    public Chr chrOwner;

    public StateReadiness(Chr _chrOwner) {

        chrOwner = _chrOwner;

    }

    public abstract TYPE Type();

    public virtual bool CanSelectAction(Action act) {
        //By default, you can't select any action

        return false;
    }

    //Call to transition to the ready state if we're at 0 fatigue
    public virtual void Ready() {
        //By default, you can't transition to the ready state unless you're fatigued

    }

    //Called at the beginning of turn to reduce fatigue
	public virtual void Recharge() {

        //By default, we just reduce fatigue by 1 (with the beginning of turn flag)
        ContAbilityEngine.Get().AddExec(new ExecChangeFatigue() {
            chrSource = null,
            chrTarget = chrOwner,

            nAmount = -1,
            bBeginningTurn = true,

        });

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

