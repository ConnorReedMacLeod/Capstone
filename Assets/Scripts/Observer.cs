using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOTE :: THIS IS UNNEEDED NOW - JUST DIRECTLY PASS THE CALLBACK FUNCTION YOU WANT

public class Observer : MonoBehaviour {

    

	virtual public void UpdateObs (Subject.FnCallback fnCallback, Object target, params object[] args){
		// fnCallback is the method to be called in the observer when the event occurs
		// target represents the focus of the event 
		//		(if a controller this could be anything, but a view update will just leave this null)
		// args holds any additional information that may be needed (treat on a switch case-by-case basis)


	}
}
