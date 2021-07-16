using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO probably extend this class for visible/locked/duration interactions rather than using bool flags
public abstract class Soul {

    public Chr chrSource;     //A reference to the character that applied this soul effect

    public Skill skillSource;  //A reference to the Skill that applied this source

    public SoulContainer soulContainer;

    public string sName;

    public bool bVisible;     //Is the effect visible in the soul? (can be interacted with)
    public bool bLocked;      //Should the effect not be able to be pushed out of the soul be new effects
    public bool bRecoilWhenApplied;     //Should the effect recoil the target when applied

    public int nMaxStacks;
    public int nCurStacks;

    public bool bRemoved;   //Has the effect been removed already from the character it was applied to

    public bool bRemoveOnChrSourceDeath; //Should the effect be removed when the character who applied it dies?

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

    public Soul(Chr _chrSource, Skill _skillSource) {

        chrSource = _chrSource;
        skillSource = _skillSource;

        bRemoveOnChrSourceDeath = false;

        bRecoilWhenApplied = true;

        nMaxStacks = 1; //by Default

        InitSubMaxDuration();

        //Intiialize triggers (whether or not there are any depends on the extending class)
        InitTriggers();

    }

    public virtual void InitSubMaxDuration() {
        //By default, we don't need to do anything.  If a derived class ends up needing fixed
        //  listeners for this property changing on creation, then we can initialize them here
    }

    public virtual void InitTriggers() {
        //By default, we don't need to do anything, but if we have triggers for this skill, then we
        // can initialize them here so that they get properly set up regardless of what constructor we use

    }

    public abstract string GetNameOfAppliedTo();

    //These are functions that we can override if we want certain effects to happen on application/removal/expiration
    //  By default, these just do nothing though
    public virtual void ApplicationEffect() { }
    public virtual void RemoveEffect() { }    //When this soul effect is removed for any reason
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

        chrSource.subDeath.Subscribe(cbOnChrSourceDeath);

        ApplicationEffect();

        if(ContSkillEngine.bDEBUGENGINE) Debug.Log(sName + " has been applied");
    }

    public void OnRemoval() {
        Debug.Log("Removing soul effect " + sName + " from " + GetNameOfAppliedTo());

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

        bRemoved = true;

        if(ContSkillEngine.bDEBUGENGINE) Debug.Log(sName + " has been removed");

        if(ShouldTriggerExpiration()) {
            ExpirationEffect();
            if(ContSkillEngine.bDEBUGENGINE) Debug.Log(sName + " has expired");
        }

    }

    public void cbOnChrSourceDeath(Object target, params object[] args) {

        if(bRemoveOnChrSourceDeath) {
            //When the character who applied this dies, then we can dispel this soul effect
            soulContainer.RemoveSoul(this);
        }
    }

    public virtual bool ShouldTriggerExpiration() {
        //By default, the effect will only count as expiring if it was set to have a duration,
        //  and that duration reached 0
        return bDuration == true && nCurDuration == 0;
    }

    public Soul(Soul soulToCopy) {

        chrSource = soulToCopy.chrSource;
        skillSource = soulToCopy.skillSource;

        soulContainer = soulToCopy.soulContainer;
        sName = string.Copy(soulToCopy.sName);
        bVisible = soulToCopy.bVisible;
        bLocked = soulToCopy.bLocked;
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

        InitTriggers();
    }
}
