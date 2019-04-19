using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Need to maintain a list of all currently active Singleton classes
//Singleton classes are registered when they are added as a component of an object in the current scene
//switching out of a scene will delete all objects within that scene
//(this might nullify some of the singleton static instances)


public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {

    private static T inst;
    private bool bStarted = false;

    public static T Get() {

        if (inst == null) {
            Debug.LogError("Error! Static instance not set!");
        }

        return inst;
    }

    protected virtual void Awake() {

        if (inst != null) {
            //If an static instance exists,
            // then panic!  Destroy ourselves
            Debug.LogError("Warning!  This singleton already exists (" + gameObject.name + "), so we shouldn't instantiate a new one");
            Destroy(gameObject);
            
        } else {
            inst = gameObject.GetComponent<T>();
        }


    }

    public virtual void ResetSingleton() {
        bStarted = false;
        Start();
    }

   /* public static void ResetAllSingletons() {

        //Call the reset method for each singleton so they can initialize anything they need
        for (int i = 0; i < lstAllSinglestons.Count; i++) {
            lstAllSinglestons[i].ResetSingleton();
        }

    }*/

    public abstract void Init();

    public virtual void Start() {

        if (bStarted == true) return;
        bStarted = true;

        //lstAllSinglestons.Add(this);

        Init();

    }


}
