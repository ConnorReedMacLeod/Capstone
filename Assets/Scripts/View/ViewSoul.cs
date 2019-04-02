using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewSoul : ViewInteractive {

    public Soul mod;

    public Text txtDuration;


    public static Subject subAllStartHover = new Subject();
    public static Subject subAllStopHover = new Subject();


    public override void onMouseStartHover(params object[] args) {

        subAllStartHover.NotifyObs(this, args);

        base.onMouseStartHover(args);
    }

    public override void onMouseStopHover(params object[] args) {

        subAllStopHover.NotifyObs(this, args);

        base.onMouseStopHover(args);
    }


    public void UpdateSoulSprite() {

        string sSprPath = "Images/Soul/imgSoulEmpty2";

        if (mod != null) {

            sSprPath = "Images/Soul/imgSoul" + mod.sName;

        }
        Sprite sprSoul = Resources.Load(sSprPath, typeof(Sprite)) as Sprite;

        GetComponent<SpriteRenderer>().sprite = sprSoul;

    }

    public void UpdateTxtDuration() {

        if (mod == null || mod.bDuration == false) {
            //Then we don't have any duration to display
            txtDuration.text = "";
        } else {
            txtDuration.text = mod.nCurDuration.ToString();
        }


    }

    public void UpdateSoul(Soul _mod) {
        mod = _mod;

        UpdateSoulSprite();
        UpdateTxtDuration();
    }
}
