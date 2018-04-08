using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTimelineEventChr : ViewTimelineEvent<TimelineEventChr> {

	private int indexEventPortrait = 1;

	public override float GetVertSpan (){
		return 0.8f + ViewTimeline.fEventGap;
	}

	public override void UpdateObs(string eventType, Object target, params object[] args){

		switch (eventType) {
		case "NewChr":
			SetPortrait (mod.chrSubject);
			break;
		default:

			break;
		}


		base.UpdateObs (eventType, target, args);
	}

	void SetPortrait(Chr chr){
		string sMatPath = "Materials/Chrs/Mat" + chr.sName;
		Material matChr = Resources.Load(sMatPath, typeof(Material)) as Material;

		GetComponentsInChildren<Renderer> ()[indexEventPortrait].material = matChr;
	}

	public void InitPlayer(){
		// Subject to change
		if (mod.chrSubject.plyrOwner.id == 0) {
			this.SetMaterial ("MatTimelineEvent1");
			transform.GetChild(indexEventPortrait).transform.localPosition = new Vector3 (-0.75f, -0.4f, -0.1f); 

		} else {
			this.SetMaterial ("MatTimelineEvent2");
			transform.GetChild(indexEventPortrait).transform.localScale = new Vector3 (-0.7f, 0.7f, 1);
		}
	}

	public override void Start(){
		base.Start ();
		//Should have the model set by now

		InitPlayer ();
		SetPortrait (mod.chrSubject);
	}
}
