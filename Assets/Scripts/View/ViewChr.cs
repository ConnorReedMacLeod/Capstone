using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Chr))]
public class ViewChr : ViewInteractive {

	bool bStarted;                          //Confirms the Start() method has executed
    
    public bool bSelectingChrTargettable;  //If we're in the middle of selecting some character and this would be valid to select
    public bool bSelectingTeamTargettable;  //If we're in the middle of selecting some character and this would be valid to select

    public Chr mod;                   //Character model

	Chr.STATESELECT lastStateSelect;  //Tracks previous character state (SELECTED, TARGETTING, UNSELECTED)

    public GameObject pfBlockerIndicator; //Reference to the prefab blocker indicator

    public GameObject goBlockerIndicator; //Blocker reference
	public GameObject goBorder;         //Border reference
	public GameObject goPortrait;       //Portrait Reference
    public Text txtHealth;              //Textfield Reference
    public Text txtArmour;              //Textfield Reference
    public Text txtPower;               //Textfield Reference
    public Text txtDefense;             //Textfield Reference
    public Text txtFatigue;             //Fatigue Overlay Reference
    public Text txtChannelTime;         //ChannelTime Overlay Reference
    public SpriteMask maskPortrait;     //SpriteMask Reference
    public ViewSoulContainer viewSoulContainer;  //SoulContainer Reference

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

            StateTargetChr.subAllStartSelection.Subscribe(cbStartTargettingChr);
            StateTargetChr.subAllFinishSelection.Subscribe(cbStopTargettingChr);

            StateTargetTeam.subAllStartSelection.Subscribe(cbStartTargettingTeam);
            StateTargetTeam.subAllFinishSelection.Subscribe(cbStopTargettingTeam);

        }
    }

	public void Init(){
		SetPortrait (mod.sName);
		if (mod.plyrOwner.id == 1) {
			//Find the portrait and flip it for one of the players
			goPortrait.transform.localScale = new Vector3 (-1.0f, 1.0f, 1.0f);

            //Find the border and flip it for one of the players
            goBorder.transform.localScale = new Vector3(1.33f, -1.33f, 1.0f);

            //Flip the character's soul position as well
            viewSoulContainer.transform.localScale = new Vector3(1.33f, -1.33f, 1.0f);

            foreach (ViewSoul viewsoul in viewSoulContainer.arViewSoul) {
                viewsoul.transform.localScale = new Vector3(1.33f, -1.33f, 1.0f);
            }

            maskPortrait.transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);
            maskPortrait.transform.localPosition = 
                new Vector3(maskPortrait.transform.localPosition.x, 
                            - maskPortrait.transform.localPosition.y,
                            maskPortrait.transform.localPosition.z);

            txtHealth.transform.localPosition  = 
                new Vector3(txtHealth.transform.localPosition.x, 
                            -txtHealth.transform.localPosition.y,
                            txtHealth.transform.localPosition.z);

            txtArmour.transform.localPosition =
                new Vector3(txtArmour.transform.localPosition.x,
                            -txtArmour.transform.localPosition.y,
                            txtArmour.transform.localPosition.z);

            txtPower.transform.localPosition =
                new Vector3(txtPower.transform.localPosition.x,
                            -txtPower.transform.localPosition.y,
                            txtPower.transform.localPosition.z);

            txtDefense.transform.localPosition =
                new Vector3(txtDefense.transform.localPosition.x,
                            -txtDefense.transform.localPosition.y,
                            txtDefense.transform.localPosition.z);

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

        Debug.Assert(sprChr != null, "Could not find specificed sprite: " + sSprPath);

        goPortrait.GetComponent<SpriteRenderer>().sprite = sprChr;

	}
    
    //Sets the sprite used for the character's border
	void SetBorder(string _sName){
		string sSprPath = "Images/Chrs/img" + _sName;
		Sprite sprBorder = Resources.Load(sSprPath, typeof(Sprite)) as Sprite;

        Debug.Assert(sprBorder != null, "Could not find specificed sprite: " + sSprPath);

        goBorder.GetComponent<SpriteRenderer>().sprite = sprBorder;
	}

    //Find the model, and do any setup to reflect it
	public void InitModel(){
        mod = GetComponent<Chr>();
        mod.Start();

        mod.subFatigueChange.Subscribe(cbUpdateFatigue);
		mod.subLifeChange.Subscribe(cbUpdateLife);
        mod.pnMaxHealth.subChanged.Subscribe(cbUpdateLife);
        mod.subStatusChange.Subscribe(cbUpdateStatus);
        mod.pnArmour.subChanged.Subscribe(cbUpdateArmour);
        mod.pnPower.subChanged.Subscribe(cbUpdatePower);
        mod.pnDefense.subChanged.Subscribe(cbUpdateDefense);
        mod.subBlockerChanged.Subscribe(cbUpdateBlocker);
        mod.subChannelTimeChange.Subscribe(cbUpdateChannelTime);
        mod.subDeath.Subscribe(cbUpdateDeath);
    }

    public void cbUpdateDeath(Object target, params object[] args) {

        if (mod.bDead) {
            //If the character is dead, then red-out their portrait
            goPortrait.GetComponent<SpriteRenderer>().color = Color.red;
        } else {
            //If the character is still alive, then keep their portrait normal
            goPortrait.GetComponent<SpriteRenderer>().color = Color.white;
        }

    }

    public void cbUpdateLife(Object target, params object[] args) {
        txtHealth.text = mod.nCurHealth + "/" + mod.pnMaxHealth.Get();
    }

    public void cbUpdateFatigue(Object target, params object[] args) {
        //If we're channeling, then we won't display fatigue
        if (mod.curStateReadiness.Type() == StateReadiness.TYPE.CHANNELING) {
            Debug.Log("We shouldn't show fatigue when channeling");
            txtFatigue.text = "";
            return;
        }

        if (mod.curStateReadiness.Type() == StateReadiness.TYPE.DEAD) {
            Debug.Log("We shouldn't show fatigue when dead");
            txtFatigue.text = "";
            return;
        }

        //Otherwise, then show a non-zero fatigue value
        if (mod.nFatigue > 0) {
            txtFatigue.text = mod.nFatigue.ToString();
        } else {
            txtFatigue.text = "";
        }
    }

    public void cbUpdateChannelTime(Object target, params object[] args) {
        //If we're not channeling, then we won't display anything
        if (mod.curStateReadiness.Type() != StateReadiness.TYPE.CHANNELING) {
            //Debug.Log("Were notified of UpdateChannelTime, but we're not in a channeling state");
            txtChannelTime.text = "";
            return;

        }

        //Otherwise, then the channeltime value
        if (((StateChanneling)mod.curStateReadiness).nChannelTime > 0) {
            txtChannelTime.text = ((StateChanneling)mod.curStateReadiness).nChannelTime.ToString();
        } else {
            txtChannelTime.text = "";
        }
    }

    public void cbUpdateArmour(Object target, params object[] args) {
        if (mod.pnArmour.Get() > 0) {
            txtArmour.text = "[" + mod.pnArmour.Get().ToString() + "]";
        } else {
            txtArmour.text = "";
        }
    }

    public void cbUpdatePower(Object target, params object[] args) {
        if (mod.pnPower.Get() > 0) {
            txtPower.text = "+" + mod.pnPower.Get().ToString() + " [POWER]";
        } else if (mod.pnPower.Get() < 0) {
            txtPower.text = mod.pnPower.Get().ToString() + " [POWER]";
        } else {
            txtPower.text = "";
        }
    }

    public void cbUpdateDefense(Object target, params object[] args) {
        if (mod.pnDefense.Get() > 0) {
            txtDefense.text = "+" + mod.pnDefense.Get().ToString() + " [DEFENSE]";
        } else if (mod.pnDefense.Get() < 0) {
            txtDefense.text = mod.pnDefense.Get().ToString() + " [DEFENSE]";
        } else {
            txtDefense.text = "";
        }
    }

    public void cbUpdateBlocker(Object target, params object[] args) {
        if (mod.bBlocker == true) {
            //If we haven't already, add the blocker indicator
            if (goBlockerIndicator == null) {
                goBlockerIndicator = Instantiate(pfBlockerIndicator, this.transform);

                if (mod.plyrOwner.id == 1) {
                    goBlockerIndicator.transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);
                    goBlockerIndicator.transform.localPosition =
                        new Vector3(goBlockerIndicator.transform.localPosition.x,
                                    -goBlockerIndicator.transform.localPosition.y,
                                    goBlockerIndicator.transform.localPosition.z);
                }

            } else {
                Debug.Log("Don't need to add a blocker indicator for " + mod.sName + " since they already have it shown");
            }
        } else {
            //If we haven't already, then remove the blocker indicator
            if(goBlockerIndicator != null) {
                GameObject.Destroy(goBlockerIndicator);
                goBlockerIndicator = null;
                Debug.Assert(goBlockerIndicator == null);
            } else {
                Debug.Log("Don't need to remove a blocker indicator for " + mod.sName + " since nothing is yet shown");
            }
        }
    }



    public void cbStartTargettingChr(Object target, params object[] args) {

        TargetArgChr tarArg = (TargetArgChr)args[0];


        if (tarArg.WouldBeLegal(mod.globalid)) {
            bSelectingChrTargettable = true;
        }

        DecideIfHighlighted();

    }

    public void cbStopTargettingChr(Object target, params object[] args) {

        if (bSelectingChrTargettable) {
            bSelectingChrTargettable = false; 
        }

        DecideIfHighlighted();

    }

    public void cbStartTargettingTeam(Object target, params object[] args) {

        TargetArgTeam tarArg = (TargetArgTeam)args[0];


        if (tarArg.WouldBeLegal(mod.plyrOwner.id)) {
            bSelectingTeamTargettable = true;
        }

        DecideIfHighlighted();

    }

    public void cbStopTargettingTeam(Object target, params object[] args) {

        if (bSelectingTeamTargettable) {
            bSelectingTeamTargettable = false;
        }

        DecideIfHighlighted();

    }


    public void DecideIfHighlighted() {

        if(bSelectingChrTargettable || bSelectingTeamTargettable) {
            Debug.Log("Should be highlighted for " + mod.sName);
        } else {
            Debug.Log("Should not be highlighted for " + mod.sName);
        }

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
