﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ViewRestButton : ViewInteractive {

    bool bStarted;                          //Confirms the Start() method has executed

    public static Subject subAllClick = new Subject(Subject.SubType.ALL);
    public static Subject subAllStartHover = new Subject(Subject.SubType.ALL);
    public static Subject subAllStopHover = new Subject(Subject.SubType.ALL);

    public bool ButtonVisible() {
        //Needs to have a character that's acting next this turn, and is selected (and maybe should be in the skill selection phase)
        return ContTurns.Get().GetNextActingChr() != null && ContTurns.Get().GetNextActingChr().stateSelect == Chr.STATESELECT.SELECTED;
    }

    public override void onMouseClick(params object[] args) {

        base.onMouseClick(args);

        //If we can't actually use this button, then don't react to clicks
        if (!ButtonVisible()) return;

        subAllClick.NotifyObs(this, args);

    }

    public override void onMouseStartHover(params object[] args) {

        base.onMouseStartHover(args);

        //Only do something if there's actually a character that's gonna go 
        if (!ButtonVisible()) return;
        subAllStartHover.NotifyObs(this, args);

    }

    public override void onMouseStopHover(params object[] args) {

        base.onMouseStopHover(args);

        if (!ButtonVisible()) return;
        subAllStopHover.NotifyObs(this, args);

    }


    public override void Start() {
        if(bStarted == false) {
            bStarted = true;


            Chr.subAllStatusChange.Subscribe(cbChrSelectionChange);
            ContTurns.Get().subAllPriorityChange.Subscribe(cbChrPriorityOrderChange);

            Display();

            base.Start();
        }
    }

    public void Display() {

        string sImgPath;


        if(!ButtonVisible()) {
            //Then hide the button entirely for now if either no character is selected
            // or if the selected character isn't the next to act
            sImgPath = "null";

        } else {//TODO:: Consider if there should be some check for what character state we're in
            //Then we want the button to be visible and usable
            sImgPath = "Images/MiscUI/imgSurrender";
        }

        Sprite sprRestButton = Resources.Load(sImgPath, typeof(Sprite)) as Sprite;

        this.GetComponent<SpriteRenderer>().sprite = sprRestButton;
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