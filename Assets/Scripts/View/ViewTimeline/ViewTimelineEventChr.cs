using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTimelineEventChr : ViewTimelineEvent {

	public string sLastName;

	public TimelineEventChr mod;

	private int indexEventPortrait = 1;

	public void SetModel(TimelineEventChr _mod){
		mod = _mod;
		mod.Subscribe (this);
	}

	public override float GetVertSpan (){
		return 0.8f + ViewTimeline.fEventGap;
	}
		
	public override int GetPlace(){
		return mod.nPlace;
	}

	public override TimelineEvent.STATE GetState (){
		return mod.state;
	}

	public override void UpdateObs(){
		if (sLastName != mod.chrSubject.sName) {
			sLastName = mod.chrSubject.sName;
			SetPortrait (sLastName);
		}


		base.UpdateObs ();
	}

	public override void Print (){
		Debug.Log ("I am the " + mod.nPlace + "th node and I represent " + mod.chrSubject.sName);
	}

	void SetPortrait(string _sName){
		string sMatPath = "Materials/Characters/mat" + _sName;
		Material matChr = Resources.Load(sMatPath, typeof(Material)) as Material;

		GetComponentsInChildren<Renderer> ()[indexEventPortrait].material = matChr;
	}

	public override void Start(){
		base.Start ();

		// Subject to change
		if (mod.chrSubject.playOwner.id == 0) {
			this.SetMaterial ("MatTimelineEvent1");
			transform.GetChild(indexEventPortrait).transform.localPosition = new Vector3 (-0.75f, -0.4f, -0.1f); 

		} else {
			this.SetMaterial ("MatTimelineEvent2");
			transform.GetChild(indexEventPortrait).transform.localScale = new Vector3 (-0.7f, 0.7f, 1);
		}
	}
}
