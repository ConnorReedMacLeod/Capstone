using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Keeps track of any global inputs that don't depend
// on context of where you're clicking
public class ContGlobalInput : Observer {



	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonUp (1)) {
			Controller.Get ().NotifyObs (Notification.GlobalRightUp, null);
			Debug.Log ("right");
		}
	}
}
