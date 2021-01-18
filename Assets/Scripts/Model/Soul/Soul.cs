using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO probably extend this class for visible/locked/duration interactions rather than using bool flags
public class Soul {

    public Chr chrSource;     //A reference to the character that applied this soul effect
    public Chr chrTarget;     //A reference to the character this soul effect is applied to

    public Action actSource;  //A reference to the Action that applied this source

    public SoulContainer soulContainer; //A reference to the soulcontainer containing this soul

    public string sName;

    public bool bVisible;     //Is the effect visible in the soul? (can be interacted with)
    public bool bLocked;      //Should the effect not be able to be pushed out of the soul be new effects
    public bool bRecoilWhenApplied;     //Should the effect recoil the target when applied

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

    public Soul(Chr _chrSource, Chr _chrTarget, Action _actSource) {

        chrSource = _chrSource;
        chrTarget = _chrTarget;
        actSource = _actSource;

        bRemoveOnChrSourceDeath = false;
        bRemoveOnChrDeath = true;

        bRecoilWhenApplied = true;

        nMaxStacks = 1; //by Default

        InitSubMaxDuration();

    }

    public virtual void InitSubMaxDuration() {
        //By default, we don't need to do anything.  If a derived class ends up needing fixed
        //  listeners for this property changing on creation, then we can initialize them here
    }

    public virtual void InitTriggers() {
        //By default, we don't need to do anything, but if we have triggers for this ability, then we
        // can initialize them here so that they get properly set up regardless of what constructor we use

    }

    //These are functions that we can override if we want certain effects to happen on application/removal/expiration
    //  By default, these just do nothing though
    public virtual void ApplicationEffect() { }
    public virtual void RemoveEffect() { }
    public virtual void ExpirationEffect() { }//Specifically when the soul effect reaches the end of its duration

    public void OnApply(SoulContainer _soulContainer) {

        //Save a reference to the soulContainer we're in
        soulContainer = _soulContainer;

        //If we have a duration, then set the current duration to the max
        if(bDuration == true) {
            nCurDuration = pnMaxDuration.Get();
        }

        if(lstTriggers != null) { //Then we have some triggers to subscribe
            //Each triggeredeffect we have should subscribe to the trigger it needs
            foreach(TriggerEffect trig in lstTriggers) {
                //Debug.Log("*** ADDING TRIGGER SUBSCRIPTION ***");
                trig.sub.Subscribe(trig.cb);
            }
        }

        foreach(Replacement rep in lstReplacements) {
            //For each replacement effect this soul effect has, register it so it'll take effect
            Replacement.Register(rep);
        }

        chrTarget.subDeath.Subscribe(cbOnChrTargetDeath);
        chrSource.subDeath.Subscribe(cbOnChrSourceDeath);

        ApplicationEffect();

        if(ContAbilityEngine.bDEBUGENGINE) Debug.Log(sName + " has been applied");
    }

    public void OnRemoval() {

        chrTarget.subDeath.UnSubscribe(cbOnChrTargetDeath);
        chrSource.subDeath.UnSubscribe(cbOnChrSourceDeath);

        if(lstTriggers != null) { //Then we have some triggers to unsubscribe
                                  //Each triggeredeffect should unsubscribe from each of its triggers its observing
            foreach(TriggerEffect trig in lstTriggers) {
                trig.sub.UnSubscribe(trig.cb);
            }
        }

        foreach(Replacement rep in lstReplacements) {
            //For each replacement effect this soul effect has, unregister it so it'll stop taking effect
            Replacement.Unregister(rep);
        }

        RemoveEffect();
        if(ContAbilityEngine.bDEBUGENGINE) Debug.Log(sName + " has been removed");

        if(bDuration == true && nCurDuration == 0) {
            ExpirationEffect();
            if(ContAbilityEngine.bDEBUGENGINE) Debug.Log(sName + " has expired");
        }

    }

    public void cbOnChrTargetDeath(Object target, params object[] args) {
        if(bRemoveOnChrDeath) {
            //When the character this is on dies, then we can dispel this soul effect
            soulContainer.RemoveSoul(this);
        }
    }

    public void cbOnChrSourceDeath(Object target, params object[] args) {

        if(bRemoveOnChrSourceDeath) {
            //When the character who applied this dies, then we can dispel this soul effect
            soulContainer.RemoveSoul(this);
        }
    }

    public Soul(Soul soulToCopy, Chr _chrTarget = null) {

        chrSource = soulToCopy.chrSource;

        if(_chrTarget != null) {
            //If a Target was provided, then we'll use that
            chrTarget = _chrTarget;
        } else {
            //Otherwise, just copy from the other object
            chrTarget = soulToCopy.chrTarget;
        }

        soulContainer = soulToCopy.soulContainer;
        sName = string.Copy(soulToCopy.sName);
        bVisible = soulToCopy.bVisible;
        bLocked = soulToCopy.bLocked;
        bRemoveOnChrDeath = soulToCopy.bRemoveOnChrDeath;
        bRemoveOnChrSourceDeath = soulToCopy.bRemoveOnChrSourceDeath;

        nMaxStacks = soulToCopy.nMaxStacks;
        nCurStacks = soulToCopy.nCurStacks;

        bDuration = soulToCopy.bDuration;
        nCurDuration = soulToCopy.nCurDuration;


        if(soulToCopy.pnMaxDuration != null) {
            pnMaxDuration = new Property<int>(soulToCopy.pnMaxDuration);
            //Ensure any fixed subscribers to pnMaxDuration.subChanged are properly set up
            InitSubMaxDuration();
        }

        if(soulToCopy.lstReplacements != null) {
            lstReplacements = new List<Replacement>(soulToCopy.lstReplacements);
        }

        //If an extension of this class needs to copy triggers, then it'll be
        //  respondible for doing that properly


    }
}
