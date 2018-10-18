using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineEventTurn : TimelineEvent {

	public int nTurn;
	public Mana.MANATYPE manaGen;

	//public ViewTimelineEvent<TimelineEventTurn> view;
    public ViewTimelineEventTurn view;

    public Subject subSetMana;
    public static Subject subAllSetMana;

	public void InitMana(){
		//TODO::Make this only semi-random
		manaGen = (Mana.MANATYPE)Random.Range (0, Mana.nManaTypes - 1);

        subSetMana.NotifyObs(this);
        subAllSetMana.NotifyObs(this);
    }

	public override void InitView(){
		
		view = GetComponent<ViewTimelineEventTurn>();
		if (view == null){
			Debug.LogError ("ERROR! COUDLN't FIND A VIEWTIMELINEEVENTTURN COMPONENT");
		}
		view.Start ();
         //TODO:: Consider moving this elsewhere
	}

	public override float GetVertSpan (){
		return view.GetVertSpan ();
	}
	public override Vector3 GetPosAfter (){
		return view.GetPosAfter ();
	}

	public void Init(LinkedListNode<TimelineEvent> _eventNode, int _nTurn){
		base.Init (_eventNode);

		prior = Timeline.PRIORITY.TURN;
        
		nTurn = _nTurn;
        view.SetTurnNumber(nTurn);

        InitMana ();

		fDelay = 4.0f;

		state = STATE.READY;

	}

	public override void Evaluate(){

		//Debug.Log ("It's now turn " + nTurn);

		//Give the mana to each player
		for (int i = 0; i < Timeline.Get().match.nPlayers; i++) {
			Timeline.Get().match.arPlayers [i].mana.AddMana (manaGen);
		}

		Debug.Log ("Giving out " + manaGen + " mana");

		//Let players know to update their cds/recharges
		Timeline.Get().NewTurn();

		base.Evaluate ();
	}
}
