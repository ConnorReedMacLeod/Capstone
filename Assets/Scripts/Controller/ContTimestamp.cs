﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks the order and details of all executables that
/// get pushed onto the execution stack
/// </summary>
public class ContTimestamp : Singleton<ContTimestamp> {

    private int nCurTimestamp;
    private Timestamp curTimestamp;

    public Timestamp GetCurrentTimestamp() {
        return curTimestamp;
    }

    public Timestamp ClaimTimestamp(Executable exec) {

        Timestamp newTimestamp = new Timestamp(nCurTimestamp, exec);

        nCurTimestamp++;

        return newTimestamp;
    }

    public override void Init() {
        nCurTimestamp = 0;
    }
}
