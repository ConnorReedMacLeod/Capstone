using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LibRandom {

    public static T GetRandomElementOfList<T>(List<T> lst) {
        return lst[Random.Range(0, lst.Count)];
    }

}
