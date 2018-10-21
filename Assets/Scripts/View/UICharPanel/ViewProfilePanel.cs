using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewProfilePanel : MonoBehaviour {

    bool bStarted;                          //Confirms the Start() method has executed

    Chr chrFocus;

    //Textfields to display information
    public Text txtName;
    public Text txtPower;
    public Text txtDefense;

    public GameObject goHeadshot;

    public void cbChrSelected(Object target, params object[] args) {
        SetFocus((Chr)target);
    }

    public void cbChrUnselected(Object target, params object[] args) {
        SetFocus(null);
    }

    public void DisplayName() {
        if (chrFocus == null) {
            txtName.text = "";
        } else {
            txtName.text = chrFocus.sName;
        }
    }

    public void DisplayPower() {
        if (chrFocus == null) {
            txtPower.text = "";
        } else {
            txtPower.text = "0";//TODO:: UPDATE WHEN READY
        }
    }

    public void DisplayDefense() {
        if (chrFocus == null) {
            txtDefense.text = "";
        } else {
            txtDefense.text = "0";//TODO:: UPDATE WHEN READY
        }
    }

    void SetHeadshot(Chr chr) {
        //Load the blank image if no character is selected
        string sImgPath = "Images/UICharPanel/imgBlankHeadshot";

        if (chr != null) {
            //If a character is selected, then grab their headshot
            sImgPath = "Images/Chrs/" + chr.sName + "/img" + chr.sName + "Headshot";
        }
        Sprite sprChr = Resources.Load(sImgPath, typeof(Sprite)) as Sprite;

        goHeadshot.GetComponent<SpriteRenderer>().sprite = sprChr;
    }

    public void DisplayAll() {
        DisplayName();
        DisplayPower();
        DisplayDefense();
        SetHeadshot(chrFocus);
    }

    void SetFocus(Chr _chrFocus) {
        chrFocus = _chrFocus;
        DisplayAll();
    }

    // Use this for initialization
    void Start () {
        if (bStarted == false) {
            bStarted = true;

            SetFocus(null);

            Chr.subAllStartSelect.Subscribe(cbChrSelected);
            Chr.subAllStartIdle.Subscribe(cbChrUnselected);
        }
    }

}
