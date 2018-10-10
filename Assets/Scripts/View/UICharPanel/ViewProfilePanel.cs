using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewProfilePanel : Observer {

    bool bStarted;                          //Confirms the Start() method has executed

    Chr chrFocus;

    //Textfields to display information
    public Text txtName;
    public Text txtPower;
    public Text txtDefense;

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

    public void DisplayAll() {
        DisplayName();
        DisplayPower();
        DisplayDefense();
    }

    void SetFocus(Chr _chrFocus) {
        chrFocus = _chrFocus;
        DisplayAll();
    }

    override public void UpdateObs(string eventType, Object target, params object[] args) {
        Debug.Log(eventType);
        switch (eventType) {
            

            //TODO:: Figure out when to update this - for like power/def changes
            case Notification.ChrSelected:
                SetFocus((Chr)target);
                break;

            case Notification.ChrUnselected:
                SetFocus(null);
                break;

            default:
                break;
        }
    }

    void Init() {

        Text[] arTextComponents = GetComponentsInChildren<Text>();

        for (int i = 0; i < arTextComponents.Length; i++) {

            switch (arTextComponents[i].name) {
                case "txtName":
                    txtName = arTextComponents[i];
                    break;

                case "txtPower":
                    txtPower = arTextComponents[i];
                    break;

                case "txtDefense":
                    txtDefense = arTextComponents[i];
                    break;

                default:
                    Debug.LogError("ERROR! Unrecognized Text component in ViewProfilePanel");
                    break;

            }
        }

        SetFocus(null);
    }
      

    // Use this for initialization
    void Start () {
        if (bStarted == false) {
            bStarted = true;
            Init();

            Match.Get().Start();
            //Start listening to each of the characters
            for(int i=0; i<Match.Get().nPlayers; i++) {
                for(int j=0; j<Match.Get().arChrs[1].Length; j++) {
                    Match.Get().arChrs[i][j].Subscribe(this);
                }
            }
        }
    }

}
