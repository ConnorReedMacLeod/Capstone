using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTimelineEventTurn : ViewTimelineEvent<TimelineEventTurn> {

	private int indexEventPortrait = 1;

	public override float GetVertSpan (){
		return 0.4f + ViewTimeline.fEventGap;
	}
		
	public override void UpdateObs(string eventType, Object target, params object[] args){
		switch (eventType) {
		case Notification.EventSetMana:

			SetPortrait (Mana.arsManaTypes [(int)(mod.manaGen)]);
			break;
		default:
			break;
		}

		base.UpdateObs (eventType, target, args);

	}

	void SetPortrait(string _sType){
		string sMatPath = "Materials/Mana/Mat" + _sType;

		Material matChr = Resources.Load(sMatPath, typeof(Material)) as Material;

		GetComponentsInChildren<Renderer> ()[indexEventPortrait].material = matChr;
	}

	public override void Start(){
		base.Start ();

		SetMaterial ("MatTimelineEventTurn");
	}

}
