using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewActionPanel : Observer {

    bool bStarted;                          //Confirms the Start() method has executed

    //Have a reference to the character so we know what abilities to represent
    public Chr chrSelected;

    public ViewAction[] arViewAction = new ViewAction[4];


    override public void UpdateObs(string eventType, Object target, params object[] args) {

        switch (eventType) {
            //TODO:: Consider adding in field-specific update types if only one field needs updating

            case Notification.ChrSelected:
                Debug.Log("Setting Target to " + ((Chr)target).sName);
                setModel((Chr)target);
                break;

            default:
                break;
        }
    }


    public void setModel(Chr _chrSelected) {

        chrSelected = _chrSelected;

        //TODO:: Register each of the actions this contains
        // currently, all of the registration is done in the inspector
        // I'm not sure how I feel about the sturdiness of that...

        for (int i = 0; i < arViewAction.Length; i++) {
            arViewAction[i].SetModel(chrSelected.arActions[i]);
        }

    }

    public void Start() {
        if (bStarted == false) {
            bStarted = true;
            setModel(null);
        }

    }
}
