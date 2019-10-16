using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContTime : Singleton<ContTime> {


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


    public override void Init() {
        //Nothing to initialize right now

    }

    public List<InvokeFunc> lstInvokes = new List<InvokeFunc>();
    public List<InvokeFunc> lstBufferToInvoke = new List<InvokeFunc>();


    public void Invoke(float _fDelay, System.Action _funcToCall) {
        lstBufferToInvoke.Add(new InvokeFunc(_fDelay, _funcToCall));
    }


    public void ProgressInvokes() {

        //Make a copy of the functions we need to add to our list of invoked function (to avoid issues of insertion when iterating through it)
        InvokeFunc[] arTemp = lstBufferToInvoke.ToArray();
        //Once our copies made, clear out the buffer so that we can accept new additions
        lstBufferToInvoke.Clear();

        //Add each function to the list we will actually tick down
        foreach(InvokeFunc inv in arTemp) {
            lstInvokes.Add(inv);
        }
        

        lstInvokes.ForEach(delegate (InvokeFunc inv) {
            //Tick down the remaining time
            inv.fDelay -= fDeltaTime;

            //If we've ticked down the total time for the function
            if(inv.fDelay <= 0f) {
                //Then call the associated function
                inv.funcToCall();
            }

        });

        //Remove each element that has met it's duration and called it's function
        lstInvokes.RemoveAll(delegate (InvokeFunc inv) {
            return inv.fDelay <= 0f;
        });

        

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
