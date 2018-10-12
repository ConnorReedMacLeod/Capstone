using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used for targgeting a specific character
public class StateTargetTeam : StateTarget {

    TargetArgTeam tarArg;


    public override void UpdateObs(string eventType, Object target, params object[] args) {

        switch (eventType) {
            case Notification.ClickArena:
                ResetTargets();
                contTarg.CancelTar();

                break;

            case Notification.ChrStopHold:
            case Notification.ClickChr:
                if (tarArg.setTar(((ViewChr)target).mod.plyrOwner)) {
                    Debug.Log("Target successfully set to " + ((ViewChr)target).mod.plyrOwner.name);

                    //move to next target
                    contTarg.IncTar();

                    contTarg.SetTargetArgState();
                } else {
                    Debug.Log("Player " + ((ViewChr)target).mod.plyrOwner.id + " is not a valid player target");
                }
                break;
            case Notification.GlobalRightUp:
                ResetTargets();
                contTarg.CancelTar();

                break;

            case Notification.ClickAct:
                // Then we've clicked a new ability, so switch targetting to that

                // Reset any targetting we've done
                ResetTargets();

                contTarg.selected.nUsingAction = ((ViewAction)target).id;

                // TODO:: Save the current targets if there are any, so that you can 
                // revert to those targets if you've failed targetting
                contTarg.ResetTar();
                contTarg.SetTargetArgState(); // Let the parent figure out what exact state we go to

                break;
        }

    }

    override public void OnEnter() {
        //TODO:: ADD AN OVERLAY FOR SELECTING A PLAYER

        Debug.Assert(contTarg.selected != null);
        tarArg = (TargetArgTeam)contTarg.selected.arActions[contTarg.selected.nUsingAction].arArgs[contTarg.nTarCount];

    }

    override public void OnLeave() {
        //TODO:: REMOVE THE OVERLAY FOR SELECTING A PLAYER
    }

    public void ResetTargets() {
        //clear any targetting 
        //TODO:: maybe only reset the targets to whatever was selected before?
        contTarg.selected.arActions[contTarg.selected.nUsingAction].Reset();

        //contTarg.SetState(new StateTargetIdle(contTarg));
    }


    public StateTargetTeam(ContTarget _contTarg) : base(_contTarg) {

    }
}
