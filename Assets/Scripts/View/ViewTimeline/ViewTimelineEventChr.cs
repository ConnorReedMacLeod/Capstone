using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTimelineEventChr : ViewTimelineEvent {

    public GameObject goFrame;
    public GameObject goHeadshot;
    public GameObject goStatus;

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

    public void cbSetStatus(Object target, params object[] args) {
        string sImgPath = ""; ;

        switch (mod.chrSubject.stateSelect) {
            case Chr.STATESELECT.SELECTED:
                return;

            case Chr.STATESELECT.TARGGETING:
                sImgPath = "Images/Timeline/imgActionPlanning";

                break;
            case Chr.STATESELECT.IDLE:
                if (mod.chrSubject.bSetAction) {
                    sImgPath = "Images/Timeline/imgActionPlanned";
                } else {
                    sImgPath = "Images/Timeline/imgActionUnplanned";
                }
                break;
        }

        Sprite sprStatus = Resources.Load(sImgPath, typeof(Sprite)) as Sprite;
        goStatus.GetComponent<SpriteRenderer>().sprite = sprStatus;
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
        cbSetStatus(mod.chrSubject);
        mod.chrSubject.subStatusChange.Subscribe(cbSetStatus);
	}
}
