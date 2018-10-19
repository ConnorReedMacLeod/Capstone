using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used for targgeting a specific character
public class StateTargetTeam : StateTarget {

    TargetArgTeam tarArg;


    public void cbCancelTargetting(Object target, params object[] args) {
        ResetTargets();
        contTarg.CancelTar();
    }

    public void cbClickChr(Object target, params object[] args) {
        if (tarArg.setTar(((ViewChr)target).mod.plyrOwner)) {
            Debug.Log("Target successfully set to Player " + ((ViewChr)target).mod.plyrOwner.id);

            //move to next target
            contTarg.IncTar();

            contTarg.SetTargetArgState();
        } else {
            Debug.Log("Player " + ((ViewChr)target).mod.plyrOwner.id + " is not a valid player target");
        }
    }

    public void cbSwitchAction(Object target, params object[] args) {
        // Reset any targetting we've done
        ResetTargets();

        contTarg.selected.nUsingAction = ((ViewAction)target).id;

        // TODO:: Save the current targets if there are any, so that you can 
        // revert to those targets if you've failed targetting
        contTarg.ResetTar();
        contTarg.SetTargetArgState(); // Let the parent figure out what exact state we go to

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
        Arena.Get().view.subMouseClick.Subscribe(cbCancelTargetting);
        ViewInteractive.subGlobalMouseRightClick.Subscribe(cbCancelTargetting);

        ViewChr.subAllClick.Subscribe(cbClickChr);
        ViewAction.subAllClick.Subscribe(cbSwitchAction);
    }
}
