using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:: Extend this to a Trigger class that will notify each of its observers one at a time,
//       can maybe have a starting method that flags each observer as not being notified yet,
//       then when we are asked to Notify the next observer, we scan through and find the first
//       observer that hasn't been triggered yet, and Notify them.

public class Subject{

    public enum SubType { ALL }; //A flag to pass to the constructor when initiallizing 
                                 // static subjects 

    //Keep a static list of all static subjects (so that we can reset them as needed)
    public static List<Subject> lstAllStaticSubjects;

    public delegate void FnCallback(Object target, params object[] args);

    public List<FnCallback> lstCallbacks = new List<FnCallback>();

    public Subject() {
        lstCallbacks = new List<FnCallback>();
    }

    public Subject(Subject subToCopy) {

        lstCallbacks = new List<FnCallback>(subToCopy.lstCallbacks);
    }

    //To be called only when creating static instances of Subjects
    public Subject(SubType subType) {

        if(lstAllStaticSubjects == null) {
            lstAllStaticSubjects = new List<Subject>();
        }

        //Note - in principle, since these are static, they should never be destroyed
        //       and thus should never need to be removed from this list
        lstAllStaticSubjects.Add(this);
        //Debug.Log("added to lstAllStaticSubjects " + this.ToString());

    }

    public static void ResetAllStaticSubjects() {

        //Reinitalize all of the static subjects so that they can be reinitialized properly
        // when restarting the game
        for(int i=0; i<lstAllStaticSubjects.Count; i++) {
            lstAllStaticSubjects[i] = new Subject();
        }

    }

	public void Subscribe(FnCallback fnCallback){

        lstCallbacks.Add (fnCallback);
	}

	public void UnSubscribe(FnCallback fnCallback) {

        lstCallbacks.Remove (fnCallback);
	}

	// Used for unspecific updates for views
	public virtual void NotifyObs(){
		NotifyObs (null);
	}

	public virtual void NotifyObs (Object target, params object[] args){

        List<FnCallback> lstCopied = new List<FnCallback>(lstCallbacks);
		foreach (FnCallback callback in lstCopied) {
            //if (callback == null)
            //	continue;//in case this object has been removed by the results of previous update iterations
            callback(target, args);
		}
	}
}
