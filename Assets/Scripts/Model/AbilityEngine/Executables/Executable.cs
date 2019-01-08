using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Executable {

    public Chr chrSource;
    public Chr chrTarget;

    public string sLabel;
    public float fDelay;

    public abstract Subject GetPreTrigger();
    public abstract Subject GetPostTrigger();
    public abstract List<Replacement> GetReplacements();
    public abstract List<Replacement> GetFullReplacements();

    public virtual void Execute() {

        //Let everyone know that this type of executable has finished being processed
        GetPostTrigger().NotifyObs(null, this);

        //Now that we've done our thing, let the engine know to start processing the next thing
        ContAbilityEngine.Get().InvokeProcessStack(fDelay, sLabel);

        
    }

}
