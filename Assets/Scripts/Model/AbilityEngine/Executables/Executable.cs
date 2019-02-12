using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Executable {

    public Chr chrSource;
    public Chr chrTarget;

    public string sLabel;
    public float fDelay;

    public bool bPreTriggered;

    

    public abstract Subject GetPreTrigger();
    public abstract Subject GetPostTrigger();
    public abstract List<Replacement> GetReplacements();
    public abstract List<Replacement> GetFullReplacements();

    public virtual bool isLegal() {
        if (chrSource.bDead) {
            Debug.Log("Executable of type  " + this.GetType().ToString() + " not legal since " + chrSource.sName + "(source) is dead");
            return false;
        }

        if (chrTarget.bDead) {
            Debug.Log("Executable of type  " + this.GetType().ToString() + " not legal since " + chrSource.sName + "(target) is dead");
            return false;
        }

        return true;
    }

    public void Execute() {
        if (isLegal() == false) {
            Debug.Log("Executable of type  " + this.GetType().ToString() + " has been cancelled since it's no longer legal");

            return;
        }

        //Put our post-trigger effects onto the stack so they're under the executable's effects
        GetPostTrigger().NotifyObs(null, this);

        //Put all of this executable's effects onto the stack
        ExecuteEffect();

        //Now that we've done our thing, let the engine know to start processing the next thing
        ContAbilityEngine.Get().InvokeProcessStack(fDelay, sLabel);
    }

    public abstract void ExecuteEffect();

}
