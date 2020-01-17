using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTargetter <T> {

    public abstract List<T> GetBaseTargets();
}
