using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used for targgeting a specific character
public class StateTargetChr : StateTarget {

	TargetArgChr tarArg;


    public static Subject subAllStartSelection = new Subject();
    public static Subject subAllFinishSelection = new Subject();

    public void cbCancelTargetting(object target, params object [] args) {
        ContLocalInputSelection.Get().CancelTar();
    }

    public void cbSetTargetChr(object target, params object [] args) {

        int idTarget = ((ViewChr)target).mod.GetTargettingId();

        if (tarArg.WouldBeLegal(idTarget)) {

            //move to next target
            ContLocalInputSelection.Get().StoreTargettingIndex(idTarget);

            //Debug.Log("Target successfully set to " + ((ViewChr)target).mod.sName + " with id " + idTarget + " for player " + inputHuman.plyrOwner.id);

        } else {
            Debug.Log(((ViewChr)target).mod.sName + ", on team " + ((ViewChr)target).mod.plyrOwner.id + " is not a valid character target");
        }
    }

    public void cbSwitchAction(Object target, params object [] args) {
        Debug.Log("clicking on an action while asked to target a char");

        ContLocalInputSelection.Get().ResetTar();

        ContLocalInputSelection.Get().nSelectedAbility = ((ViewAction)target).mod.id;

        Debug.Log("reselected action is now " + ContLocalInputSelection.Get().nSelectedAbility);
        ContLocalInputSelection.Get().SetTargetArgState(); // Let the parent figure out what exact state we go to

    }

	override public void OnEnter(){

		Debug.Assert(ContLocalInputSelection.Get().chrSelected != null);

        //Get the appropriate current targetting type for the currently selected action
		tarArg = (TargetArgChr)ContLocalInputSelection.Get().chrSelected.arActions [ContLocalInputSelection.Get().nSelectedAbility].arArgs[ContLocalInputSelection.Get().indexCurTarget];

        Arena.Get().view.subMouseClick.Subscribe(cbCancelTargetting);
        ViewInteractive.subGlobalMouseRightClick.Subscribe(cbCancelTargetting);

        ViewChr.subAllClick.Subscribe(cbSetTargetChr);
        ViewAction.subAllClick.Subscribe(cbSwitchAction);
        ViewBlockerButton.subAllClick.Subscribe(cbSwitchAction);
        ViewRestButton.subAllClick.Subscribe(cbSwitchAction);

        subAllStartSelection.NotifyObs(null, tarArg);
    }

	override public void OnLeave(){
        Arena.Get().view.subMouseClick.UnSubscribe(cbCancelTargetting);
        ViewInteractive.subGlobalMouseRightClick.UnSubscribe(cbCancelTargetting);

        ViewChr.subAllClick.UnSubscribe(cbSetTargetChr);
        ViewAction.subAllClick.UnSubscribe(cbSwitchAction);
        ViewBlockerButton.subAllClick.UnSubscribe(cbSwitchAction);
        ViewRestButton.subAllClick.UnSubscribe(cbSwitchAction);

        subAllFinishSelection.NotifyObs(null, tarArg);
    }

}
