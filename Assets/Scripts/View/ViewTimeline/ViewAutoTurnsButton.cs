﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewAutoTurnsButton : MonoBehaviour {

    public static Subject subAllAutoExecuteEvent = new Subject();

    public void OnMouseDown() {
        subAllAutoExecuteEvent.NotifyObs(this);
    }

}