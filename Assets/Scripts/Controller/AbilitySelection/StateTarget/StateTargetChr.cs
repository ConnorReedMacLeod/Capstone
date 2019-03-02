using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used for targgeting a specific character
public class StateTargetChr : StateTarget {

	TargetArgChr tarArg;

    public void cbCancelTargetting(object target, params object [] args) {
        inputHuman.CancelTar();
    }

    public void cbSetTargetChr(object target, params object [] args) {

        int idTarget = ((ViewChr)target).mod.GetTargettingId();

        if (tarArg.WouldBeLegal(idTarget)) { 

            //move to next target
            inputHuman.StoreTargettingIndex(idTarget);

            Debug.Log("Target successfully set to " + ((ViewChr)target).mod.sName);

        } else {
            Debug.Log(((ViewChr)target).mod.sName + ", on team " + ((ViewChr)target).mod.plyrOwner.id + " is not a valid character target");
        }
    }

    public void cbSwitchAction(Object target, params object [] args) {

        inputHuman.nSelectedAbility = ((ViewAction)target).id;

        inputHuman.ResetTar();
        inputHuman.SetTargetArgState(); // Let the parent figure out what exact state we go to

    }

	override public void OnEnter(){

		Debug.Assert(inputHuman.selected != null);
        //BUG :: THIS MAY CAUSE AN ERROR IF AN ALLY TARGET IS CAST TO A NORMAL CHR TARGET - MAY NOT CHECK FOR SAME TEAM
		tarArg = (TargetArgChr)inputHuman.selected.arActions [inputHuman.nSelectedAbility].arArgs[inputHuman.indexCurTarget];

        Arena.Get().view.subMouseClick.Subscribe(cbCancelTargetting);
        ViewInteractive.subGlobalMouseRightClick.Subscribe(cbCancelTargetting);

        ViewChr.subAllClick.Subscribe(cbSetTargetChr);
        ViewAction.subAllClick.Subscribe(cbSwitchAction);
        ViewBlockerButton.subAllClick.Subscribe(cbSwitchAction);
    }

	override public void OnLeave(){
        Arena.Get().view.subMouseClick.UnSubscribe(cbCancelTargetting);
        ViewInteractive.subGlobalMouseRightClick.UnSubscribe(cbCancelTargetting);

        ViewChr.subAllClick.UnSubscribe(cbSetTargetChr);
        ViewAction.subAllClick.UnSubscribe(cbSwitchAction);
        ViewBlockerButton.subAllClick.UnSubscribe(cbSwitchAction);
    }
		
	public StateTargetChr(InputHuman _inputHuman) : base(_inputHuman) {

	}
}
