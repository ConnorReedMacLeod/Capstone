using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:: Extend this to a Trigger class that will notify each of its observers one at a time,
//       can maybe have a starting method that flags each observer as not being notified yet,
//       then when we are asked to Notify the next observer, we scan through and find the first
//       observer that hasn't been triggered yet, and Notify them.

public class Subject{

	bool bStarted;

    public delegate void FnCallback(Object target, params object[] args);

    public List<FnCallback> lstCallbacks = new List<FnCallback>();

    public Subject(Subject subToCopy) {
        Start();
        lstCallbacks = new List<FnCallback>(subToCopy.lstCallbacks);
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
        Start();

        List<FnCallback> lstCopied = new List<FnCallback>(lstCallbacks);
		foreach (FnCallback callback in lstCopied) {
            //if (callback == null)
            //	continue;//in case this object has been removed by the results of previous update iterations
            callback(target, args);
		}
	}

	public virtual void Start (){
        if (!bStarted) {
            bStarted = true;

        }
	}
}
