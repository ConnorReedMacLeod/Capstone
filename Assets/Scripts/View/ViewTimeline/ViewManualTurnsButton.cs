﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManualTurnsButton : MonoBehaviour {

    public static Subject subAllManualExecuteEvent = new Subject();

	public void OnMouseDown(){
        subAllManualExecuteEvent.NotifyObs(this);
	}

}
