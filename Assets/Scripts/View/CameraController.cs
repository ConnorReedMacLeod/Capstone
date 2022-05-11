using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Manages the focus of the camera (provides some pre-set
//   locations for where the camera can focus on depending on what 
//   needs to be selected)
public class CameraController : MonoBehaviour {

    //Configuration constants
    public float fTotalRelocationTime;
    public float fIntroRatio;
    public float fOutroRatio;

    public float fMinSpeed = 0.01f;
    private float fMaxSpeed;
    private float fCurSpeed;
    

    private float fRelocationProgress; //As a (0,1)percentage of the way through the relocation

    public bool bHaveTarget; //True if we are in the process of moving toward a new target
    public Vector3 v3TargetPos;
    public Vector3 v3StartPos;
    public float fDistToTravel;


    public void MoveTowardTarget() {

        if(bHaveTarget == false) {
            //If we don't have a position to move to, then we can just exit immediately
            return;
        }

        //Otherwise, we do have a position that we have to move to
        if(fRelocationProgress < fIntroRatio) {
            //Then we're still trying to speed up to the max speed



        }else if(fRelocationProgress < (1 - fOutroRatio)) {
            //Then we're moving at max speed



        } else {

            //Need to interpolate 


            if(fRelocationProgress >= 1f) {
                //If we've reached the target, then we can snap directly to the target position and clear the target position
                this.gameObject.transform.position = v3TargetPos;

                bHaveTarget = false;
                return;
            }
        }

        //At this point, we've updated our progress, so lerp between our starting point and our target
        this.gameObject.transform.position = Vector3.Lerp(v3StartPos, v3TargetPos, fRelocationProgress);

    }





     
    public void SetTarget(Vector3 _v3TargetPos) {

        v3TargetPos = _v3TargetPos;

        float fOldDistToTravel = fDistToTravel;
        fDistToTravel = (v3TargetPos - transform.position).magnitude;

        if (bHaveTarget) {
            //if we already have a target we're moving toward a target, then we'll have to do some adjustments

            //Scale our starting speed based on the relative distances between our old distance to travel and our new one
            fCurSpeed *= (fOldDistToTravel / fDistToTravel);
        }


    }



    // Update is called once per frame
    void Update() {
        MoveTowardTarget();
    }
}
