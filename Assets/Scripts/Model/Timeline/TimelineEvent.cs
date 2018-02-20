using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//base class for any event that's in the timeline
public class TimelineEvent : Subject { //I guess making it a subject works so that its view can update just on it

	public enum STATE{
		UNREADY, //Scheduled in the future, but not ready (no action selected)
		READY,   //Scheduled in the future, and is ready to execute
		CURRENT,   //The currently processed event
		FINISHED   //Already processed - just shown as history
	};

	public STATE state;

	public int nPlace;

	public Timeline.PRIORITY prior;
	public float fDelay;

	public void IncPlace(){
		nPlace++;
	}

	public void DecPlace(){
		nPlace--;
	}

	public void SetState(STATE _state){
		state = _state;

		NotifyObs ();
	}

	public TimelineEvent (Timeline.PRIORITY _prior = Timeline.PRIORITY.NONE){
		
		prior = _prior;
		fDelay = 4.0f;
		state = STATE.UNREADY;

	}

	public virtual void Evaluate(){

		//TODO:: add some monobehaviour to the timeline to enable Invoke
		//UPDATE:: I'm using the model to do this (maybe find something better?)
		Timeline.Get ().mod.Invoke ("NextTimelineEvent", fDelay);
	}

}
