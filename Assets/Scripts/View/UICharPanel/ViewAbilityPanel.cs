using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewAbilityPanel : ViewInteractive {

    public bool bStarted = false;

    public Chr chrSelected;

    public ViewAction[] arViewAction = new ViewAction[4];

    public Vector3 v3Offscreen = new Vector3(-100f, -100f, 0f);

    private Vector3[] arV3AbilityPositions = new Vector3[4];

    private float fXOffsetMultiplier = 1f;
    private float fXOffset = 2.1f;
    private float fYOffset = 0.35f;

    private int nCurrentShowingPlyrID = 0;

    public void cbChrSelected(Object target, params object[] args) {
        OnSelectChar((Chr)target);
    }

    public void cbChrUnselected(Object target, params object[] args) {
        Deselect((Chr)target);
    }


    public void ReverseAbilityPanel() {

        for(int i = 0; i < 4 / 2; i++) {
            Vector3 v3Temp = arV3AbilityPositions[i];
            arV3AbilityPositions[i] = arV3AbilityPositions[4 - i - 1];
            arV3AbilityPositions[4 - i - 1] = v3Temp;
        }
    }


    public void OnSelectChar(Chr _chrSelected) {
        chrSelected = _chrSelected;

        if(nCurrentShowingPlyrID != chrSelected.plyrOwner.id) {
            nCurrentShowingPlyrID = chrSelected.plyrOwner.id;

            ReverseAbilityPanel();

            fXOffsetMultiplier *= -1;

            for(int i = 0; i < 4; i++) {
                arViewAction[i].transform.localScale = new Vector3(fXOffsetMultiplier, 1.0f, 1.0f);
            }
            this.transform.localScale = new Vector3(fXOffsetMultiplier, 1.0f, 1.0f);

        }

        this.transform.position = new Vector3(
                chrSelected.transform.position.x + fXOffset * fXOffsetMultiplier,
                chrSelected.transform.position.y + fYOffset,
                chrSelected.transform.position.z
                );

        //Notify each ability 
        for(int i = 0; i < arViewAction.Length; i++) {
            //Move each ability to its correct (possibly flipped) location
            arViewAction[i].transform.localPosition = arV3AbilityPositions[i];

            if(chrSelected != null) {
                arViewAction[i].SetModel(chrSelected.arSkillSlots[i].skill);
            } else {
                arViewAction[i].SetModel(null);
            }
        }

    }

    public void Deselect(Chr chrUnselected) {
        //Debug.Log("Calling deselect for " + chrUnselected);
        if(chrSelected != chrUnselected)
            //Don't do anything if we're unselecting a character that we're not currently selecting (probably some race condition
            return;


        //Move the panel offscreen
        this.transform.position = v3Offscreen;

        //Notify each ability 
        for(int i = 0; i < arViewAction.Length; i++) {
            arViewAction[i].SetModel(null);
        }
    }

    public void Start() {
        if(bStarted == false) {
            bStarted = true;

            //Move the panel offscreen
            this.transform.position = v3Offscreen;

            //Start each ability as representing nothing 
            for(int i = 0; i < arViewAction.Length; i++) {
                arViewAction[i].SetModel(null);
                arV3AbilityPositions[i] = arViewAction[i].transform.localPosition;
            }

            Chr.subAllStartSelect.Subscribe(cbChrSelected);
            Chr.subAllStartIdle.Subscribe(cbChrUnselected);
        }
    }
}
