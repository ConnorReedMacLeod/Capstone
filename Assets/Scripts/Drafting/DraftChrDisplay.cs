using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraftChrDisplay : MonoBehaviour {

    public CharType.CHARTYPE chrInSlot;
    public Image imgPortrait;
    public Text txtChrNameLabel;

    public void RedOut() {

        imgPortrait.color = new Color(1f, 0f, 0f, 0.5f);
    }

    public void GreyOut() {

        imgPortrait.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }

    public void SetEmptySlot() {
        txtChrNameLabel.text = "---";

        imgPortrait.overrideSprite = Resources.Load("Images/Chrs/imgLain", typeof(Sprite)) as Sprite;
    }

    public void SetChrInSlot(CharType.CHARTYPE _chrInSlot) {

        chrInSlot = _chrInSlot;

        if(chrInSlot == CharType.CHARTYPE.LENGTH) {
            SetEmptySlot();
            return;
        }

        string sChrName = CharType.GetChrName(chrInSlot);

        txtChrNameLabel.text = sChrName;

        //For some reason, setting the overrideSprite (rather than the normal sprite) works here.  Unity is dumb
        imgPortrait.overrideSprite = Resources.Load("Images/Chrs/" + sChrName + "/img" + sChrName + "neutral", typeof(Sprite)) as Sprite;
        //LibView.AssignSpritePathToObject("Images/Chrs/" + sChrName + "/img" + sChrName + "neutral", this.gameObject);
    }
}
