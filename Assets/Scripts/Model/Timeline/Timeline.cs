using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timeline : Subject {

	private static Timeline instance;

	public static int MAXTURNS = 15;

	public enum PRIORITY {
		BOT, //beginning of turn
		HIGH, //before normal actions
		NONE, //normal actions
		LOW, //after normal actions
		EOT, //end of turn
		TURN //marks a new turn
	};

	public Model mod;

	public LinkedList <TimelineEvent> listEvents;

	public LinkedListNode <TimelineEvent> curEvent;

	//This is so the view can know which event to update
	//TODO:: Make this a class that wraps everything up nicely
	public enum UPDATETYPE
	{
		NONE, //Nothing just happend, I swear
		NEWEVENT //A New Event was added to the timeline
	}
	public UPDATETYPE lastUpdate;
	public TimelineEvent eventLastAdded;

	private Timeline(){}

	//Used to get a reference to the timeline from anywhere
	public static Timeline Get(){
		if (instance == null) {
			instance = new Timeline ();
		}
		return instance;
	}


	//Can have different AddEvents with different parameters that make the correct event type
	public void AddEvent(Character chr, int nDelay, PRIORITY prior = PRIORITY.NONE){
		TimelineEventChr newEventChr = new TimelineEventChr (chr, prior);

		InsertEvent (newEventChr, nDelay);
	}



	//TODO:: Add some break conditions for when you run out of turns
	//Not public so you don't accidently call it from outside
	void InsertEvent(TimelineEvent newEvent, int nDelay){

		LinkedListNode<TimelineEvent> newPos = curEvent;

		//Search ahead nDelay turns
		while (nDelay > 0){
			newPos = newPos.Next;
			if(newPos.Value.prior == PRIORITY.TURN){
				nDelay--;
			}
		} 

		//Search through the events that turn and add it before the next event 
		// with higher priority
		while (true) {//note this will break since eventually you hit the next turn

			if (newPos.Next.Value.prior > newEvent.prior) {
				listEvents.AddAfter (newPos, newEvent);

				//Let it know its position in the list
				newEvent.nPlace = newPos.Value.nPlace;
				IncPlace (newPos.Next);

				break;
			} else {
				newPos = newPos.Next;
			}
		}

		//So that the view can know what it should update
		lastUpdate = UPDATETYPE.NEWEVENT;
		eventLastAdded = newEvent;
		NotifyObs ();
		lastUpdate = UPDATETYPE.NONE;

	}

	public void InitTurns(){

		for (int i = 0; i < MAXTURNS; i++) {
			TimelineEventTurn newEvent = new TimelineEventTurn (i);
			newEvent.nPlace = i;
			newEvent.InitMana ();//TODO::Make this only semi-random
			listEvents.AddLast (newEvent);

			//TODO:: Do all of this at once
			//Let the view know to update
			lastUpdate = UPDATETYPE.NEWEVENT;
			eventLastAdded = newEvent;
			NotifyObs ();
			lastUpdate = UPDATETYPE.NONE;

		}

		curEvent = listEvents.First;

	}


	public void IncPlace(LinkedListNode<TimelineEvent> curNode){

		while (curNode != null) {
			curNode.Value.IncPlace ();
			curNode = curNode.Next;
		}
	}

	public void DecPlace(LinkedListNode<TimelineEvent> curNode){

		while (curNode != null) {
			curNode.Value.DecPlace ();
			curNode = curNode.Next;
		}
	}


	//Initially give players turns 1 - 7
	public void InitChars(){
		for (int i = 0; i < mod.nPlayers; i++) {
			for (int j = 0; j < mod.arPlayers [i].nChrs; j++) {
				AddEvent (mod.arChrs [i] [j], 2 * j + i + 1);
			}
		}
	}

	public void InitTimeline(Model _mod){
		mod = _mod;

		listEvents = new LinkedList<TimelineEvent> ();
		InitTurns ();

		InitChars ();



	}
		
	public void EvaluateEvent(){
		Print ();
		curEvent.Value.Evaluate ();


		curEvent = curEvent.Next;
	}

	public void NotifyTick(){

		for (int i = 0; i < mod.nPlayers; i++) {
			for (int j = 0; j < Player.MAXCHRS; j++) {
				if (mod.arChrs [i] [j] == null) {
					continue; // A character isn't actually here (extra space for characters)
				}

				//Reduce the character's recharge
				mod.arChrs [i] [j].TimeTick ();
				//Reduce the cd of that character's actions
				NotifyTickAction (mod.arChrs [i] [j]);

			}
		}
	}

	public void NotifyTickAction(Character chr){

		for (int i = 0; i < Character.nActions; i++) {
			chr.arActions [i].TimeTick ();
		}
	}

	public void Print(){
		LinkedListNode<TimelineEvent> curNode = listEvents.First;
		Debug.Log ("Printing entire Timeline model");
		while (curNode != null) {
			Debug.Log (curNode.Value.prior + " " + curNode.Value.nPlace);

			curNode = curNode.Next;
		}
		Debug.Log ("Finished printing");
	}
}
