using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewPriorityHeadshot : MonoBehaviour {

    public Text txtFatigueLabel;
    public Chr chr;
    
    public void SetChrDisplaying(Chr _chr) {
        if (chr != null) {
            chr.subFatigueChange.UnSubscribe(cbTargetFatigueUpdated);
        }

        chr = _chr;

        if (chr != null) {
            string sImgPath = "Images/Chrs/" + chr.sName + "/img" + chr.sName + "Neutral";

            LibView.AssignSpritePathToObject(sImgPath, this.gameObject);

            chr.subFatigueChange.Subscribe(cbTargetFatigueUpdated);
            SetFatigueLabel();
        }

    }

    public void DestroyHeadshot() {
        chr.subFatigueChange.UnSubscribe(cbTargetFatigueUpdated);
        Destroy(this.gameObject);
    }

    public void SetFatigueLabel() {
        txtFatigueLabel.text = chr.nFatigue.ToString();
    }

    public void cbTargetFatigueUpdated(Object tar, params object[] args) {
        SetFatigueLabel();
    }
}
