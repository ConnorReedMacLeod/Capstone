using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour{

    private static T inst;

	public static T Get() {

        if(inst == null) {
            Debug.LogError("Error! Static instance not set!");
        }

        return inst;
    }

    protected virtual void Awake() {

        if(inst != null){
            //If an static instance exists,
            // then panic!  Destroy ourselves
            Destroy(this);
            Debug.LogError("Warning!  This singleton already exists, so we shouldn't instantiate a new one");
        } else {
            inst = gameObject.GetComponent<T>();
        }

        
    }


}
