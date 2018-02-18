using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : Element {

	virtual public void UpdateObs(){
		UpdateObs ("");
	}

	virtual public void UpdateObs(string updateType){
		//TODO:: Consider actually implementing string messages to let the observer know what it should update
	}
}
