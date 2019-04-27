using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ViewBlockerButton : ViewInteractive {

    bool bStarted;                          //Confirms the Start() method has executed

    public const int id = Chr.idBlocking;                              //The standard id for the block action

    public static Subject subAllClick = new Subject(Subject.SubType.ALL);
    public static Subject subAllStartHover = new Subject(Subject.SubType.ALL);
    public static Subject subAllStopHover = new Subject(Subject.SubType.ALL);

    public bool ButtonVisible() {
        //Needs to have a character that's acting next this turn, and is selected (and maybe should be in the ability selection phase)
        return ContTurns.Get().GetNextActingChr() != null && ContTurns.Get().GetNextActingChr().stateSelect == Chr.STATESELECT.SELECTED;
    }

    public override void onMouseClick(params object[] args) {

        //If we can't actually use this button, then don't react to clicks
        //(No character selected or the selected character can't block anyway)
        if (!ButtonVisible() || !ContTurns.Get().GetNextActingChr().CanBlock()) return;

        subAllClick.NotifyObs(this, args);

        base.onMouseClick(args);
    }

    public override void onMouseStartHover(params object[] args) {

        //Only do something if there's actually a character that's gonna go 
        if (!ButtonVisible()) return;
        subAllStartHover.NotifyObs(this, args);

        base.onMouseStartHover(args);
    }

    public override void onMouseStopHover(params object[] args) {

        if (!ButtonVisible()) return;
        subAllStopHover.NotifyObs(this, args);

        base.onMouseStopHover(args);
    }


    public void Start() {
        if (bStarted == false) {
            bStarted = true;


            Chr.subAllStatusChange.Subscribe(cbChrSelectionChange);
            ContTurns.subAllPriorityChange.Subscribe(cbChrPriorityOrderChange);

            Display();
        }
    }

    public void Display() {

        string sImgPath;


        if (!ButtonVisible()) {
            //Then hide the button entirely for now if either no character is selected
            // or if the selected character isn't the next to act
            sImgPath = "null";

        } else if (ContTurns.Get().GetNextActingChr().CanBlock()){
            //Then we want the button to be visible and usable
            sImgPath = "Images/MiscUI/imgBlockerToken";

        } else {//Then we can't block
            //Then we just leave a greyed out blocker button
            sImgPath = "Images/MiscUI/imgBlockerTokenHolder";
        }

        Sprite sprBlockerButton = Resources.Load(sImgPath, typeof(Sprite)) as Sprite;
        this.GetComponent<SpriteRenderer>().sprite = sprBlockerButton;
    }



    public void cbChrSelectionChange(Object target, params object[] args) {
        //So if you select/unselect a character, then we can show/hide the button as needed
        Display();
    }

    public void cbChrPriorityOrderChange(Object target, params object[] args) {
        //So that if a character becomes the currently acting character or not, we can show/hide
        Display();
    }

}