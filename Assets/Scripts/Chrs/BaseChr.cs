using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseChr {

    public Chr chrOwner;

    public BaseChr(Chr _chrOwner) {
        chrOwner = _chrOwner;
    }

    public virtual void SetName() {
        chrOwner.sName = "Default";
    }

    //Can Override this if you want to change base health for a particular character
    public virtual void SetMaxHealth() {
        chrOwner.pnMaxHealth.SetBase(100);
        chrOwner.nCurHealth = chrOwner.pnMaxHealth.Get();
    }

    public virtual void SetActions() {
        chrOwner.arActions[0] = new ActionRest(chrOwner);
        chrOwner.arActions[1] = new ActionRest(chrOwner);
        chrOwner.arActions[2] = new ActionRest(chrOwner);
        chrOwner.arActions[3] = new ActionRest(chrOwner);

        chrOwner.arActions[7] = new ActionRest(chrOwner);
    }
}
