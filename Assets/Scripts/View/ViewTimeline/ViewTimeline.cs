﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reponsible for keeping track of where the container for
// events should be and scrolling to line up the current event

public class ViewTimeline : MonoBehaviour {

	public bool bStarted;

	public Timeline mod;

	public LinkedList<ViewTimelineEvent> listViewEvents;

	public Transform transEventContainer;

	public static float fEventGap;

	public void InitEventContainer(){
		GameObject go = GameObject.FindGameObjectWithTag ("EventContainer");
		if (go == null) {
			Debug.LogError ("ERROR! NO OBJECT HAS A EVENTCONTAINER TAG!");
		}
		transEventContainer = go.GetComponent<Transform> ();
		if (transEventContainer == null) {
			Debug.LogError ("ERROR! EVENTCONTAINER TAGGED OBJECT DOES NOT HAVE A TRANSFORM COMPONENT!");
		}
	}

	//Find the model, and do any setup for reflect it
	public void InitModel(){
		mod = GetComponent<Timeline>();

        mod.subEventFinished.Subscribe(cbEventFinished);
	}

    public void cbEventFinished(Object target, params object[] args) {
       
        ScrollEventHolder(((TimelineEvent)target).GetView().GetVertSpan());
    }

	public void ScrollEventHolder(float _diff){

        Vector3 newPos = new Vector3 (transEventContainer.position.x, 
			transEventContainer.position.y + _diff, transEventContainer.position.z);
		SetEventHolderPos (newPos);
	}
		
	public void SetEventHolderPos(Vector3 newPos){
		transEventContainer.position = newPos;
	}

	//undoes the scaling of the parent
	public void UnscaleEventHolder(){
		transEventContainer.localScale = new Vector3
			(transEventContainer.localScale.x / transEventContainer.parent.localScale.x,
				transEventContainer.localScale.y / transEventContainer.parent.localScale.y,
				transEventContainer.localScale.z / transEventContainer.parent.localScale.z);
	}
		
	public void Start(){
        Debug.Log("Starting and bStarted is " + bStarted);
		if (bStarted == false) {
			bStarted = true;

			// Find our model
			InitModel ();
			InitEventContainer ();
			//UnscaleEventHolder ();
		}

	}

	public ViewTimeline(){

		listViewEvents = new LinkedList<ViewTimelineEvent> ();
		fEventGap = 0.2f;
        bStarted = false;

	}
}
