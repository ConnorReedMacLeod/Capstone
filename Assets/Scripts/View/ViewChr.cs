﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Chr))]
public class ViewChr : ViewInteractive {

	bool bStarted;                          //Confirms the Start() method has executed

	public Chr mod;                   //Character model

	Chr.STATESELECT lastStateSelect;  //Tracks previous character state (SELECTED, TARGETTING, UNSELECTED)
	Vector3 v3LastPos;                      //Tracks previous character position against the model position

	public GameObject goBorder;        //Border reference
	public GameObject goPortrait;       //Portrait Reference
    public Text txtHealth;              //Textfield Reference
    public SpriteMask maskPortrait;     //SpriteMask Reference

    public static Subject subAllStartHover = new Subject();
    public static Subject subAllStopHover = new Subject();
    public static Subject subAllClick = new Subject();

    public void Start()
    {
		if (bStarted == false) {
			bStarted = true;

			// Find our model
			InitModel ();
			lastStateSelect = Chr.STATESELECT.IDLE;

		}
    }

    public void InitOpponentCharacter() {
        //Let the Arena position ourselves as an enemy
        Arena.Get().InitPlaceUnit(false, this);

        //Find the portrait and flip it
        goPortrait.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);

        //Find the border and flip it
        goBorder.transform.localScale = new Vector3(1.33f, -1.33f, 1.0f);

        //Flip and position the mask
        maskPortrait.transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);
        maskPortrait.transform.localPosition =
            new Vector3(maskPortrait.transform.localPosition.x,
                        -maskPortrait.transform.localPosition.y,
                        maskPortrait.transform.localPosition.z);

        txtHealth.transform.localPosition =
            new Vector3(txtHealth.transform.localPosition.x,
                        -txtHealth.transform.localPosition.y,
                        txtHealth.transform.localPosition.z);

    }

    public void InitLocalCharacter() {
        //Let the Arena position ourselves as a locally owned player
        Arena.Get().InitPlaceUnit(true, this);


    }

	public void Init(){

		SetPortrait (mod.sName);

        //Check if we're owned by the local player
        Debug.Log("mod is " + mod);
        Debug.Log("mod.plyrOwner is " + mod.plyrOwner);
        Debug.Log("local id is " + Player.idLocal); // <<<<--- BUG CAUSING
        if(mod.plyrOwner != null && mod.plyrOwner.id == Player.idLocal) {
            //If the owner of this player is already registered, and it's our local player
            InitLocalCharacter();
        } else {
            //Then either the player registered is the opponent, or no one has registered
            //this character (which could only happen for an opponent)
            InitOpponentCharacter();
        }
	}

    public override void onMouseClick(params object[] args) {
        //Currently not doing anything - just passing along the notification
        
        subAllClick.NotifyObs(this, args);

        base.onMouseClick(args);
    }

    public override void onMouseDoubleClick(params object[] args) {

        //just do the same thing as a normal click
        onMouseClick(args);

        base.onMouseDoubleClick(args);
    }

    public override void onMouseStartHover(params object[] args) {
        //Currently not doing anything - just passing along the notification to a global notification

        subAllStartHover.NotifyObs(this, args);

        base.onMouseStartHover(args);
    }

    public override void onMouseStopHover(params object[] args) {
        //Currently not doing anything - just passing along the notification to a global notification

        subAllStopHover.NotifyObs(this, args);

        base.onMouseStopHover(args);
    }


    //Sets the sprite used for the character's full picture portrait
    void SetPortrait(string _sName){
		string sSprPath = "Images/Chrs/" + _sName + "/img" + _sName + "Portrait";
		Sprite sprChr = Resources.Load(sSprPath, typeof(Sprite)) as Sprite;

        goPortrait.GetComponent<SpriteRenderer>().sprite = sprChr;

	}
    
    //Sets the sprite used for the character's border
	void SetBorder(string _sName){
		string sSprPath = "Images/Chrs/img" + _sName;
		Sprite sprBorder = Resources.Load(sSprPath, typeof(Sprite)) as Sprite;

        goBorder.GetComponent<SpriteRenderer>().sprite = sprBorder;
	}

    //Find the model, and do any setup to reflect it
	public void InitModel(){
		mod = GetComponent<Chr>();
		mod.subHealthChange.Subscribe(cbUpdateTxtHealth);
        mod.subStatusChange.Subscribe(cbUpdateStatus);

	}

    public void cbUpdateTxtHealth(Object target, params object[] args) {
        txtHealth.text = mod.nCurHealth + "/" + mod.nMaxHealth;
    }


	//TODO:: Make this a state machine
    //Updates the character's state (SELECTED, TARGETTING, UNSELECTED)
	void cbUpdateStatus(Object target, params object[] args) {

		//Checks if character status has changed
		if (lastStateSelect != mod.stateSelect) {
			switch (mod.stateSelect) {

			//On switch to selection, highlight the border
			case Chr.STATESELECT.SELECTED:
				SetBorder ("ChrBorderSelected");

				break;

            //On switch to targetting, despawns the ActionWheel
			case Chr.STATESELECT.TARGGETING:
				//RemoveActionWheel ();
				break;

            //On switch to unselected, make changes depending on previous state
			case Chr.STATESELECT.IDLE:

				if (lastStateSelect == Chr.STATESELECT.TARGGETING) {
					//Nothing needs to be done (currently, this may change)

				} else if (lastStateSelect == Chr.STATESELECT.SELECTED) {
                        //Nothing needs to be done (currently, this may change)
                }
                 //Then unhighlight the border
                SetBorder("ChrBorder");
                 break;
            
            //Catches unrecognized character states
			default: 
				Debug.LogError ("UNRECOGNIZED VIEW CHR SELECT STATE!");
				return;
			}
			lastStateSelect = mod.stateSelect;
		}
	}
}
