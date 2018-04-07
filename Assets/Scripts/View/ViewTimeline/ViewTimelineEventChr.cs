using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTimelineEventChr : ViewTimelineEvent<TimelineEventChr> {

	public string sLastName;

	private int indexEventPortrait = 1;

	public override float GetVertSpan (){
		return 0.8f + ViewTimeline.fEventGap;
	}

	public override void UpdateObs(string eventType, Object target, params object[] args){
		if (sLastName != mod.chrSubject.sName) {
			sLastName = mod.chrSubject.sName;
			SetPortrait (sLastName);
		}


		base.UpdateObs (eventType, target, args);
	}

	void SetPortrait(string _sName){
		string sMatPath = "Materials/Characters/mat" + _sName;
		Material matChr = Resources.Load(sMatPath, typeof(Material)) as Material;

		GetComponentsInChildren<Renderer> ()[indexEventPortrait].material = matChr;
	}

	public override void Start(){
		base.Start ();

		// Subject to change
		if (mod.chrSubject.plyrOwner.id == 0) {
			this.SetMaterial ("MatTimelineEvent1");
			transform.GetChild(indexEventPortrait).transform.localPosition = new Vector3 (-0.75f, -0.4f, -0.1f); 

		} else {
			this.SetMaterial ("MatTimelineEvent2");
			transform.GetChild(indexEventPortrait).transform.localScale = new Vector3 (-0.7f, 0.7f, 1);
		}
	}
}
