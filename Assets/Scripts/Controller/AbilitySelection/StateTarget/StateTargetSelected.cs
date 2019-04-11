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

        //If we're clicking on the ability of a character we don't own, then don't do any selection for them
        if (inputHuman.selected.plyrOwner.id != inputHuman.plyrOwner.id) return;

        ChooseAction(((ViewAction)target).mod);
    }

    public void cbClickBlockerButton(Object target, params object[] args) {

        //If we're clicking on the ability of a character we don't own, then don't do any selection for them
        if (inputHuman.selected.plyrOwner.id != inputHuman.plyrOwner.id) return;

        ChooseAction(inputHuman.selected.arActions[Chr.idBlocking]);
    }

    public void cbClickRestButton(Object target, params object[] args) {

        //If we're clicking on the ability of a character we don't own, then don't do any selection for them
        if (inputHuman.selected.plyrOwner.id != inputHuman.plyrOwner.id) return;

        ChooseAction(inputHuman.selected.arActions[Chr.idResting]);
    }

    public void ChooseAction(Action actChosen) {

        Debug.Assert(actChosen.chrSource.plyrOwner.id == inputHuman.plyrOwner.id);

        // When we've clicked an action, use that action
        Debug.Log(actChosen + " is is being targetted for character " + inputHuman.selected.sName);

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

        inputHuman.SetTargetArgState(); // Let the parent figure out what exact state we go to
    }

	override public void OnEnter(){
        Debug.Log("Entering StateTargetSelected");
		Debug.Assert(inputHuman.selected != null);

        Arena.Get().view.subMouseClick.Subscribe(cbDeselect);
        ViewChr.subAllClick.Subscribe(cbReselectChar);
        ViewAction.subAllClick.Subscribe(cbClickAction);
        ViewBlockerButton.subAllClick.Subscribe(cbClickBlockerButton);
        ViewRestButton.subAllClick.Subscribe(cbClickRestButton);
        KeyBindings.SetBinding(cbClickRestButton, KeyCode.Space);
        KeyBindings.SetBinding(cbClickBlockerButton, KeyCode.B);

        inputHuman.selected.Select (); 

	}

	override public void OnLeave(){
        Arena.Get().view.subMouseClick.UnSubscribe(cbDeselect);
        ViewChr.subAllClick.UnSubscribe(cbReselectChar);
        ViewAction.subAllClick.UnSubscribe(cbClickAction);
        ViewBlockerButton.subAllClick.UnSubscribe(cbClickBlockerButton);
        ViewRestButton.subAllClick.UnSubscribe(cbClickRestButton);
        KeyBindings.Unbind(KeyCode.Space); //clear the binding
        KeyBindings.Unbind(KeyCode.B);//clear the binding
    }


	public StateTargetSelected(InputHuman _inputHuman) : base(_inputHuman) {
        
    }
}
