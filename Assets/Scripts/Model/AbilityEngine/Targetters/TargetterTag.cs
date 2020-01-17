using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetterTag<T> {

    public abstract List<T> ApplyTagFiltering (List<T> lstTargets);

}
