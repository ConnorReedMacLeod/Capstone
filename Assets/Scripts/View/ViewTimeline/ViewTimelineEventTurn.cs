using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTimelineEventTurn : ViewTimelineEvent {


	public TimelineEventTurn mod;

	private int indexEventPortrait = 1;

	public void SetModel(TimelineEventTurn _mod){
		mod = _mod;
		mod.Subscribe (this);


	}

	public override float GetVertSpan (){
		return 0.5f + ViewTimeline.fEventGap;
	}

	public override int GetPlace(){
		return mod.nPlace;
	}

	public override TimelineEvent.STATE GetState (){
		return mod.state;
	}

	public override void UpdateObs(){
		SetPortrait (Mana.arsManaTypes [(int)mod.manaGen]);

		base.UpdateObs ();
	}

	public override void Print (){
		Debug.Log ("I am the " + mod.nPlace + "th node and I'm Turn " + mod.nTurn);
	}

	void SetPortrait(string _sType){
		string sMatPath = "Materials/Mana/Mat" + _sType;
		Material matChr = Resources.Load(sMatPath, typeof(Material)) as Material;

		GetComponentsInChildren<Renderer> ()[indexEventPortrait].material = matChr;
	}

	public override void Start(){
		base.Start ();

		this.SetMaterial ("MatTimelineEventTurn");
	}

}
