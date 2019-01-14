using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A soul effect that's configured to use as a channel's behaviour
/// </summary>


public class SoulChannel : Soul {

    /// <summary>
    /// Creates a properly configured SoulChannel based on the given Action's Execute method
    /// </summary>
	public SoulChannel(Action act):base(act.chrSource, act.chrSource) {
        bVisible = false;
        bDuration = false;

        sName = "Channel-" + act.sName;

        //A channel will perform the action's execute method when it completes
        funcOnRemoval = act.Execute;
    }

    public SoulChannel(SoulChannel soulToCopy) : base(soulToCopy) {

    }
}
