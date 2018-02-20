using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTimeline : Observer {

	public Timeline mod;

	public LinkedList<ViewTimelineEvent> listViewEvents;

	public GameObject objEventHolder;

	public GameObject pfViewTimelineEventWide;
	public GameObject pfViewTimelineEventLarge;
	public GameObject pfViewTimelineEventSmall;


	public static float fEventGap;

	public void SetModel(Timeline _mod){
		mod = _mod;
		mod.Subscribe (this);
	}

	//Figure out where this new View Event should go in the list
	public void InsertEvent(ViewTimelineEvent viewTimelineEvent){
		int nPlace = viewTimelineEvent.GetPlace ();
		LinkedListNode<ViewTimelineEvent> curNode = listViewEvents.First;

		if (curNode == null) {
			//Then this is the first one, so just add it
			listViewEvents.AddFirst(viewTimelineEvent);
			listViewEvents.First.Value.SetPos(Vector3.zero);
			return;
		}

		while (nPlace != 0 && curNode.Next != null) {
			curNode = curNode.Next;
			nPlace--;
		}

		if (curNode.Next == null) {
			//Then we can't go any further - add to the end
			listViewEvents.AddLast (viewTimelineEvent);
		} else {
			//Place it in the correct index position
			listViewEvents.AddBefore (curNode, viewTimelineEvent);
		}

		//Shift down everything after the newly added item
		curNode = curNode.Previous; // Go back to the newly added node
		while (curNode != null) {

			Vector3 v3PrevPos = Vector3.zero;

			//If there's a previous node, place this node after it
			if (curNode.Previous != null) {
				v3PrevPos = curNode.Previous.Value.GetPosAfter ();
			}


			curNode.Value.SetPos(v3PrevPos);

			curNode = curNode.Next;
		}
	}

	//This could combine a few things into a helper method, but since slightly different types need to be
	//used for each event type, I've just overloaded this once for each type
	public void NewEvent(TimelineEventChr timelineEventChr){
		//Give it a Large event icon
		GameObject newObjEvent = Instantiate (pfViewTimelineEventLarge, objEventHolder.transform);
		ViewTimelineEventChr newViewEventChr = newObjEvent.AddComponent<ViewTimelineEventChr> () as ViewTimelineEventChr;

		newViewEventChr.SetModel (timelineEventChr);

		//TODO:: Let the view event know about any prefabs it'll need
		//newViewChr.pfActionWheel = pfActionWheel;

		//Add it to the list of Event views
		InsertEvent(newViewEventChr);

		//Have the new view make sure it's reflecting the model
		newViewEventChr.UpdateObs ();
	}

	//For adding a new Turn event view
	public void NewEvent(TimelineEventTurn timelineEventTurn){
		//Give a wide event icon
		GameObject newObjEvent = Instantiate (pfViewTimelineEventWide, objEventHolder.transform);
		ViewTimelineEventTurn newViewEventTurn = newObjEvent.AddComponent<ViewTimelineEventTurn> () as ViewTimelineEventTurn;

		newViewEventTurn.SetModel (timelineEventTurn);

		//TODO:: Let the view event know about any prefabs it'll need
		//newViewChr.pfActionWheel = pfActionWheel;

		//Add it to the list of Event views
		InsertEvent(newViewEventTurn);

		//Have the new view make sure it's reflecting the model
		newViewEventTurn.UpdateObs ();
	}
		

	public override void UpdateObs(){

		//Depending on what was last added
		switch (mod.lastUpdate) {
		case Timeline.UPDATETYPE.NEWEVENT:
			switch (mod.eventLastAdded.prior) {
			case Timeline.PRIORITY.TURN:
				NewEvent ((TimelineEventTurn)mod.eventLastAdded);
				break;
			case Timeline.PRIORITY.HIGH:
			case Timeline.PRIORITY.NONE:
			case Timeline.PRIORITY.LOW:
				NewEvent ((TimelineEventChr)mod.eventLastAdded);
				break;
			case Timeline.PRIORITY.EOT:
			case Timeline.PRIORITY.BOT:
				//TODO:: ADD IN STATUS AND PASSIVE STUFF
				break;
			}
			break;
		case Timeline.UPDATETYPE.NONE:
			break;
		default:
			Debug.LogError ("UNRECOGNIZED TIMELINE UPDATE TYPE");
			break;
		}
		//Print ();
	}

	public void ScrollEventHolder(float _diff){
		Vector3 newPos = new Vector3 (objEventHolder.transform.position.x, 
			objEventHolder.transform.position.y + _diff, objEventHolder.transform.position.z);
		SetEventHolderPos (newPos);
	}
		
	public void SetEventHolderPos(Vector3 newPos){
		objEventHolder.transform.position = newPos;
	}

	//undoes the scaling of the parent
	public void UnscaleEventHolder(){
		objEventHolder.transform.localScale = new Vector3
			(objEventHolder.transform.localScale.x / objEventHolder.transform.parent.localScale.x,
				objEventHolder.transform.localScale.y / objEventHolder.transform.parent.localScale.y,
				objEventHolder.transform.localScale.z / objEventHolder.transform.parent.localScale.z);
	}

	public void Print(){
		LinkedListNode<ViewTimelineEvent> curNode = listViewEvents.First;
		Debug.Log ("Printing entire Timeline view");
		while (curNode != null) {
			curNode.Value.Print ();

			curNode = curNode.Next;
		}
		Debug.Log ("Finished printing");
	}

	//TODO:: Set up all of the very first events concurrently, rather than one at a time
	public void Start(){
		//Print ();
		//Know what you should be representing
		SetModel (Timeline.Get ());

		UnscaleEventHolder ();
	}

	public ViewTimeline(){

		listViewEvents = new LinkedList<ViewTimelineEvent> ();
		fEventGap = 0.2f;

	}
}
