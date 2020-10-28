using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used for targgeting a specific character
public class StateTargetChr : StateTarget {

	TargetArgChr tarArg;


    public static Subject subAllStartSelection = new Subject(Subject.SubType.ALL);
    public static Subject subAllFinishSelection = new Subject(Subject.SubType.ALL);

    public void cbCancelTargetting(object target, params object [] args) {
        ContLocalUIInteraction.Get().CancelTar();
    }

    public void cbTargetChr(object target, params object [] args) {

        //We clicked on a character, so let's make a SelectionInfo package for it
        SelectionSerializer.SelectionChr infoSelectionChr =
            new SelectionSerializer.SelectionChr(
                ContLocalUIInteraction.Get().chrSelected,
                ContLocalUIInteraction.Get().actSelected,
                ((ViewChr)target).mod);

        if (infoSelectionChr.CanActivate()) {

            ContLocalUIInteraction.Get().FinishTargetting(infoSelectionChr);

        } else {
            Debug.Log(((ViewChr)target).mod.sName + ", on team " + ((ViewChr)target).mod.plyrOwner.id + " is not a valid character target");
        }
    }

    public void cbSwitchAction(Object target, params object [] args) {
        Debug.Log("clicking on an action while asked to target a char");

        ContLocalUIInteraction.Get().ResetTar();

        ContLocalUIInteraction.Get().actSelected = ((ViewAction)target).mod;

        Debug.Log("reselected action is now " + ContLocalUIInteraction.Get().actSelected.sDisplayName);
        ContLocalUIInteraction.Get().SetTargetArgState(); // Let the parent figure out what exact state we go to

    }

	override public void OnEnter(){

		Debug.Assert(ContLocalInputSelection.Get().chrSelected != null);

        Debug.Log("chrSelected is " + ContLocalInputSelection.Get().chrSelected.sName);

        //Get the appropriate current targetting type for the currently selected action
        tarArg = (TargetArgChr)ContLocalInputSelection.Get().chrSelected.arActions [ContLocalInputSelection.Get().nSelectedAbility].arArgs[ContLocalInputSelection.Get().indexCurTarget];

        Arena.Get().view.subMouseClick.Subscribe(cbCancelTargetting);
        ViewInteractive.subGlobalMouseRightClick.Subscribe(cbCancelTargetting);

        ViewChr.subAllClick.Subscribe(cbTargetChr);
        ViewAction.subAllClick.Subscribe(cbSwitchAction);
        ViewBlockerButton.subAllClick.Subscribe(cbSwitchAction);
        ViewRestButton.subAllClick.Subscribe(cbSwitchAction);

        subAllStartSelection.NotifyObs(null, tarArg);
    }

	override public void OnLeave(){

        Arena.Get().view.subMouseClick.UnSubscribe(cbCancelTargetting);
        ViewInteractive.subGlobalMouseRightClick.UnSubscribe(cbCancelTargetting);

        ViewChr.subAllClick.UnSubscribe(cbTargetChr);
        ViewAction.subAllClick.UnSubscribe(cbSwitchAction);
        ViewBlockerButton.subAllClick.UnSubscribe(cbSwitchAction);
        ViewRestButton.subAllClick.UnSubscribe(cbSwitchAction);

        subAllFinishSelection.NotifyObs(null, tarArg);
    }

}
