using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:: Find a way to get a type-specific reference of an Event's view

//base class for any event that's in the timeline
// responsible for keeping track of its position relative
// to the position before it
public abstract class TimelineEvent : MonoBehaviour {

	public bool bStarted;

	public enum STATE{
		UNREADY, //Scheduled in the future, but not ready (no action selected)
		READY,   //Scheduled in the future, and is ready to execute
		CURRENT,   //The currently processed event
		FINISHED   //Already processed - just shown as history
	};

	public STATE state;

	public Timeline.PRIORITY prior;
	public float fDelay;

    public ViewTimelineEvent view {
        get {
            return GetView();
        }
        set {
            view = value;
        }
    }

    public LinkedListNode <TimelineEvent> nodeEvent;

    public Subject subEventMoved = new Subject();
    public static Subject subAllEventMoved = new Subject();
    public Subject subEventChangedState = new Subject();
    public static Subject subAllEventChangedState = new Subject();

    public virtual ViewTimelineEvent GetView() {
        //TODO:: Consider if there's a way to do this without
        //       a unity library function call each time
        return GetComponent<ViewTimelineEvent>();
    }

    public virtual void Start(){
		if (bStarted == false) {
			bStarted = true;

			
		}
	}

	public virtual void Init(LinkedListNode<TimelineEvent> _nodeEvent){
		nodeEvent = _nodeEvent;
        
        subEventMoved.NotifyObs(this);
        subAllEventMoved.NotifyObs(this);
    }

	public void SetState(STATE _state){
		state = _state;

        subEventChangedState.NotifyObs(this);
        subAllEventChangedState.NotifyObs(this);
    }

	public void SetPriority(Timeline.PRIORITY _prior = Timeline.PRIORITY.NONE){
		prior = _prior;
	}

	public TimelineEvent (){
		
		prior = Timeline.PRIORITY.NONE;
		fDelay = 4.0f;
		state = STATE.UNREADY;

	}

	public virtual void Evaluate(){

		// Set up the next event to go off later
		//Invoke ("NextTimelineEvent", fDelay);
	}

}
