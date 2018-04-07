using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTimelineEventPassive : ViewTimelineEvent<TimelineEventPassive> {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override float GetVertSpan (){
		//TODO:: ACTUALLY IMPLEMENT THIS
		return 1.0f;
	}
}
