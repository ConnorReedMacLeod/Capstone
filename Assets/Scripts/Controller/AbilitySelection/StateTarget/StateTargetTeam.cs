using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used for targgeting a specific character
public class StateTargetTeam : StateTarget {

    TargetArgTeam tarArg;

    public static Subject subAllStartSelection = new Subject();
    public static Subject subAllFinishSelection = new Subject();

    public void cbCancelTargetting(Object target, params object[] args) {
        ContLocalInputSelection.Get().CancelTar();
    }

    public void cbClickChr(Object target, params object[] args) {

        int idTarget = ((ViewChr)target).mod.plyrOwner.GetTargettingId();

        if (tarArg.WouldBeLegal(idTarget)) {

            //move to next target
            ContLocalInputSelection.Get().StoreTargettingIndex(idTarget);

            Debug.Log("Target successfully set to Player " + ((ViewChr)target).mod.plyrOwner.id);

        } else {
            Debug.Log("Player " + ((ViewChr)target).mod.plyrOwner.id + " is not a valid player target");
        }
    }

    public void cbSwitchAction(Object target, params object[] args) {

        ContLocalInputSelection.Get().nSelectedAbility = ((ViewAction)target).mod.id;

        // TODO:: Save the current targets if there are any, so that you can 
        // revert to those targets if you've failed targetting
        ContLocalInputSelection.Get().ResetTar();
        ContLocalInputSelection.Get().SetTargetArgState(); // Let the parent figure out what exact state we go to

    }

    override public void OnEnter() {
        //TODO:: ADD AN OVERLAY FOR SELECTING A PLAYER

        Debug.Assert(ContLocalInputSelection.Get().chrSelected != null);
        tarArg = (TargetArgTeam)ContLocalInputSelection.Get().chrSelected.arActions[ContLocalInputSelection.Get().nSelectedAbility].arArgs[ContLocalInputSelection.Get().indexCurTarget];

        Arena.Get().view.subMouseClick.Subscribe(cbCancelTargetting);
        ViewInteractive.subGlobalMouseRightClick.Subscribe(cbCancelTargetting);

        ViewChr.subAllClick.Subscribe(cbClickChr);
        ViewAction.subAllClick.Subscribe(cbSwitchAction);

        subAllStartSelection.NotifyObs(null, tarArg);

    }

    override public void OnLeave() {
        //TODO:: REMOVE THE OVERLAY FOR SELECTING A PLAYER
        Arena.Get().view.subMouseClick.UnSubscribe(cbCancelTargetting);
        ViewInteractive.subGlobalMouseRightClick.UnSubscribe(cbCancelTargetting);

        ViewChr.subAllClick.UnSubscribe(cbClickChr);
        ViewAction.subAllClick.UnSubscribe(cbSwitchAction);

        subAllFinishSelection.NotifyObs(null, tarArg);

    }

}
