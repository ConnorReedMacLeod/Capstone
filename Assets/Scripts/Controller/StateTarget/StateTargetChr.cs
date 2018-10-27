using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used for targgeting a specific character
public class StateTargetChr : StateTarget {

	TargetArgChr tarArg;

    public void cbCancelTargetting(object target, params object [] args) {
        ResetTargets();
        contTarg.CancelTar();
    }

    public void cbSetTargetChr(object target, params object [] args) {
        if (tarArg.setTar(((ViewChr)target).mod)) {
            Debug.Log("Target successfully set to " + ((ViewChr)target).mod.sName);

            //move to next target
            contTarg.IncTar();

            contTarg.SetTargetArgState();
        } else {
            Debug.Log(((ViewChr)target).mod.sName + ", on team " + ((ViewChr)target).mod.ownerID + " is not a valid character target");
        }
    }

    public void cbSwitchAction(Object target, params object [] args) {
        ResetTargets();

        contTarg.selected.nUsingAction = ((ViewAction)target).id;

        // TODO:: Save the current targets if there are any, so that you can 
        // revert to those targets if you've failed targetting
        contTarg.ResetTar();
        contTarg.SetTargetArgState(); // Let the parent figure out what exact state we go to

    }

	override public void OnEnter(){

		Debug.Assert(contTarg.selected != null);
        //BUG :: THIS MAY CAUSE AN ERROR IF AN ALLY TARGET IS CAST TO A NORMAL CHR TARGET - MAY NOT CHECK FOR SAME TEAM
		tarArg = (TargetArgChr)contTarg.selected.arActions [contTarg.selected.nUsingAction].arArgs[contTarg.nTarCount];

        Arena.Get().view.subMouseClick.Subscribe(cbCancelTargetting);
        ViewInteractive.subGlobalMouseRightClick.Subscribe(cbCancelTargetting);

        ViewChr.subAllClick.Subscribe(cbSetTargetChr);
        ViewAction.subAllClick.Subscribe(cbSwitchAction);

    }

	override public void OnLeave(){
        Arena.Get().view.subMouseClick.UnSubscribe(cbCancelTargetting);
        ViewInteractive.subGlobalMouseRightClick.UnSubscribe(cbCancelTargetting);

        ViewChr.subAllClick.UnSubscribe(cbSetTargetChr);
        ViewAction.subAllClick.UnSubscribe(cbSwitchAction);
    }

	public void ResetTargets(){
		//clear any targetting 
		//TODO:: maybe only reset the targets to whatever was selected before?
		contTarg.selected.arActions [contTarg.selected.nUsingAction].Reset ();

		//contTarg.SetState (new StateTargetIdle (contTarg));
	}

		
	public StateTargetChr(ContTarget _contTarg): base(_contTarg){

	}
}
