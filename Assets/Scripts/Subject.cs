using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subject {

	List<Observer> lstObservers;

	public void Subscribe(Observer newObs){
		lstObservers.Add (newObs);
	}

	public void UnSubscribe(Observer newObs){
		lstObservers.Remove (newObs);
	}

	public void NotifyObs(){
		List<Observer> copiedList = new List<Observer> (lstObservers);
		foreach (Observer obs in copiedList) {
			if (obs == null)
				continue;//in case this object has been removed by the results of previous update iterations
			obs.UpdateObs ();
		}
	}

	public Subject (){
		lstObservers = new List<Observer> ();
	}
}
