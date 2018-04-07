using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour {


	virtual public void UpdateObs (string eventType, Object target, params object[] args){
		// eventType signals the type of event - write a switch statement to handle cases
		// target represents the focus of the event 
		//		(if a controller this could be anything, but a view update will just leave this null)
		// args holds any additional information that may be needed (treat on a switch case-by-case basis)
	}
}
