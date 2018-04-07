using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO:: Make a static unscale method in some library class that can unscale anything
//        with respect to its parent

// Reponsible for keeping track of where the container for
// events should be and scrolling to line up the current event

public class ViewTimeline : Observer {

	public bool bStarted;

	public Timeline mod;

	public LinkedList<ViewTimelineEvent<TimelineEvent>> listViewEvents;

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
		mod.Subscribe (this);

	}

	public override void UpdateObs(string eventType, Object target, params object[] args){

		//Depending on what was last added
		switch (eventType) {
		case "FinishedEvent":
			ScrollEventHolder (((TimelineEvent)target).GetVertSpan ());
			break;

		/*case Timeline.UPDATETYPE.NEWEVENT:
			switch (mod.eventLastAdded.prior) {
			case Timeline.PRIORITY.TURN:
				NewEvent ((TimelineEventTurn)mod.eventLastAdded);
				break;
			case Timeline.PRIORITY.HIGH:
			case Timeline.PRIORITY.NONE:
			case Timeline.PRIORITY.LOW:
				NewEvent ((TimelineEventChr)mod.eventLastAdded);
				break;
			case Timeline.PRIORITY.EOT:
			case Timeline.PRIORITY.BOT:
				//TODO:: ADD IN STATUS AND PASSIVE STUFF
				break;
			}
			break*/;
		default:
			Debug.LogError ("UNRECOGNIZED TIMELINE UPDATE");
			break;
		}
		//Print ();
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
		if (bStarted == false) {
			bStarted = true;

			// Find our model
			InitModel ();
			InitEventContainer ();
			//UnscaleEventHolder ();
		}

	}

	public ViewTimeline(){

		listViewEvents = new LinkedList<ViewTimelineEvent<TimelineEvent>> ();
		fEventGap = 0.2f;

	}
}
