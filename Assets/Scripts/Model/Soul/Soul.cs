﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO probably extend this class for visible/locked/duration interactions rather than using bool flags
public abstract class Soul {

    public Chr chrSource;     //A reference to the character that applied this soul effect
    public Chr chrTarget;     //A reference to the character this soul effect is applied to

    public SoulContainer soulContainer; //A reference to the soulcontainer containing this soul

    public string sName;

    public bool bVisible;     //Is the effect visible in the soul? (can be interacted with)
    public bool bLocked;      //Should the effect not be able to be pushed out of the soul be new effects

    public int nMaxStacks;
    public int nCurStacks;

    public bool bDuration; 
    public Property<int> pnMaxDuration;
    public int nCurDuration;
    
    //A structure to hold information about a single trigger needed by a Soul effect
    public struct TriggerEffect {
        public Subject sub;
        public Subject.FnCallback cb;
    }

    public List<TriggerEffect> lstTriggers;

    public Soul(Chr _chrSource, Chr _chrTarget) {

        chrSource = _chrSource;
        chrTarget = _chrTarget;

        nMaxStacks = 1; //by Default

    }

    public virtual void funcOnApplication() { } //By default, don't do anything
    public virtual void funcOnRemoval() { } //Also do nothing
    public virtual void funcOnExpiration() { } //Specifically when the soul effect reaches the end of its duration
   
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
                //TODO:: Consider switching this to an extended trigger class rather than just a Subject
                trig.sub.Subscribe(trig.cb);
            }
        }

        funcOnApplication();

    }

    public void OnRemoval() {

        if (lstTriggers != null) { //Then we have some triggers to unsubscribe
                                   //Each triggeredeffect should unsubscribe from each of its triggers its observing
            foreach (TriggerEffect trig in lstTriggers) {
                trig.sub.UnSubscribe(trig.cb);
            }
        }

        
        funcOnRemoval();

        if(bDuration == true && nCurDuration == 0) {
            funcOnExpiration();
        }

    }


}
