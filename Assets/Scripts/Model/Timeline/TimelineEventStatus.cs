using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:: This class
public class TimelineEventStatus : TimelineEvent {

	public ViewTimelineEvent<TimelineEventStatus> view;

	public override void InitView(){
		view = GetComponent<ViewTimelineEvent<TimelineEventStatus>>();
		if (view == null){
			Debug.LogError ("ERROR! COUDLN't FIND A VIEWTIMELINEEVENTSTATUS COMPONENT");
		}
		Subscribe (view);
	}

	public override float GetVertSpan (){
		return view.GetVertSpan ();
	}
	public override Vector3 GetPosAfter (){
		return view.GetPosAfter ();
	}

}
