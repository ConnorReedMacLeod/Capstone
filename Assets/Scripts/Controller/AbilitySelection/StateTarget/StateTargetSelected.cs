using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTargetSelected : StateTarget {

    public void cbDeselect(Object target, params object[] args) {
        ContLocalUIInteraction.Get().SetState(new StateTargetIdle());
    }

    public void cbReselectChar(Object target, params object[] args) {
        // If we now click on a different character, then we'll select them instead
        ContLocalUIInteraction.Get().chrSelected.Idle(); // Need to deselect our current character first
        ContLocalUIInteraction.Get().chrSelected = ((ViewChr)target).mod;

        ContLocalUIInteraction.Get().SetState(new StateTargetSelected());
    }

    public void cbClickAction(Object target, params object[] args) {

        ContLocalUIInteraction.Get().StartTargetting(((ViewAction)target).mod);

    }

    public void cbClickBlockerButton(Object target, params object[] args) {

        Action actBlocking = ContLocalUIInteraction.Get().chrSelected.arActions[Chr.idBlocking];
        ContLocalUIInteraction.Get().StartTargetting(actBlocking);

    }

    public void cbClickRestButton(Object target, params object[] args) {

        Action actRest = ContLocalUIInteraction.Get().chrSelected.arActions[Chr.idResting];
        ContLocalUIInteraction.Get().StartTargetting(actRest);

    }


    override public void OnEnter() {
        Debug.Assert(ContLocalUIInteraction.Get().chrSelected != null);

        Arena.Get().view.subMouseClick.Subscribe(cbDeselect);
        ViewChr.subAllClick.Subscribe(cbReselectChar);
        ViewAction.subAllClick.Subscribe(cbClickAction);
        ViewBlockerButton.subAllClick.Subscribe(cbClickBlockerButton);
        ViewRestButton.subAllClick.Subscribe(cbClickRestButton);
        KeyBindings.SetBinding(cbClickRestButton, KeyCode.Space);
        KeyBindings.SetBinding(cbClickBlockerButton, KeyCode.B);

        ContLocalUIInteraction.Get().chrSelected.Select();

    }

    override public void OnLeave() {

        Arena.Get().view.subMouseClick.UnSubscribe(cbDeselect);
        ViewChr.subAllClick.UnSubscribe(cbReselectChar);
        ViewAction.subAllClick.UnSubscribe(cbClickAction);
        ViewBlockerButton.subAllClick.UnSubscribe(cbClickBlockerButton);
        ViewRestButton.subAllClick.UnSubscribe(cbClickRestButton);
        KeyBindings.Unbind(KeyCode.Space); //clear the binding
        KeyBindings.Unbind(KeyCode.B);//clear the binding
    }
}
