using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timestamp {

    public int nTimestamp;
    public Executable exec;

    public Timestamp(int _nTimestamp, Executable _exec) {
        nTimestamp = _nTimestamp;
        exec = _exec;
    }

    public override string ToString() {
        return string.Format("{0} ({1})", nTimestamp, exec);
    }
}
