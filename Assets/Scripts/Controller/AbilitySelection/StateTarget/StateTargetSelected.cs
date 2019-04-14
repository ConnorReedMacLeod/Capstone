using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTargetSelected : StateTarget {

    public void cbDeselect(Object target, params object[] args) {
        ContLocalInputSelection.Get().SetState(new StateTargetIdle());
    }

    public void cbReselectChar(Object target, params object[] args) {
        // If we now click on a different character, then we'll select them instead
        ContLocalInputSelection.Get().chrSelected.Idle(); // Need to deselect our current character first
        ContLocalInputSelection.Get().chrSelected = ((ViewChr)target).mod;

        ContLocalInputSelection.Get().SetState(new StateTargetSelected());
    }

    public void cbClickAction(Object target, params object[] args) {

        ChooseAction(((ViewAction)target).mod);

    }

    public void cbClickBlockerButton(Object target, params object[] args) {

        ChooseAction(ContLocalInputSelection.Get().chrSelected.arActions[Chr.idBlocking]);

    }

    public void cbClickRestButton(Object target, params object[] args) {

        ChooseAction(ContLocalInputSelection.Get().chrSelected.arActions[Chr.idResting]);

    }

    public void ChooseAction(Action actChosen) {

        // When we've clicked an action, try to use that action

        //If this character is owned by an AI-input player, then we don't have authority and we shouldn't select anything
        if (ContLocalInputSelection.Get().chrSelected.plyrOwner.curInputType == Player.InputType.AI) {
            //NOTE - This will eventually extend to check some authority setting for the local player
            Debug.Log("We can't select actions for a character owned by an AI");
            return;
        }

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

        //If we've reached this point, then we can start filling in the ContCharacterSelection's fields for this character

        ContLocalInputSelection.Get().chrSelected.Targetting();

        ContLocalInputSelection.Get().nSelectedAbility = actChosen.id;

        ContLocalInputSelection.Get().SetTargetArgState(); // Let the parent figure out what exact state we go to
    }

	override public void OnEnter(){
		Debug.Assert(ContLocalInputSelection.Get().chrSelected != null);

        Arena.Get().view.subMouseClick.Subscribe(cbDeselect);
        ViewChr.subAllClick.Subscribe(cbReselectChar);
        ViewAction.subAllClick.Subscribe(cbClickAction);
        ViewBlockerButton.subAllClick.Subscribe(cbClickBlockerButton);
        ViewRestButton.subAllClick.Subscribe(cbClickRestButton);
        KeyBindings.SetBinding(cbClickRestButton, KeyCode.Space);
        KeyBindings.SetBinding(cbClickBlockerButton, KeyCode.B);

        ContLocalInputSelection.Get().chrSelected.Select (); 

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
}
