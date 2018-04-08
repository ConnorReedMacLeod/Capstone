using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subject : MonoBehaviour {

	bool bStart;

	public List<Observer> lstObservers;

	public void Subscribe(Observer newObs){
		if (bStart == false) {
			Start ();
		}
		lstObservers.Add (newObs);
	}

	public void UnSubscribe(Observer newObs){
		if (bStart == false) {
			Start ();
		}
		lstObservers.Remove (newObs);
	}

	// Used for standard updates for views
	public void NotifyObs(){
		NotifyObs ("Default", null);
	}

	public void NotifyObs (string eventType, Object target, params object[] args){
		if (bStart == false) {
			Start ();
		}
		List<Observer> copiedList = new List<Observer> (lstObservers);
		foreach (Observer obs in copiedList) {
			if (obs == null)
				continue;//in case this object has been removed by the results of previous update iterations
			obs.UpdateObs (eventType, target, args);
		}
	}

	public virtual void Start (){
		if (bStart == false) {
			bStart = true;
			lstObservers = new List<Observer> ();
		}
	}
}
