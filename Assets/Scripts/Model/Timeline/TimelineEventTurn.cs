using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineEventTurn : TimelineEvent {

	public int nTurn;
	public Mana.MANATYPE manaGen;

    public new ViewTimelineEventTurn view {
        get {
            return (ViewTimelineEventTurn)GetView();
        }
        set {
            view = value;
        }
    }

    public Subject subSetMana = new Subject();
    public static Subject subAllSetMana = new Subject();

    public Subject subSetTurn = new Subject();

    public override ViewTimelineEvent GetView() {
        //TODO:: Consider if there's a way to do this without
        //       a unity library function call each time
        return GetComponent<ViewTimelineEventTurn>();
    }

    public void InitMana(){
		//TODO::Make this only semi-random
		manaGen = (Mana.MANATYPE)Random.Range (0, Mana.nManaTypes - 1);

        subSetMana.NotifyObs(this);
        subAllSetMana.NotifyObs(this);
    }

	public void Init(LinkedListNode<TimelineEvent> _eventNode, int _nTurn){
		base.Init (_eventNode);

		prior = Timeline.PRIORITY.TURN;
        
		nTurn = _nTurn;
        subSetTurn.NotifyObs(this);

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
