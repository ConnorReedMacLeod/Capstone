using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewBlockerButton : ViewInteractive {

    bool bStarted;                          //Confirms the Start() method has executed

    public const int id = 8;                              //The standard id for the block action
    public Chr chrSelected;
    public Action mod;                      		//The action's model

    public static Subject subAllClick = new Subject();

    public override void onMouseClick(params object[] args) {

        //If we can't actually use this button, then don't react to clicks
        if (!CanDeclareBlocker()) return;

        subAllClick.NotifyObs(this, args);

        base.onMouseClick(args);
    }

    public void Start() {
        if (bStarted == false) {
            bStarted = true;

            Chr.subAllStartSelect.Subscribe(cbChrSelected);
            Chr.subAllStartTargetting.Subscribe(cbChrUnselected);
            Chr.subAllStartIdle.Subscribe(cbChrUnselected);
        }
    }

    public bool CanDeclareBlocker() {
        Debug.Log(chrSelected + " " + chrSelected.bBlocker + " " + chrSelected.pbCanBlock.Get());
        return chrSelected != null && chrSelected.bBlocker == false && chrSelected.pbCanBlock.Get() == true;
    }

    public void Display() {

        string sImgPath;


        if (chrSelected == null || ContTurns.Get().GetNextActingChr() != chrSelected) {
            //Then hide the button entirely for now if either no character is selected
            // or if the selected character isn't the next to act
            sImgPath = "null";

        } else if (CanDeclareBlocker()){
            //Then we want the button to be visible and usable
            sImgPath = "Images/MiscUI/imgBlockerToken";

        } else {
            //Then we just leave a greyed out blocker button
            sImgPath = "Images/MiscUI/imgBlockerTokenHolder";
        }

        Sprite sprBlockerButton = Resources.Load(sImgPath, typeof(Sprite)) as Sprite;
        this.GetComponent<SpriteRenderer>().sprite = sprBlockerButton;
    }


    //Let the Action button know which action it's representing
    public void SetModel(Action _mod) {

        mod = _mod;
        Display();

    }

    public void cbChrSelected(Object target, params object[] args) {
        chrSelected = (Chr)target;
        SetModel(chrSelected.arActions[id]);
    }

    public void cbChrUnselected(Object target, params object[] args) {
        chrSelected = null;
        SetModel(null);
    }

}