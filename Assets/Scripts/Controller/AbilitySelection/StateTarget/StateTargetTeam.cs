using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used for targgeting a specific character
public class StateTargetTeam : StateTarget {

    TargetArgTeam tarArg;


    public void cbCancelTargetting(Object target, params object[] args) {
        ResetTargets();
        inputHuman.CancelTar();
    }

    public void cbClickChr(Object target, params object[] args) {
        if (tarArg.setTar(((ViewChr)target).mod.plyrOwner)) {
            Debug.Log("Target successfully set to Player " + ((ViewChr)target).mod.plyrOwner.id);

            //move to next target
            inputHuman.IncTar();

            inputHuman.SetTargetArgState();
        } else {
            Debug.Log("Player " + ((ViewChr)target).mod.plyrOwner.id + " is not a valid player target");
        }
    }

    public void cbSwitchAction(Object target, params object[] args) {
        // Reset any targetting we've done
        ResetTargets();

        inputHuman.selected.nUsingAction = ((ViewAction)target).id;

        // TODO:: Save the current targets if there are any, so that you can 
        // revert to those targets if you've failed targetting
        inputHuman.ResetTar();
        inputHuman.SetTargetArgState(); // Let the parent figure out what exact state we go to

    }

    override public void OnEnter() {
        //TODO:: ADD AN OVERLAY FOR SELECTING A PLAYER

        Debug.Assert(inputHuman.selected != null);
        tarArg = (TargetArgTeam)inputHuman.selected.arActions[inputHuman.selected.nUsingAction].arArgs[inputHuman.nTarCount];

        Arena.Get().view.subMouseClick.Subscribe(cbCancelTargetting);
        ViewInteractive.subGlobalMouseRightClick.Subscribe(cbCancelTargetting);

        ViewChr.subAllClick.Subscribe(cbClickChr);
        ViewAction.subAllClick.Subscribe(cbSwitchAction);

    }

    override public void OnLeave() {
        //TODO:: REMOVE THE OVERLAY FOR SELECTING A PLAYER
        Arena.Get().view.subMouseClick.UnSubscribe(cbCancelTargetting);
        ViewInteractive.subGlobalMouseRightClick.UnSubscribe(cbCancelTargetting);

        ViewChr.subAllClick.UnSubscribe(cbClickChr);
        ViewAction.subAllClick.UnSubscribe(cbSwitchAction);
    }

    public void ResetTargets() {
        //clear any targetting 
        //TODO:: maybe only reset the targets to whatever was selected before?
        inputHuman.selected.arActions[inputHuman.selected.nUsingAction].ResetTargettingArgs();

        //contTarg.SetState(new StateTargetIdle(contTarg));
    }


    public StateTargetTeam(InputHuman _inputHuman) : base(_inputHuman) {
        
    }
}
