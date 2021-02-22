using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used for targgeting a player/team (currently by just clicking on a character they own)
public class StateTargetTeam : StateTarget {

    public static Subject subAllStartSelection = new Subject(Subject.SubType.ALL);
    public static Subject subAllFinishSelection = new Subject(Subject.SubType.ALL);

    public void cbCancelTargetting(Object target, params object[] args) {
        ContLocalUIInteraction.Get().CancelTar();
    }

    public void cbClickChr(Object target, params object[] args) {

        //We clicked on a character, so let's make a SelectionInfo package for it
        SelectionSerializer.SelectionPlayer infoSelectionPlyr =
            new SelectionSerializer.SelectionPlayer(
                ContLocalUIInteraction.Get().chrSelected,
                ContLocalUIInteraction.Get().actSelected,
                ((ViewChr)target).mod.plyrOwner);

        if(infoSelectionPlyr.CanSelect()) {

            ContLocalUIInteraction.Get().FinishTargetting(infoSelectionPlyr);

        } else {
            Debug.Log("Player " + ((ViewChr)target).mod.plyrOwner.id + " is not a valid player target");
        }
    }

    public void cbSwitchAction(Object target, params object[] args) {

        Debug.Log("attempting to reselect" + ((ViewAction)target).mod.sDisplayName);

        ContLocalUIInteraction.Get().StartTargetting(((ViewAction)target).mod);

    }

    override public void OnEnter() {
        //TODO:: ADD AN OVERLAY FOR SELECTING A PLAYER


        Arena.Get().view.subMouseClick.Subscribe(cbCancelTargetting);
        ViewInteractive.subGlobalMouseRightClick.Subscribe(cbCancelTargetting);

        ViewChr.subAllClick.Subscribe(cbClickChr);
        ViewAction.subAllClick.Subscribe(cbSwitchAction);


        ContLocalUIInteraction.subAllStartManualTargetting.NotifyObs(this);
    }

    override public void OnLeave() {
        //TODO:: REMOVE THE OVERLAY FOR SELECTING A PLAYER


        Arena.Get().view.subMouseClick.UnSubscribe(cbCancelTargetting);
        ViewInteractive.subGlobalMouseRightClick.UnSubscribe(cbCancelTargetting);

        ViewChr.subAllClick.UnSubscribe(cbClickChr);
        ViewAction.subAllClick.UnSubscribe(cbSwitchAction);

    }

}
