using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTimelineEventChr : ViewTimelineEvent<TimelineEventChr> {

    public GameObject goFrame;
    public GameObject goPortrait;

	public override float GetVertSpan (){
		return 0.3f + ViewTimeline.fEventGap;
	}

	void SetPortrait(Chr chr){
		string sImgPath = "Images/Chrs/img" + chr.sName;
		Sprite sprChr = Resources.Load(sImgPath, typeof(Sprite)) as Sprite;

		goPortrait.GetComponent<SpriteRenderer> ().sprite = sprChr;
	}

	public void InitPlayer(){
		// Subject to change
		if (mod.chrSubject.plyrOwner.id == 0) {
            //this.SetMaterial ("MatTimelineEvent1");
            goPortrait.transform.localPosition = new Vector3 (-1.0f, -0.26f, -0.1f);
            goPortrait.transform.localScale = new Vector3(0.2f, 0.2f, 1);

        } else {
            //this.SetMaterial ("MatTimelineEvent2");
            goPortrait.transform.localScale = new Vector3 (-0.2f, 0.2f, 1);
		}
	}

	public override void Start(){
		base.Start ();
		//Should have the model set by now

		InitPlayer ();
		SetPortrait (mod.chrSubject);
	}
}
