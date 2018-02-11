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
		foreach (Observer obs in lstObservers) {
			obs.UpdateObs ();
		}
	}

	public Subject (){
		lstObservers = new List<Observer> ();
	}
}
