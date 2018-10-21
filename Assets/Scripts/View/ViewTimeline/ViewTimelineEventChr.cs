using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTimelineEventChr : ViewTimelineEvent {

    public GameObject goFrame;
    public GameObject goHeadshot;

    public new TimelineEventChr mod {
        get {
            return (TimelineEventChr)GetMod();
        }
        set {
            mod = value;
        }
    }

    public override TimelineEvent GetMod() {
        return GetComponent<TimelineEventChr>();
    }

    public override float GetVertSpan (){
		return 0.3f + ViewTimeline.fEventGap;
	}

	void SetHeadshot(Chr chr){

        string sImgPath = "Images/Chrs/" + chr.sName + "/img" + chr.sName + "Headshot";

        Sprite sprChr = Resources.Load(sImgPath, typeof(Sprite)) as Sprite;

		goHeadshot.GetComponent<SpriteRenderer> ().sprite = sprChr;
	}

	public void InitPlayer(){
		// Subject to change
		if (mod.chrSubject.plyrOwner.id == 0) {
            //this.SetMaterial ("MatTimelineEvent1");
            goHeadshot.transform.localPosition = new Vector3 (-1.0f, -0.26f, -0.1f);
            goHeadshot.transform.localScale = new Vector3(0.4f, 0.4f, 1);

        } else {
            //this.SetMaterial ("MatTimelineEvent2");
            goHeadshot.transform.localScale = new Vector3 (-0.4f, 0.4f, 1);
		}
	}

	public override void Start(){
		base.Start ();
		//Should have the model set by now

		InitPlayer ();
		SetHeadshot (mod.chrSubject);
	}
}
