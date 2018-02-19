using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineEventChr : TimelineEvent {

	public Character chrSubject;

	// If no priority is given, assume neutral
	public TimelineEventChr (Character _chrSubject, Timeline.PRIORITY _prior = Timeline.PRIORITY.NONE) : base (_prior){

		chrSubject = _chrSubject;

		fDelay = 2.0f;

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
