using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A soul effect that's configured to use as a channel's behaviour
/// </summary>



public class SoulChannel : Soul {

    public bool bDelayedAction; // Is this soulChannel only used for executing a single effect after a delay
                                //  with no on-going effect

    public bool bChannelCompleted; // If we've successfully reached the end of the channel and should trigger its expiration effect

    /// <summary>
    /// Creates a properly configured SoulChannel that will call the Action's Execute method
    /// </summary>
    public SoulChannel(Action _actSource) : base(_actSource.chrSource, _actSource.chrSource, _actSource) {
        bVisible = false;
        bDuration = false;

        bRecoilWhenApplied = false;

        sName = "Channel-" + actSource.sName;

        bChannelCompleted = false;
    }

    public override bool ShouldTriggerExpiration() {

        //Check if our Channel Completed flag was been set by an ExecCompleteChannel clause
        return bChannelCompleted;

    }

    public override void ExpirationEffect() {

        //If we are purely a delayed action with no other side effects, then we can just call the
        //  execute method of our stored action
        if(bDelayedAction) {

            //If we reach the end of the duration of the effect, then execute the effects of the 
            // stored action and apply it to the stored target of the channel action
            actSource.Execute();
        }

        //If we're a custom SoulChannel effect, then if we want to do anything specific
        //  when completing our channel, then we should extend this method to do that effect
        //   baseline - we don't need to do anything
    }

    public void OnInterrupted() {

        Debug.Log("SoulChannel interrupted");

    }

    public SoulChannel(SoulChannel soulToCopy, Action _act) : base(soulToCopy) {

        Debug.Log("Creating copy of soulchannel");

        bDelayedAction = soulToCopy.bDelayedAction;
        bChannelCompleted = soulToCopy.bChannelCompleted;
    }
}
