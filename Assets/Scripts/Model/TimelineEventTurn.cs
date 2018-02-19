using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineEventTurn : TimelineEvent {

	public int nTurn;
	public Mana.MANATYPE manaGen;

	public void InitMana(){
		manaGen = (Mana.MANATYPE)Random.Range (0, Mana.nManaTypes - 1);
	}

	public TimelineEventTurn(int _nTurn){
		prior = Timeline.PRIORITY.TURN;
		nTurn = _nTurn;

		fDelay = 4.0f;

	}

	public override void Evaluate(){

		Debug.Log ("It's now turn " + nTurn);

		//Give the mana to each player
		for (int i = 0; i < Timeline.Get().mod.nPlayers; i++) {
			Timeline.Get().mod.arPlayers [i].mana.AddMana (manaGen);
		}

		Debug.Log ("Giving out " + manaGen + " mana");

		//Let players know to update their cds/recharges
		Timeline.Get().NotifyTick ();

		base.Evaluate ();
	}
}
