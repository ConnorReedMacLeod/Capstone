using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:: This class
public class TimelineEventPassive : TimelineEvent {

	public ViewTimelineEvent<TimelineEventPassive> view;

	public override void InitView(){
		view = GetComponent<ViewTimelineEvent<TimelineEventPassive>>();
		if (view == null){
			Debug.LogError ("ERROR! COUDLN't FIND A VIEWTIMELINEEVENTPASSIVE COMPONENT");
		}
			
		view.Start ();
	}

	public override float GetVertSpan (){
		return view.GetVertSpan ();
	}
	public override Vector3 GetPosAfter (){
		return view.GetPosAfter ();
	}

	public override void Evaluate(){

		//Actually do this

		Timeline.Get ().EventFinished ();
	}

}
