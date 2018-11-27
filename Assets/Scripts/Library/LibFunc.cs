using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LibFunc {

    public delegate T Get <T>  ();

    //Can be used to fix the return value, so that we always return
    //the value of toReturn AS IT IS NOW, even if the source of toReturn changes later
    public static Get<T> ReturnSnapShot<T>(T toReturn) {
        return () => { return toReturn; };
    }
	
}
