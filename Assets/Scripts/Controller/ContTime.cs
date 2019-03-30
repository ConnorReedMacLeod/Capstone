using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContTime : MonoBehaviour {

    private static ContTime inst;
    public static ContTime Get() {
        return inst;
    }


    public bool bPaused;
    public float fDeltaTime;


    public class InvokeFunc {
        public float fDelay;
        public System.Action funcToCall;

        public InvokeFunc(float _fDelay, System.Action _funcToCall) {
            fDelay = _fDelay;
            funcToCall = _funcToCall;
        }
    };


    public List<InvokeFunc> lstInvokes = new List<InvokeFunc>();


    public void Invoke(float _fDelay, System.Action _funcToCall) {
        Debug.Log("adding " + _funcToCall);
        lstInvokes.Add(new InvokeFunc(_fDelay, _funcToCall));

    }


    public void ProgressInvokes() {


        lstInvokes.ForEach(delegate (InvokeFunc inv) {
            //Tick down the remaining time
            inv.fDelay -= fDeltaTime;

            Debug.Log("Time is now " + inv.fDelay);

            //If we've ticked down the total time for the function
            if(inv.fDelay <= 0f) {
                Debug.Log("Calling the func ");
                //Then call the associated function
                inv.funcToCall();
            }

        });

        //Remove each element that has met it's duration and called it's function
        lstInvokes.RemoveAll(delegate (InvokeFunc inv) {
            return inv.fDelay <= 0f;
        });

    }


    private void Awake() {
        Debug.Log("waking");
        if (inst != null && inst != this) {
            Destroy(this.gameObject);
        } else {
            Debug.Log("Setting inst");
            inst = this;
        }
    }

    private void SetDeltaTime() {
        if (bPaused) {
            fDeltaTime = 0f;
        } else {
            fDeltaTime = Time.deltaTime;
        }
    }

    public void Pause() {
        bPaused = true;

        SetDeltaTime();

    }

    public void UnPause() {
        bPaused = false;

        SetDeltaTime();
    }


    private void Update() {

        SetDeltaTime();

        ProgressInvokes();

    }





}
