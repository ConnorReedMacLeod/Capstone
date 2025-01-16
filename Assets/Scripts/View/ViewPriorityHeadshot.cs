using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewPriorityHeadshot : MonoBehaviour {

    public Text txtFatigueLabel;
    public Chr chr;
    
    public void SetChrDisplaying(Chr _chr) {
        if (chr != null) {
            chr.subFatigueChange.UnSubscribe(cbTargetPriorityUpdated);
            chr.subSwitchingInChange.UnSubscribe(cbTargetPriorityUpdated);
        }

        chr = _chr;

        if (chr != null) {
            string sImgPath = "Images/Chrs/" + chr.sName + "/img" + chr.sName + "Neutral";

            LibView.AssignSpritePathToObject(sImgPath, this.gameObject);

            chr.subFatigueChange.Subscribe(cbTargetPriorityUpdated);
            chr.subSwitchingInChange.Subscribe(cbTargetPriorityUpdated);
            UpdateLabel();
        }

    }

    public void DestroyHeadshot() {
        chr.subFatigueChange.UnSubscribe(cbTargetPriorityUpdated);
        chr.subSwitchingInChange.UnSubscribe(cbTargetPriorityUpdated);
        Destroy(this.gameObject);
    }

    public void DisplayFatigue() {
        txtFatigueLabel.color = Color.magenta;
        txtFatigueLabel.text = chr.curStateReadiness.GetPriority().ToString();
    }

    public void DisplaySwitchInTime() {
        txtFatigueLabel.color = Color.green;
        txtFatigueLabel.text = string.Format("{0}({1})", chr.curStateReadiness.GetPriority().ToString(), chr.nSwitchingInTime);
    }

    public void UpdateLabel() {
        if(chr.curStateReadiness.Type() == StateReadiness.TYPE.SWITCHINGIN) {
            DisplaySwitchInTime();
        } else {
            DisplayFatigue();
        }
    }

    public void cbTargetPriorityUpdated(Object tar, params object[] args) {
        UpdateLabel();
    }
}
