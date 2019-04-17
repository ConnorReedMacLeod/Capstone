using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour{

    //This is the same as the normal Singleton, but we don't destroy the object on reloading/changing the scene

    protected override void Awake() {
        base.Awake();

        DontDestroyOnLoad(gameObject);
    }
}
