﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used for targgeting a specific character
public class StateTargetTeam : StateTarget {

    TargetArgTeam tarArg;


    public void cbCancelTargetting(Object target, params object[] args) {
        inputHuman.CancelTar();
    }

    public void cbClickChr(Object target, params object[] args) {

        int idTarget = ((ViewChr)target).mod.plyrOwner.GetTargettingId();

        if (tarArg.WouldBeLegal(idTarget)) {

            //move to next target
            inputHuman.StoreTargettingIndex(idTarget);

            Debug.Log("Target successfully set to Player " + ((ViewChr)target).mod.plyrOwner.id);

        } else {
            Debug.Log("Player " + ((ViewChr)target).mod.plyrOwner.id + " is not a valid player target");
        }
    }

    public void cbSwitchAction(Object target, params object[] args) {

        inputHuman.nSelectedAbility = ((ViewAction)target).id;

        // TODO:: Save the current targets if there are any, so that you can 
        // revert to those targets if you've failed targetting
        inputHuman.ResetTar();
        inputHuman.SetTargetArgState(); // Let the parent figure out what exact state we go to

    }

    override public void OnEnter() {
        //TODO:: ADD AN OVERLAY FOR SELECTING A PLAYER

        Debug.Assert(inputHuman.selected != null);
        tarArg = (TargetArgTeam)inputHuman.selected.arActions[inputHuman.nSelectedAbility].arArgs[inputHuman.indexCurTarget];

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

    public StateTargetTeam(InputHuman _inputHuman) : base(_inputHuman) {
        
    }
}