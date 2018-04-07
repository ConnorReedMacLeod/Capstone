using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineEventChr : TimelineEvent {

	public Chr chrSubject;

	//TODO:: CHANGE THIS TO AN INIT METHOD
	// If no priority is given, assume neutral
	public TimelineEventChr (Chr _chrSubject, Timeline.PRIORITY _prior = Timeline.PRIORITY.NONE) : base (_prior){

		chrSubject = _chrSubject;

		fDelay = 2.0f;

	}

	public ViewTimelineEvent<TimelineEventChr> view;

	public override void InitView(){
		view = GetComponent<ViewTimelineEvent<TimelineEventChr>>();
		if (view == null){
			Debug.LogError ("ERROR! COUDLN't FIND A VIEWTIMELINEEVENTCHR COMPONENT");
		}
	}

	public override float GetVertSpan (){
		return view.GetVertSpan ();
	}
	public override Vector3 GetPosAfter (){
		return view.GetPosAfter ();
	}


	//TODO:: Lock out targetting for this ability while it's being executed
	public override void Evaluate(){

		//if the character hasn't prepared a valid action
		if (!chrSubject.ValidAction()) {
				
			chrSubject.SetRestAction ();
		
		}

		chrSubject.ExecuteAction ();

		base.Evaluate ();
	}


}
