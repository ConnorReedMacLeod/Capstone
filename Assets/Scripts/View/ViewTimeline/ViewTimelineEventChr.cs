using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTimelineEventChr : ViewTimelineEvent {


	public TimelineEventChr mod;

	private int indexEventPortrait = 1;

	public void SetModel(TimelineEventChr _mod){
		mod = _mod;
		mod.Subscribe (this);
	}

	//TODO:: Fix scaling - event images are anchored in the center which means
	//       incrementing by the height doesn't work for different adjacent elements
	//       Fix:  add the images to an empty parent and anchor them at the top
	public override Vector3 GetPosAfter(){
		// This height is hardcoded - see the comment above the parent's Start method
		return new Vector3(v3Pos.x, v3Pos.y - 0.8f - ViewTimeline.fEventGap, v3Pos.z);
	}
		
	public override int GetPlace(){
		return mod.nPlace;
	}

	public override void UpdateObs(){
		SetPortrait (mod.chrSubject.sName);
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
		}
	}
}
