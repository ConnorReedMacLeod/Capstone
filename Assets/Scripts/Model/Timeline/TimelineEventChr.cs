using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineEventChr : TimelineEvent {

	public Chr chrSubject;

    public Subject subChrChanged = new Subject();
    public static Subject subAllChrChanged = new Subject();

	public TimelineEventChr (){

		fDelay = 2.0f;

	}

	public void SetChr(Chr _chrSubject){
		chrSubject = _chrSubject;
        
        subChrChanged.NotifyObs(this);
        subAllChrChanged.NotifyObs(this);
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
