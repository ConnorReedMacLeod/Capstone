using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Executable {

    public Chr chrOwner;
    public string sLabel;
    public float fDelay;

    //public abstract Subject GetPreTrigger();
    //public abstract Subject GetPostTrigger();

    public virtual void Execute() {

        //Maybe send notifications that an executable has happened?

        //Now that we've done our thing, let the engine know to start processing the next thing
        ContAbilityEngine.Get().InvokeProcessStack(fDelay, sLabel);

        
    }

}
