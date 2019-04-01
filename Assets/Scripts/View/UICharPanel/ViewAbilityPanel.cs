using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewAbilityPanel : ViewInteractive {

    public bool bStarted = false;

    public Chr chrSelected;

    public ViewAction [] arViewAction= new ViewAction[4];

    public Vector3 v3Offscreen = new Vector3(-100f, -100f, 0f);

	private Vector3 v3AbilitySlot0;
	private Vector3 v3AbilitySlot1;
	private Vector3 v3AbilitySlot2;
	private Vector3 v3AbilitySlot3;

	private bool bFlippedIcon = false;

	public void cbChrSelected(Object target, params object[] args) {
        OnSelectChar((Chr)target);
    }

    public void cbChrUnselected(Object target, params object[] args) {
        Deselect((Chr)target);
    }

    


    public void OnSelectChar(Chr _chrSelected) {
        chrSelected = _chrSelected;

		//Position the panel next to the selected character
		if (chrSelected.plyrOwner.id == 0) {
			this.transform.position = new Vector3(
				chrSelected.transform.position.x + 2.1f,
				chrSelected.transform.position.y + 0.35f,
				chrSelected.transform.position.z
				);

			if (bFlippedIcon) {
				for (int i = 0; i < 4; i++) {
					arViewAction[i].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
				}

				this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
				bFlippedIcon = false;
			}


			arViewAction[0].transform.localPosition = v3AbilitySlot0;
			arViewAction[1].transform.localPosition = v3AbilitySlot1;
			arViewAction[2].transform.localPosition = v3AbilitySlot2;
			arViewAction[3].transform.localPosition = v3AbilitySlot3;

		} else if (chrSelected.plyrOwner.id == 1) {
			this.transform.position = new Vector3(
			chrSelected.transform.position.x - 2.1f,
			chrSelected.transform.position.y + 0.35f,
			chrSelected.transform.position.z
			);

			if (!bFlippedIcon) {
				for (int i = 0; i < 4; i++) {
					arViewAction[i].transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
				}
				this.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
				bFlippedIcon = true;
			}

			arViewAction[0].transform.localPosition = v3AbilitySlot3;
			arViewAction[1].transform.localPosition = v3AbilitySlot2;
			arViewAction[2].transform.localPosition = v3AbilitySlot1;
			arViewAction[3].transform.localPosition = v3AbilitySlot0;

		}

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

			v3AbilitySlot0 = arViewAction[0].transform.localPosition;
			v3AbilitySlot1 = arViewAction[1].transform.localPosition;
			v3AbilitySlot2 = arViewAction[2].transform.localPosition;
			v3AbilitySlot3 = arViewAction[3].transform.localPosition;

			Chr.subAllStartSelect.Subscribe(cbChrSelected);
            Chr.subAllStartIdle.Subscribe(cbChrUnselected);
        }
    }
}
