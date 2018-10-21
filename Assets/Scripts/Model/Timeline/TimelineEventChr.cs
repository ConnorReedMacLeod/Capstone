using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineEventChr : TimelineEvent {

	public Chr chrSubject;

    public new ViewTimelineEventChr view {
        get {
            return (ViewTimelineEventChr)GetView();
        }
        set {
            view = value;
        }
    }

    public Subject subChrChanged = new Subject();
    public static Subject subAllChrChanged = new Subject();

    public override ViewTimelineEvent GetView() {
        //TODO:: Consider if there's a way to do this without
        //       a unity library function call each time
        return GetComponent<ViewTimelineEventChr>();
    }

    public TimelineEventChr (){

		fDelay = 2.0f;

	}

	public void SetChr(Chr _chrSubject){
		chrSubject = _chrSubject;
        
        subChrChanged.NotifyObs(this);
        subAllChrChanged.NotifyObs(this);
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
