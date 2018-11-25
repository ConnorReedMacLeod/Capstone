using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO probably extend this class for visible/locked/duration interactions rather than using bool flags
public class Soul {

    public string sName;

    public bool bVisible;     //Is the effect visible in the soul? (can be interacted with)
    public bool bLocked;      //Should the effect not be able to be pushed out of the soul be new effects

    public int nMaxStacks;
    public int nCurStacks;

    public bool bDuration; 
    public int nMaxDuration;
    public int nCurDuration;
    
    //A structure to hold information about a single trigger needed by a Soul effect
    public struct TriggerEffect {
        public Subject sub;
        public Subject.FnCallback cb;
    }

    public List<TriggerEffect> lstTriggers;

    public System.Action funcOnApplication;
    public System.Action funcOnRemoval;
    public System.Action funcOnExpiration; //Specifically when the soul effect reaches the end of its duration

    public void OnApply() {

        //If we have a duration, then set the current duration to the max
        if(bDuration == true) {
            nCurDuration = nMaxDuration;
        }

        if (lstTriggers != null) { //Then we have some triggers to subscribe
            //Each triggeredeffect we have should subscribe to the trigger it needs
            foreach (TriggerEffect trig in lstTriggers) {
                //TODO:: Consider switching this to an extended trigger class rather than just a Subject
                trig.sub.Subscribe(trig.cb);
            }
        }

        if (funcOnApplication != null) {
            funcOnApplication();
        }
    }

    public void OnRemoval() {

        if (lstTriggers != null) { //Then we have some triggers to unsubscribe
                                   //Each triggeredeffect should unsubscribe from each of its triggers its observing
            foreach (TriggerEffect trig in lstTriggers) {
                trig.sub.UnSubscribe(trig.cb);
            }
        }

        if (funcOnRemoval != null) {
            funcOnRemoval();
        }

        if(bDuration == true && funcOnExpiration != null) {
            funcOnExpiration();
        }

    }


}
