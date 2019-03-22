using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewAbilityPanel : ViewInteractive {

    public bool bStarted = false;

    public Chr chrSelected;

    public ViewAction [] arViewAction= new ViewAction[4];

    public Vector3 v3Offscreen = new Vector3(-100f, -100f, 0f);


    public void cbChrSelected(Object target, params object[] args) {
        OnSelectChar((Chr)target);
    }

    public void cbChrUnselected(Object target, params object[] args) {
        Deselect((Chr)target);
    }

    


    public void OnSelectChar(Chr _chrSelected) {
        chrSelected = _chrSelected;

        //Position the panel next to the selected character
        this.transform.position = chrSelected.transform.position;

        //Notify each ability 
        for (int i = 0; i < arViewAction.Length; i++) {
            if (chrSelected != null) {
                arViewAction[i].SetModel(chrSelected.arActions[i]);
            } else {
                arViewAction[i].SetModel(null);
            }
        }

    }

    public void Deselect(Chr chrUnselected) {
        Debug.Log("Calling deselect for " + chrUnselected);
        if (chrSelected != chrUnselected)
            //Don't do anything if we're unselecting a character that we're not currently selecting (probably some race condition
            return;
        

        //Move the panel offscreen
        this.transform.position = v3Offscreen;

        //Notify each ability 
        for (int i = 0; i < arViewAction.Length; i++) {
            arViewAction[i].SetModel(null);
        }
    }

    public void Start() {
        if (bStarted == false) {
            bStarted = true;

            //Move the panel offscreen
            this.transform.position = v3Offscreen;

            //Start each ability as representing nothing 
            for (int i = 0; i < arViewAction.Length; i++) {
                arViewAction[i].SetModel(null);
            }

            Chr.subAllStartSelect.Subscribe(cbChrSelected);
            Chr.subAllStartIdle.Subscribe(cbChrUnselected);
        }
    }
}
