using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewTimelineEventTurn : ViewTimelineEvent {

    public SpriteRenderer rendMana;
    public Text txtTurnNumber;

    public new TimelineEventTurn mod {
        get {
            return (TimelineEventTurn)GetMod();
        }
        set {
            mod = value;
        }
    }

    public override TimelineEvent GetMod() {
        return GetComponent<TimelineEventTurn>();
    }

    public override float GetVertSpan (){
		return 0.4f + ViewTimeline.fEventGap;
	}

    public void cbSetMana(Object target, params object[] args) {
        SetImgMana(Mana.arsManaTypes[(int)(mod.manaGen)]);
    }

    public void cbSetTurn(Object target, params object[] args) {
        txtTurnNumber.text = ((TimelineEventTurn)mod).nTurn.ToString();
    }

	public void SetImgMana(string _sType){
		string sImgPath = "Images/Mana/ImgMana" + _sType + "Tiny";

		Sprite sprMana = Resources.Load(sImgPath, typeof(Sprite)) as Sprite;

        rendMana.sprite = sprMana;
	}

	public override void Start(){
		base.Start ();

        mod.subSetTurn.Subscribe(cbSetTurn);
        mod.subSetMana.Subscribe(cbSetMana);
		//SetMaterial ("MatTimelineEventTurn");
	}

}
