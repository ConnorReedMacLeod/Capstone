using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewTimelineEventTurn : ViewTimelineEvent<TimelineEventTurn> {

    public SpriteRenderer rendMana;
    public Text txtTurnNumber;

	public override float GetVertSpan (){
		return 0.4f + ViewTimeline.fEventGap;
	}
		
	public override void UpdateObs(string eventType, Object target, params object[] args){
		switch (eventType) {
		case Notification.EventSetMana:

            SetImgMana(Mana.arsManaTypes [(int)(mod.manaGen)]);
			break;
		default:
			break;
		}

		base.UpdateObs (eventType, target, args);

	}

    public void SetTurnNumber(int _nTurnNumber) {
        txtTurnNumber.text = _nTurnNumber.ToString();
    }

	public void SetImgMana(string _sType){
		string sImgPath = "Images/Mana/ImgMana" + _sType + "Tiny";

		Sprite sprMana = Resources.Load(sImgPath, typeof(Sprite)) as Sprite;

        rendMana.sprite = sprMana;
	}

	public override void Start(){
		base.Start ();

		//SetMaterial ("MatTimelineEventTurn");
	}

}
