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

	public LinkedListNode <TimelineEvent> nodeEvent;

	public abstract void InitView ();
	//Query the specific view's version of these methods
	public abstract float GetVertSpan ();
	public abstract Vector3 GetPosAfter ();

    public Subject subEventMoved;
    public static Subject subAllEventMoved;
    public Subject subEventChangedState;
    public static Subject subAllEventChangedState;

	public virtual void Start(){
		if (bStarted == false) {
			bStarted = true;

			InitView ();
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
