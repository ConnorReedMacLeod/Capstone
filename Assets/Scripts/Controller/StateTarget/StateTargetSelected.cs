using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTargetSelected : StateTarget {

    public void cbDeselect(Object target, params object[] args) {
        contTarg.SetState(new StateTargetIdle(contTarg));
    }

    public void cbReselectChar(Object target, params object[] args) {
        // If we now click on a different character, then we'll select them instead
        contTarg.selected.Idle(); // Need to deselect our current character first
        contTarg.selected = ((ViewChr)target).mod;

        contTarg.SetState(new StateTargetSelected(contTarg));
    }

    public void cbChooseAction(Object target, params object[] args) {
        // When we've clicked an action, use that action

        contTarg.selected.Targetting();
        contTarg.selected.nUsingAction = ((ViewAction)target).id;

        // TODO:: Save the current targets if there are any, so that you can 
        // revert to those targets if you've failed targetting
        contTarg.ResetTar();
        contTarg.SetTargetArgState(); // Let the parent figure out what exact state we go to
    }

	override public void OnEnter(){
        Debug.Log("Entering Selected");
		Debug.Assert(contTarg.selected != null);

        Arena.Get().view.subMouseClick.Subscribe(cbDeselect);
        ViewChr.subAllClick.Subscribe(cbReselectChar);
        ViewAction.subAllClick.Subscribe(cbChooseAction);

        contTarg.selected.Select (); 

	}

	override public void OnLeave(){
        Arena.Get().view.subMouseClick.UnSubscribe(cbDeselect);
        ViewChr.subAllClick.UnSubscribe(cbReselectChar);
        ViewAction.subAllClick.UnSubscribe(cbChooseAction);
    }


	public StateTargetSelected(ContTarget _contTarg): base(_contTarg){
        
    }
}
