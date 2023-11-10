using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Adds in a reference to the character owning the SoulContainer and implements a few methods that depend on the owner
public class SoulContainerChr : SoulContainer {

    public Chr chrOwner;


    public override string GetOwnerName() {
        return chrOwner.sName;
    }

    public override void InitMaxVisibleSoul() {
        nMaxVisibleSoul = 3;
    }

    public override void LetOwnerNotifySoulApplied(Soul soulApplied) {
        chrOwner.subSoulApplied.NotifyObs(this, soulApplied);
    }

    public override void LetOwnerNotifySoulRemoved(Soul soulRemoved) {
        chrOwner.subSoulRemoved.NotifyObs(this, soulRemoved);
    }

    public override void OnOverfillingSoul() {
        //If we overfill a Chr's soul, then we want to SoulBreak them


        chrOwner.soulContainer.ApplySoul(new SoulSoulBreak(chrOwner, chrOwner, null));

    }
}
