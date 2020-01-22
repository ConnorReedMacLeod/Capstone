using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A soul effect that's configured to use as a channel's behaviour
/// </summary>



public class SoulChannel : Soul {

    public Action act; //Store a reference to the action we represent
    

    /// <summary>
    /// Creates a properly configured SoulChannel based on the given Action's Execute method
    /// </summary>
	public SoulChannel(Action _act):base(_act.chrSource, _act.chrSource, _act) {
        bVisible = false;
        bDuration = false;

        bRecoilWhenApplied = false;

        act = _act;

        

        sName = "Channel-" + act.sName;

        //By default, we will do the successful completion action
        funcOnRemoval = onSuccessfulCompletion;
    }

    public void onSuccessfulCompletion() {

        //Then give that action's stack of clauses to the Ability Engine to process
        ContAbilityEngine.PushClauses(act.lstClauses);

        //Then pay for the action (increase cooldown)
        act.PayCooldown();
    }

    public void OnInterruptedCompletion() {
        //Then just pay for the action (increase cooldown)
        act.PayCooldown();
    }

    public SoulChannel(SoulChannel soulToCopy, Action _act) : base(soulToCopy) {

        //We'll need to copy these field ourselves, since the base constructor doesn't have it
        act = _act;

    }
}
