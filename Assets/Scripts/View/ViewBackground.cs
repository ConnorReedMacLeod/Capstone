﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewBackground : ViewInteractive {


    public static Subject subAllBackgroundClick = new Subject(Subject.SubType.ALL);

    public override void onMouseClick(params object[] args) {

        subAllBackgroundClick.NotifyObs();

        base.onMouseClick(args);

    }

}
