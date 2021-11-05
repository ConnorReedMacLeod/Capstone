using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraftableChr : ViewInteractive {
    
    public Chr.CHARTYPE chrInSlot;
    public Image imgPortrait;

    public void RedOut() {

        imgPortrait.color = new Color(1f, 0f, 0f, 0.5f);

       // Debug.Log("Redding out for " + imgPortrait.sprite);
        //imgOverlay.color = new Color(255, 0, 0, 127);

        //Debug.Log("Color for red is now " + imgOverlay.color);
    }

    public void GreyOut() {
        //Debug.Log("Greying out for " + imgPortrait.sprite);
        //imgOverlay.color = new Color(127, 127, 127, 127);
        //Debug.Log("Color for grey is now " + imgOverlay.color);

        imgPortrait.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }

    public void SetChrInSlot(Chr.CHARTYPE _chrInSlot) {
        if(chrInSlot == _chrInSlot) {
            return;
        }

        chrInSlot = _chrInSlot;

        string sChrName = Chr.ARSCHRNAMES[(int)chrInSlot];
      
        //For some reason, setting the overrideSprite (rather than the normal sprite) works here.  Unity is dumb
        imgPortrait.overrideSprite = Resources.Load("Images/Chrs/" + sChrName + "/img" + sChrName + "neutral", typeof(Sprite)) as Sprite;
        //LibView.AssignSpritePathToObject("Images/Chrs/" + sChrName + "/img" + sChrName + "neutral", this.gameObject);
    }

    public override void onMouseClick(params object[] args) {
        

        Debug.Log("Clicked on " + chrInSlot);
        GreyOut();

        base.onMouseClick(args);
    }


}
