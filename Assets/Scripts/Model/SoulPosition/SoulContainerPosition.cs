using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Adds in a reference to the Position owning the SoulContainer and implements a few methods that depend on the owner
public class SoulContainerPosition : SoulContainer {

    public Position posOwner;


    public override string GetOwnerName() {
        return posOwner.ToString();
    }

    public override void InitMaxVisibleSoul() {
        nMaxVisibleSoul = 1;
    }

    public override void LetOwnerNotifySoulApplied(Soul soulApplied) {
        posOwner.subSoulApplied.NotifyObs(this, soulApplied);
    }

    public override void LetOwnerNotifySoulRemoved(Soul soulRemoved) {
        posOwner.subSoulRemoved.NotifyObs(this, soulRemoved);
    }
}
