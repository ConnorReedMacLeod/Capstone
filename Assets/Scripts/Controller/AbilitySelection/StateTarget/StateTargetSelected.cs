using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTargetSelected : StateTarget {

    public void cbDeselect(Object target, params object[] args) {
        inputHuman.SetState(new StateTargetIdle(inputHuman));
    }

    public void cbReselectChar(Object target, params object[] args) {
        // If we now click on a different character, then we'll select them instead
        inputHuman.selected.Idle(); // Need to deselect our current character first
        inputHuman.selected = ((ViewChr)target).mod;

        inputHuman.SetState(new StateTargetSelected(inputHuman));
    }

    public void cbClickAction(Object target, params object[] args) {
        ChooseAction(((ViewAction)target).mod);
    }

    public void cbClickBlockerButton(Object target, params object[] args) {
        ChooseAction(inputHuman.selected.arActions[Chr.idBlocking]);
    }

    public void ChooseAction(Action actChosen) {
        // When we've clicked an action, use that action
        //Debug.Log(actChosen + " is is being targetted");

        // But first, check if targetting is locked
        if (actChosen.chrSource.bLockedTargetting) {
            Debug.Log("We can't choose an action for a locked character");
            return;
        }

        // And check if it's on cooldown
        if(actChosen.nCurCD > 0) {
            Debug.Log("We can't use an ability that's on cooldown");
            return;
        }

        if(!actChosen.chrSource.curStateReadiness.CanSelectAction(actChosen)) {
            Debug.Log("We can't use an action right now cause our state doesn't allow it");
            return;
        }

        inputHuman.selected.Targetting();
        
        inputHuman.nSelectedAbility = actChosen.id;

        // TODO:: Save the current targets if there are any, so that you can 
        // revert to those targets if you've failed targetting
        inputHuman.ResetTar();
        inputHuman.SetTargetArgState(); // Let the parent figure out what exact state we go to
    }

	override public void OnEnter(){
		Debug.Assert(inputHuman.selected != null);

        Arena.Get().view.subMouseClick.Subscribe(cbDeselect);
        ViewChr.subAllClick.Subscribe(cbReselectChar);
        ViewAction.subAllClick.Subscribe(cbClickAction);
        ViewBlockerButton.subAllClick.Subscribe(cbClickBlockerButton);

        inputHuman.selected.Select (); 

	}

	override public void OnLeave(){
        Arena.Get().view.subMouseClick.UnSubscribe(cbDeselect);
        ViewChr.subAllClick.UnSubscribe(cbReselectChar);
        ViewAction.subAllClick.UnSubscribe(cbClickAction);
        ViewBlockerButton.subAllClick.UnSubscribe(cbClickBlockerButton);
    }


	public StateTargetSelected(InputHuman _inputHuman) : base(_inputHuman) {
        
    }
}
