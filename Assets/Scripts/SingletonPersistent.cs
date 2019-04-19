using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonPersistent<T> : Singleton<T> where T : MonoBehaviour{

    //This is the same as the normal Singleton, but we don't destroy the object on reloading/changing the scene

    protected override void Awake() {
        base.Awake();

        DontDestroyOnLoad(gameObject);
    }

    public override void ResetSingleton() {
        //Don't do anything - Since we are persistent, we shouldn't (by default) do any resetting between scenes
    }
}
