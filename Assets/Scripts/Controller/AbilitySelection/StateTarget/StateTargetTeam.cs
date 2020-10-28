using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used for targgeting a player/team (currently by just clicking on a character they own)
public class StateTargetTeam : StateTarget {

    TargetArgTeam tarArg;

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

        if (infoSelectionPlyr.CanActivate()) {

            ContLocalUIInteraction.Get().FinishTargetting(infoSelectionPlyr);

        } else {
            Debug.Log("Player " + ((ViewChr)target).mod.plyrOwner.id + " is not a valid player target");
        }
    }

    public void cbSwitchAction(Object target, params object[] args) {

        ContLocalUIInteraction.Get().nSelectedAbility = ((ViewAction)target).mod.id;

        // TODO:: Save the current targets if there are any, so that you can 
        // revert to those targets if you've failed targetting
        ContLocalUIInteraction.Get().ResetTar();
        ContLocalUIInteraction.Get().SetTargetArgState(); // Let the parent figure out what exact state we go to

    }

    override public void OnEnter() {
        //TODO:: ADD AN OVERLAY FOR SELECTING A PLAYER

        Debug.Assert(ContLocalUIInteraction.Get().chrSelected != null);
        tarArg = (TargetArgTeam)ContLocalUIInteraction.Get().chrSelected.arActions[ContLocalUIInteraction.Get().nSelectedAbility].arArgs[ContLocalUIInteraction.Get().iTargetIndex];

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
