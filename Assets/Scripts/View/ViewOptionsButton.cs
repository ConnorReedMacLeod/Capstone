using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ViewOptionsButton : ViewInteractive {

    bool bStarted;                          //Confirms the Start() method has executed

    public bool bSelected = false;

    public Subject subClick = new Subject();


    public override void onMouseClick(params object[] args) {

        bSelected = true;
        Display();

        //Rely on our owner to properly subscribe skills to our Subject

        subClick.NotifyObs(this, args);

        base.onMouseClick(args);
    }

    public void Start() {
        if(bStarted == false) {
            bStarted = true;

            Display();
        }
    }

    public void Display() {

        string sMatPath;

        if(bSelected) {
            //Then we want the button to use the selected material
            sMatPath = "Materials/matButtonSelected";

        } else {
            //Then choose the unselected material
            sMatPath = "Materials/matButtonUnselected";
        }

        Material matButton = Resources.Load(sMatPath, typeof(Material)) as Material;
        this.GetComponent<MeshRenderer>().material = matButton;
    }


    //If we are acting as a radio button with mutually exclusive alternatives, then we should react if one of
    // the other options has been clicked
    public void cbSelectedOptionInGroup(Object target, params object[] args) {

        if((ViewOptionsButton)target == this) {
            //If we were the one selected, then we don't need to do anything (should already be selected)
            Debug.Assert(bSelected = true);
        } else {
            //Otherwise a mutually exclusive option was selected, so deselect ourselves
            bSelected = false;
        }

        Display();
    }

}