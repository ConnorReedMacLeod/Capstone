using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


//Manages the focus of the camera (provides some pre-set
//   locations for where the camera can focus on)
public class CameraController : MonoBehaviour {

    //Configuration constants
    public float fTotalRelocationTime;
    public float fIntroRatio;
    public float fOutroRatio;

    [System.Serializable]
    public struct CameraLocation {
        public string sName;
        public Vector3 v3Location;
        public KeyCode keycodeHotkey;
    }
    //Provide a public-facing inspector-editable array of saved locations for the camera
    //  - will be implemented as a dictionary when running the game
    public CameraLocation[] arSavedCameraLocations;
    private Dictionary<string, Vector3> dictSavedCameraLocations;

    private float fMaxSpeed;
    private float fCurSpeed;


    private float fTimeProgress; //As a (0,1) percentage of the way through the relocation
    private float fRelocationProgress; //As a (0,1) percentage of how positionally far we are through the relocation

    private float fRelocationAtIntroEnd; //Save the progress (as a (0,1) percentage) we've made by the end of the intro segment

    private bool bHaveTarget; //True if we are in the process of moving toward a new target
    private Vector3 v3Target;
    private Vector3 v3Start;


    //Which saved location we're currently moving to
    public int iCurLocation;

    public void MoveTowardTarget() {

        if(bHaveTarget == false) {
            //If we don't have a position to move to, then we can just exit immediately
            return;
        }

        //Advance our time progress
        fTimeProgress += Time.fixedDeltaTime / fTotalRelocationTime;

        //Otherwise, we do have a position that we have to move to
        if(fTimeProgress < fIntroRatio) {
            //Then we're in the speed-up portion of our repositioning

            //Since we're constantly accelerating from a starting speed of 0 up until fMaxSpeed,
            // then we can lerp to find our current speed
            fCurSpeed = Mathf.Lerp(0f, fMaxSpeed, Mathf.InverseLerp(0f, fIntroRatio, fTimeProgress));

            //If we're moving at a constant acceleration, then our speed curve is a straight line going up to
            // fMaxSpeed over time fIntroRatio*fTotalRelocationTime
            // We can integrate the area under this curve to get our position at time t
            fRelocationProgress = 0.5f * fTimeProgress * fCurSpeed;

        } else if(fTimeProgress < (1 - fOutroRatio)) {
            //Then we're moving at max speed

            //Our progress will be our progress at intro-end plus the amount we'd move at maxspeed

            fRelocationProgress = fRelocationAtIntroEnd + fMaxSpeed * (fTimeProgress - fIntroRatio);

        } else {
            //Then we're trying to slow down from max speed

            if(fTimeProgress >= 1f) {
                //If we've reached the target, then we can snap directly to the target position and clear the target position
                this.gameObject.transform.position = v3Target;

                bHaveTarget = false;
                return;
            }


            //First, interpolate how far along our slow-down segment we are
            fCurSpeed = Mathf.Lerp(fMaxSpeed, 0f, Mathf.InverseLerp(1 - fOutroRatio, 1, fTimeProgress));

            //We can determine the area under our speed curve to find how close we are to the end
            fRelocationProgress = 1 - 0.5f * fCurSpeed * (1 - fTimeProgress);

        }

        //At this point, we've updated our current progress so lerp between our starting point and our target
        this.gameObject.transform.position = Vector3.Lerp(v3Start, v3Target, fRelocationProgress);

    }

    public void SetTarget(Vector3 _v3Target) {

        bHaveTarget = true;

        v3Target = _v3Target;
        v3Start = this.transform.position;

        fCurSpeed = 0f;
        fTimeProgress = 0f;
        fRelocationProgress = 0f;

    }


    // Update is called once per frame
    void Update() {
        MoveTowardTarget();
    }

    public void SetTargetLocation(string sLocationName) {

        if(dictSavedCameraLocations.ContainsKey(sLocationName) == false) {
            Debug.LogErrorFormat("No saved Camera Location for {0}", sLocationName);
            return;
        }

        SetTarget(dictSavedCameraLocations[sLocationName]);

    }

    public void cbCycleToNextLocation(Object tar, params object[] args) {
        iCurLocation = (iCurLocation + 1) % dictSavedCameraLocations.Count;

        SetTargetLocation(arSavedCameraLocations[iCurLocation].sName);
    }

    public void Start() {

        //Take the inspector-defined list of pre-defined camera locations and turn them into a dictionary to look-up with
        dictSavedCameraLocations = new Dictionary<string, Vector3>(arSavedCameraLocations.Length);

        foreach(CameraLocation loc in arSavedCameraLocations) {
            dictSavedCameraLocations.Add(loc.sName, loc.v3Location);

            //TODONOW - fix this
            Subject.FnCallback cbHotKey = null;

            //Set up the keybinding to move to that location
            KeyBindings.SetBinding(cbHotKey, loc.keycodeHotkey);
        }

        KeyBindings.SetBinding(cbCycleToNextLocation, KeyCode.L);


        //Initialize our speed constants since these are all distance-independant and work as a percentage
        // of the distance we're trying to move
        //These were calculated by finding the area under the expected speed curve:
        //  ______
        // /      \
        fMaxSpeed = 2 / ((2 - fIntroRatio - fOutroRatio));


        //We'll also figure out the distance we'll have moved at the end of the intro and save it
        // so that we can quickly add it in to our progress for our max-speed segment
        fRelocationAtIntroEnd = 0.5f * fMaxSpeed * fIntroRatio;
    }
}
