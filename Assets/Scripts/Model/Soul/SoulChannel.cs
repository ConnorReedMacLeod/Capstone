using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A soul effect that's configured to use as a channel's behaviour
/// </summary>



public class SoulChannel : SoulChr {

    public bool bDelayedSkill; // Is this soulChannel only used for executing a single effect after a delay
                               //  with no on-going effect

    public bool bChannelCompleted; // If we've successfully reached the end of the channel and should trigger its expiration effect

    /// <summary>
    /// Creates a properly configured SoulChannel that will call the Skill's Execute method
    /// </summary>
    public SoulChannel(Skill _skillSource) : base(_skillSource.chrOwner, _skillSource.chrOwner, _skillSource) {
        bVisible = false;
        bDuration = false;

        bRecoilWhenApplied = false;

        sName = "Channel-" + skillSource.sName;

        bChannelCompleted = false;
    }

    public override bool ShouldTriggerExpiration() {

        //Check if our Channel Completed flag was been set by an ExecCompleteChannel clause
        return bChannelCompleted;

    }

    public override void ExpirationEffect() {

        //If we are purely a delayed skill with no other side effects, then we can just call the
        //  execute method of our stored skill
        if(bDelayedSkill) {

            //If we reach the end of the duration of the effect, then execute the effects of the 
            // stored skill and apply it to the stored target of the channel skill
            skillSource.Execute();
        }

        //If we're a custom SoulChannel effect, then if we want to do anything specific
        //  when completing our channel, then we should extend this method to do that effect
        //   baseline - we don't need to do anything
    }

    public void OnInterrupted() {

        Debug.Log("SoulChannel " + sName + " interrupted");

    }

    public SoulChannel(SoulChannel soulToCopy, Skill _skill) : base(soulToCopy, soulToCopy.chrSource) {

        bDelayedSkill = soulToCopy.bDelayedSkill;
        bChannelCompleted = soulToCopy.bChannelCompleted;

    }


    //This is a nice way to have a SoulChannel reference be able to get its proper derived-type's copy constructor while
    // still returning as a base SoulChannel type.  Just override derived type's implementation of GetCopy to 
    public virtual SoulChannel GetCopy(Skill _skill) {
        return new SoulChannel(_skill);
    }
}
