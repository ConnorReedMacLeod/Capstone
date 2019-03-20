using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewSoul : MonoBehaviour {

    public Soul mod;

    public Text txtDuration;

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
