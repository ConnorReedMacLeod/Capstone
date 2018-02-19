using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//base class for any event that's in the timeline
public class TimelineEvent : Subject { //I guess making it a subject works so that its view can update just on it

	public Timeline.PRIORITY prior;
	public float fDelay;

	public TimelineEvent (Timeline.PRIORITY _prior = Timeline.PRIORITY.NONE){
		
		prior = _prior;
		fDelay = 4.0f;

	}

	public virtual void Evaluate(){

		//TODO:: add some monobehaviour to the timeline to enable Invoke
		//UPDATE:: I'm using the model to do this (maybe find something better?)
		Timeline.Get ().mod.Invoke ("NextTimelineEvent", fDelay);
	}

}
