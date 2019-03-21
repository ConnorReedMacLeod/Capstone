using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO probably extend this class for visible/locked/duration interactions rather than using bool flags
public class Soul {

    public Chr chrSource;     //A reference to the character that applied this soul effect
    public Chr chrTarget;     //A reference to the character this soul effect is applied to

    public SoulContainer soulContainer; //A reference to the soulcontainer containing this soul

    public string sName;

    public bool bVisible;     //Is the effect visible in the soul? (can be interacted with)
    public bool bLocked;      //Should the effect not be able to be pushed out of the soul be new effects

    public int nMaxStacks;
    public int nCurStacks;

    public bool bRemoveOnChrDeath; //Should the ability be removed when the character its on dies?
    public bool bRemoveOnChrSourceDeath; //Should the ability be removed when the character who applied it dies?

    public bool bDuration; 
    public Property<int> pnMaxDuration;
    public int nCurDuration;

    public List<Replacement> lstReplacements = new List<Replacement>(); //A (potentially empty) list of replacement effects for this effect
    
    //A structure to hold information about a single trigger needed by a Soul effect
    public struct TriggerEffect {
        public Subject sub;
        public Subject.FnCallback cb;
    }

    public List<TriggerEffect> lstTriggers;

    public Soul(Chr _chrSource, Chr _chrTarget) {

        chrSource = _chrSource;
        chrTarget = _chrTarget;

        bRemoveOnChrSourceDeath = false;
        bRemoveOnChrDeath = true;

        nMaxStacks = 1; //by Default

    }

    //These are functions that we can set to nonnull values if we want something to happen on these triggers
    public System.Action funcOnApplication;
    public System.Action funcOnRemoval;
    public System.Action funcOnExpiration;//Specifically when the soul effect reaches the end of its duration
   
    public void OnApply(SoulContainer _soulContainer) {

        //Save a reference to the soulContainer we're in
        soulContainer = _soulContainer;

        //If we have a duration, then set the current duration to the max
        if(bDuration == true) {
            nCurDuration = pnMaxDuration.Get();
        }

        if (lstTriggers != null) { //Then we have some triggers to subscribe
            //Each triggeredeffect we have should subscribe to the trigger it needs
            foreach (TriggerEffect trig in lstTriggers) {
                //Debug.Log("*** ADDING TRIGGER SUBSCRIPTION ***");
                trig.sub.Subscribe(trig.cb);
            }
        }

        foreach (Replacement rep in lstReplacements) {
            //For each replacement effect this soul effect has, register it so it'll take effect
            Replacement.Register(rep);
        }

        chrTarget.subDeath.Subscribe(cbOnChrTargetDeath);
        chrSource.subDeath.Subscribe(cbOnChrSourceDeath);
        
        if(funcOnApplication != null) funcOnApplication();

        Debug.Log(sName + " has been applied");
    }

    public void OnRemoval() {

        chrTarget.subDeath.UnSubscribe(cbOnChrTargetDeath);
        chrSource.subDeath.UnSubscribe(cbOnChrSourceDeath);

        if (lstTriggers != null) { //Then we have some triggers to unsubscribe
                                   //Each triggeredeffect should unsubscribe from each of its triggers its observing
            foreach (TriggerEffect trig in lstTriggers) {
                trig.sub.UnSubscribe(trig.cb);
            }
        }

        foreach (Replacement rep in lstReplacements) {
            //For each replacement effect this soul effect has, unregister it so it'll stop taking effect
            Replacement.Unregister(rep);
        }

        if (funcOnRemoval != null) funcOnRemoval();
        Debug.Log(sName + " has been removed");

        if (bDuration == true && nCurDuration == 0) {
            if (funcOnExpiration != null) funcOnExpiration();
            Debug.Log(sName + " has expired");
        }

    }

    public void cbOnChrTargetDeath(Object target, params object[] args) {
        if (bRemoveOnChrDeath) {
            //When the character this is on dies, then we can dispel this soul effect
            soulContainer.RemoveSoul(this);
        }
    }

    public void cbOnChrSourceDeath(Object target, params object[] args) {
        Debug.Log("Called cbOnChrSourceDeath");

        if (bRemoveOnChrSourceDeath) {
            //When the character who applied this dies, then we can dispel this soul effect
            soulContainer.RemoveSoul(this);
        }
    }

    public Soul(Soul soulToCopy) {
        chrSource = soulToCopy.chrSource;
        chrTarget = soulToCopy.chrTarget;

        soulContainer = soulToCopy.soulContainer;
        sName = soulToCopy.sName;
        bVisible = soulToCopy.bVisible;
        bLocked = soulToCopy.bLocked;
        bRemoveOnChrDeath = soulToCopy.bRemoveOnChrDeath;
        bRemoveOnChrSourceDeath = soulToCopy.bRemoveOnChrSourceDeath;

        nMaxStacks = soulToCopy.nMaxStacks;
        nCurStacks = soulToCopy.nCurStacks;

        bDuration = soulToCopy.bDuration;
        nCurDuration = soulToCopy.nCurDuration;

        if (soulToCopy.pnMaxDuration != null) {
            pnMaxDuration = new Property<int>(soulToCopy.pnMaxDuration);
        }

        if (soulToCopy.lstReplacements != null) {
            lstReplacements = new List<Replacement>(soulToCopy.lstReplacements);
        }
        if (soulToCopy.lstTriggers != null) {
            lstTriggers = new List<TriggerEffect>(soulToCopy.lstTriggers);
        }

        //These might cause errors if the soul's function's change later, and we don't want our previously
        // started channel to change and reflect them
        funcOnApplication = soulToCopy.funcOnApplication;
        funcOnExpiration = soulToCopy.funcOnExpiration;
        funcOnRemoval = soulToCopy.funcOnRemoval;

    }
}
