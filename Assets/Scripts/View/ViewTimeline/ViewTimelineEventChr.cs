using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTimelineEventChr : ViewTimelineEvent<TimelineEventChr> {

    public GameObject goFrame;
    public GameObject goPortrait;
    public GameObject goStatus;

	public override float GetVertSpan (){
		return 0.3f + ViewTimeline.fEventGap;
	}

	public override void UpdateObs(string eventType, Object target, params object[] args){

		switch (eventType) {
		case "NewChr":
			SetPortrait (mod.chrSubject);
			break;

        case Notification.EventChangedState:
                Debug.Log("state changed");
                UpdateStatus();
            break;

        default:

			break;
		}
        

        base.UpdateObs (eventType, target, args);
	}

	void SetPortrait(Chr chr){
		string sImgPath = "Images/Chrs/img" + chr.sName;
		Sprite sprChr = Resources.Load(sImgPath, typeof(Sprite)) as Sprite;

		goPortrait.GetComponent<SpriteRenderer> ().sprite = sprChr;
	}

    void UpdateStatus() {

        string sImgPath = ""; ;

        switch (mod.chrSubject.stateSelect) {
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
