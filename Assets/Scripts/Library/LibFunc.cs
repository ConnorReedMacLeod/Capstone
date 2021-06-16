using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LibFunc {

    public delegate T Get<T>();
    public delegate T Combine<T>(T t1, T t2);

    //Can be used to fix the return value, so that we always return
    //the value of toReturn AS IT IS NOW, even if the source of toReturn changes later
    //This can be very useful when creating functions that use values that may change
    //after the function is created - e.g. you may want to snapshot the skill a character
    // is using RIGHT NOW, rather than always grabbing whatever the currently used skill is
    public static Get<T> ReturnSnapShot<T>(T toReturn) {
        return () => { return toReturn; };
    }


    public static T[] AddArray<T>(T[] ar1, T[] ar2, Combine<T> fCombine) {
        Debug.Assert(ar1.Length == ar2.Length);

        T[] arToReturn = new T[ar1.Length];
        for(int i = 0; i < ar1.Length; i++) {
            arToReturn[i] = fCombine(ar1[i], ar2[i]);
        }

        return arToReturn;
    }
}
