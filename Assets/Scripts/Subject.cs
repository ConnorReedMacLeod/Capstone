﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subject{

	bool bStarted;

    public delegate void FnCallback(Object target, params object[] args);

    public List<FnCallback> lstCallbacks = new List<FnCallback>();

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
