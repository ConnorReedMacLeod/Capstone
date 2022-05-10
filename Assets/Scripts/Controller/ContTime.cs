using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContTime : Singleton<ContTime> {


    public bool bPaused;

    public bool bAutoFastForward;
    public bool bManualFastForward;

    public bool bFastForward {
        get {
            return bAutoFastForward || bManualFastForward;
        }
    }
    public const float fFastForwardDelays = 0f; //Use no delay when fast forwarding

    public float fDeltaTime;

    public enum DELAYOPTIONS {
        FAST, MEDIUM, INF
    };

    //Get the time delay when accounting for fastforwarding
    static float GetRealTimeDelay(float fRawTime) {
        return Get().bFastForward ? fFastForwardDelays : fRawTime;
    }

    public static float fDelayChooseSkillFast {
        get {
            return GetRealTimeDelay(5.0f);
        }
    }
    public static float fDelayChooseSkillMedium{
        get {
            return GetRealTimeDelay(30.0f);
        }
    }
    public static float fDelayChooseSkillInf {
        get {
            return GetRealTimeDelay(9999999.0f);
        }
    }

    public float fMaxSelectionTime;

    public static float fDelayInstant {
        get {
            return GetRealTimeDelay(0.0f);
        }
    }

    public static float fDelayGameEffects {
        get {
            return GetRealTimeDelay(0.5f);
        }
    }
    public static float fDelayTurnSkill {
        get {
            return GetRealTimeDelay(0.5f);
        }
    }
    public static float fDelayMinorSkill {
        get {
            return GetRealTimeDelay(0.5f);
        }
    }
    public static float fDelayStandard {
        get {
            return GetRealTimeDelay(1.25f);
        }
    }
    public static float fDelayBan {
        get {
            return GetRealTimeDelay(20.0f);
        }
    }
    public static float fDelayDraftPick {
        get {
            return GetRealTimeDelay(20.0f);
        }
    }
    public static float fDelayLoadoutSetup {
        get {
            return GetRealTimeDelay(120.0f);
        }
    }


    public void SetMaxSelectionTime(DELAYOPTIONS delay) {
        switch (delay) {
            case DELAYOPTIONS.FAST:
                fMaxSelectionTime = fDelayChooseSkillFast;
                break;

            case DELAYOPTIONS.MEDIUM:
                fMaxSelectionTime = fDelayChooseSkillMedium;
                break;

            case DELAYOPTIONS.INF:
                fMaxSelectionTime = fDelayChooseSkillInf;
                break;
        }
    }



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

        //If there's no delay, then just call the function immediately
        if (_fDelay == 0f) {
            //Note - I'm a bit worried this might lead to really inflated call stacks that can't get cleared out
            _funcToCall();
        } else {
            //Set it up to call after a delay
            lstBufferToInvoke.Add(new InvokeFunc(_fDelay, _funcToCall));
        }

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
        if(bPaused) {
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

    public void SetAutoFastForward(bool _bAutoFastForward) {
        if(bAutoFastForward != _bAutoFastForward) {
            Debug.Log(LibDebug.AddColor(string.Format("Changing Auto Fast Forwarding to {0}", _bAutoFastForward), LibDebug.Col.MAGENTA));
        }
        bAutoFastForward = _bAutoFastForward;
    }

    public void SetManualFastForward(bool _bManualFastForward) {
        if (bManualFastForward != _bManualFastForward) {
            Debug.Log(LibDebug.AddColor(string.Format("Changing Auto Fast Forwarding to {0}", _bManualFastForward), LibDebug.Col.MAGENTA));
        }
        bManualFastForward = _bManualFastForward;
    }

    private void Update() {

        SetDeltaTime();

        ProgressInvokes();

    }

    public void PrintInvokeList() {

        Debug.Log("lstInvoke (length=" + lstInvokes.Count + ") contents:");
        lstInvokes.ForEach(delegate (InvokeFunc inv) {

            Debug.Log("key: " + inv.funcToCall + " time: " + inv.fDelay);

        });

    }



}
