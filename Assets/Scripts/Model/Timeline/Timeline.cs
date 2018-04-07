using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Reponsible for keeping track of the order of events

[RequireComponent(typeof(ViewTimeline))]
public class Timeline : Subject {

	public static int MAXTURNS = 15;

	bool bStarted;

	public enum PRIORITY {
		BOT, //beginning of turn
		HIGH, //before normal actions
		NONE, //normal actions
		LOW, //after normal actions
		EOT, //end of turn
		TURN //marks a new turn
	};

	public Match match;

	public LinkedList <TimelineEvent> listEvents;

	public LinkedListNode <TimelineEvent> curEvent;

	public GameObject pfTimelineEventChr;
	public GameObject pfTimelineEventTurn;

	public ViewTimeline view;

	public static Timeline Get (){
		GameObject go = GameObject.FindGameObjectWithTag ("Timeline");
		if (go == null) {
			Debug.LogError ("ERROR! NO OBJECT HAS A TIMELINE TAG!");
		}
		Timeline instance = go.GetComponent<Timeline> ();
		if (instance == null) {
			Debug.LogError ("ERROR! TIMELINE TAGGED OBJECT DOES NOT HAVE A TIMELINE COMPONENT!");
		}
		return instance;
	}

	public void Start(){
		if (bStarted == false) {
			bStarted = true;

			match = Match.Get ();

			listEvents = new LinkedList<TimelineEvent> ();

			view = GetComponent<ViewTimeline> ();
			view.Start ();


		}
	}
	

	//Can have different AddEvents with different parameters that make the correct event type
	public void AddEvent(Chr chr, int nDelay, PRIORITY prior = PRIORITY.NONE){
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
				newEvent.nodeEvent = newPos;
				newEvent.NotifyObs ("MovedEvent", null);

				UpdateEventPositions (newPos.Next);

				break;
			} else {
				newPos = newPos.Next;
			}
		}
	}

	public void InitTurns(){

		for (int i = 0; i < MAXTURNS; i++) {
			GameObject goEvent = Instantiate (pfTimelineEventTurn, view.transEventContainer);
			TimelineEventTurn newEvent = goEvent.GetComponent<TimelineEventTurn> ();
			listEvents.AddLast (newEvent);
			newEvent.Start ();

			// Give a reference to the linked list node, and to the turn #
			newEvent.Init (listEvents.Last, i);


		}

		curEvent = listEvents.First;

	}


	// Update the timeline position for curNode and for everything after it
	public void UpdateEventPositions(LinkedListNode<TimelineEvent> curNode){

		while (curNode != null) {
			curNode.Value.NotifyObs ("MovedEvent", null);
			curNode = curNode.Next;
		}
	}


	//Initially give players turns 1 - 7
	public void InitChars(){
		for (int i = 0; i < match.nPlayers; i++) {
			for (int j = 0; j < match.arPlayers [i].nChrs; j++) {
				AddEvent (match.arChrs [i] [j], 2 * j + i + 1);
			}
		}
	}

	public void InitTimeline(){

		InitTurns ();

		//InitChars ();



	}
		
	public void EvaluateEvent(){
		//Print ();
		curEvent.Value.SetState(TimelineEvent.STATE.FINISHED);

		//Let the timeline know to shift upward
		NotifyObs ("FinishedEvent", curEvent.Value);

		curEvent = curEvent.Next;
		curEvent.Value.SetState(TimelineEvent.STATE.CURRENT);

		curEvent.Value.Evaluate ();



	}
		
	public void NewTurn(){

		for (int i = 0; i < match.nPlayers; i++) {
			for (int j = 0; j < Player.MAXCHRS; j++) {
				if (match.arChrs [i] [j] == null) {
					continue; // A character isn't actually here (extra space for characters)
				}

				//Reduce the character's recharge
				match.arChrs [i] [j].TimeTick ();
				//Reduce the cd of that character's actions
				RechargeActions (match.arChrs [i] [j]);

			}
		}
	}

	public void RechargeActions(Chr chr){

		for (int i = 0; i < Chr.nActions; i++) {
			chr.arActions [i].Recharge ();
		}
	}

	public void Print(){
		LinkedListNode<TimelineEvent> curNode = listEvents.First;
		Debug.Log ("Printing entire Timeline model");
		while (curNode != null) {
			Debug.Log (curNode.Value.prior + " " + curNode.Value.nodeEvent);

			curNode = curNode.Next;
		}
		Debug.Log ("Finished printing");
	}
}
