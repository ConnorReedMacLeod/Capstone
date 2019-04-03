using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Chr))]
public class ViewChr : ViewInteractive {

	bool bStarted;                          //Confirms the Start() method has executed
    
    public enum PortraitState {
        IDLE, ACTING, INJURED
    };

    public PortraitState statePortrait; //The portrait state the character is currently in

    public bool bRecoiling;             //Targetted by an ability recently
    public float fCurRecoilTime;
    public const float fMaxRecoilTime = 0.4f;
    public Vector3 v3BasePosition;
    public const float fMaxRecoilDistance = 0.12f;
    public Vector3 v3RecoilDirection;
    public const int nRecoilShakes = 3;
    public const float fRecoilSpeed = fMaxRecoilDistance / (fMaxRecoilTime / (nRecoilShakes * 4));

    public bool bSelectingChrTargettable;  //If we're in the middle of selecting some character and this would be valid to select
    public bool bSelectingTeamTargettable;  //If we're in the middle of selecting some character and this would be valid to select

    public Chr mod;                   //Character model

	Chr.STATESELECT lastStateSelect;  //Tracks previous character state (SELECTED, TARGETTING, UNSELECTED)

    public GameObject pfBlockerIndicator; //Reference to the prefab blocker indicator
    public GameObject pfSelectionGlow; //Reference to the prefab glow for selection

    public GameObject goCurSelectionGlow; //A reference to the current glow (or null if there is none)
    public GameObject goBlockerIndicator; //Blocker Reference
	public GameObject goBorder;         //Border Reference
	public GameObject goPortrait;       //Portrait Reference
	public GameObject goFatigueDisplay; //Fatigue Display Reference
	public GameObject goChannelDisplay; //Channel Display Reference
	public GameObject goPowerDefense;	//Power Defense Display Reference
	public GameObject goPowerDisplay;   //Power Display Reference
	public GameObject goDefenseDisplay; //Defense Display Reference

	private Vector3 v3FatiguePosition;  //Fatigue Display Position
	private Vector3 v3ChannelPosition;	//Channel Display Position
	private Vector3 v3PowerPosition;    //Power Display Position
	private Vector3 v3DefensePosition;	//Defense Display Position

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

            v3BasePosition = goPortrait.transform.localPosition;
            v3RecoilDirection = Vector3.left;

            StateTargetChr.subAllStartSelection.Subscribe(cbStartTargettingChr);
            StateTargetChr.subAllFinishSelection.Subscribe(cbStopTargettingChr);

            StateTargetTeam.subAllStartSelection.Subscribe(cbStartTargettingTeam);
            StateTargetTeam.subAllFinishSelection.Subscribe(cbStopTargettingTeam);

        }
    }

	public void Init(){
		SetPortrait ();
		if (mod.plyrOwner.id == 1) {
			//Find the portrait and flip it for one of the players
			goPortrait.transform.localScale = new Vector3 (-0.5f, 0.5f, 1.0f);

			//Find the border and flip it for one of the players
			//goBorder.transform.localScale = new Vector3(1.33f, -1.33f, 1.0f);

			//Find the fatigue display, and flip it to the other side of the portrai
			goFatigueDisplay.transform.localPosition = new Vector3(
				-goFatigueDisplay.transform.localPosition.x,
				goFatigueDisplay.transform.localPosition.y,
				goFatigueDisplay.transform.localPosition.z);

			goChannelDisplay.transform.localPosition = new Vector3(
				-goChannelDisplay.transform.localPosition.x,
				goChannelDisplay.transform.localPosition.y,
				goChannelDisplay.transform.localPosition.z);

			goPowerDefense.transform.localPosition = new Vector3(
				-goPowerDefense.transform.localPosition.x,
				goPowerDefense.transform.localPosition.y,
				goPowerDefense.transform.localPosition.z);

			/*goPowerDisplay.transform.localPosition = new Vector3(
				-goPowerDisplay.transform.localPosition.x,
				goPowerDisplay.transform.localPosition.y,
				goPowerDisplay.transform.localPosition.z);*/

			/*goDefenseDisplay.transform.localPosition = new Vector3(
				-goDefenseDisplay.transform.localPosition.x,
				goDefenseDisplay.transform.localPosition.y,
				goDefenseDisplay.transform.localPosition.z);*/

			//Flip the character's soul position as well
			viewSoulContainer.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);

            foreach (ViewSoul viewsoul in viewSoulContainer.arViewSoul) {
                viewsoul.transform.localScale = new Vector3(-0.666f, 0.666f, 1.0f);
            }

        }
		//Fatigue and Channel positioning
		v3FatiguePosition = goFatigueDisplay.transform.localPosition;
		v3ChannelPosition = goChannelDisplay.transform.localPosition;

		goFatigueDisplay.transform.localPosition = new Vector3(-100.0f, -100.0f, -100.0f);
		goChannelDisplay.transform.localPosition = new Vector3(-100.0f, -100.0f, -100.0f);

		//Power and Defense positioning
		v3PowerPosition = goPowerDisplay.transform.localPosition;
		v3DefensePosition = goDefenseDisplay.transform.localPosition;

		goPowerDisplay.transform.localPosition = new Vector3(-100.0f, -100.0f, -100.0f);
		goDefenseDisplay.transform.localPosition = new Vector3(-100.0f, -100.0f, -100.0f);
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
    void SetPortrait(){
		string sSprPath = "Images/Chrs/" + mod.sName + "/img" + mod.sName + "Neutral";

        switch (statePortrait) {
            case PortraitState.ACTING:
                sSprPath = "Images/Chrs/" + mod.sName + "/img" + mod.sName + "Action";
                break;

            case PortraitState.INJURED:
                sSprPath = "Images/Chrs/" + mod.sName + "/img" + mod.sName + "Hurt";
                break;

            default:

                break;
        }

		Sprite sprChr = Resources.Load(sSprPath, typeof(Sprite)) as Sprite;

        if(sprChr == null) {
            Debug.LogError("Could not load " + sSprPath);
            sSprPath = "Images/Chrs/" + mod.sName + "/img" + mod.sName + "Neutral";
            sprChr = Resources.Load(sSprPath, typeof(Sprite)) as Sprite;
        }

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
        mod.subPreExecuteAbility.Subscribe(cbStartUsingAbility);
        mod.subPostExecuteAbility.Subscribe(cbStopUsingAbility);

        mod.subLifeChange.Subscribe(cbRecoil);
        mod.subSoulApplied.Subscribe(cbSoulApplied);
        mod.subStunApplied.Subscribe(cbRecoil);
        mod.subLifeChange.Subscribe(cbOnInjured);

        ContTurns.Get().subNextActingChrChange.Subscribe(cbOnNextActingChange);
    }

    public void cbOnNextActingChange(Object target, params object[] args) {

        DecideIfHighlighted(mod);

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
            /*Debug.Log("We shouldn't show fatigue when channeling");
            txtFatigue.text = "";
			goFatigueDisplay.transform.localPosition = new Vector3(-100.0f, -100.0f, -100.0f);
			return;*/
        }

        if (mod.curStateReadiness.Type() == StateReadiness.TYPE.DEAD) {
            Debug.Log("We shouldn't show fatigue when dead");
            txtFatigue.text = "";
			goFatigueDisplay.transform.localPosition = new Vector3(-100.0f, -100.0f, -100.0f);
			return;
        }

        //Otherwise, then show a non-zero fatigue value
        if (mod.nFatigue > 0) {
            txtFatigue.text = mod.nFatigue.ToString();
			goFatigueDisplay.transform.localPosition = v3FatiguePosition;
        } else {
            txtFatigue.text = "";
			goFatigueDisplay.transform.localPosition = new Vector3(-100.0f, -100.0f, -100.0f);
        }
    }

    public void cbUpdateChannelTime(Object target, params object[] args) {
        //If we're not channeling, then we won't display anything
        if (mod.curStateReadiness.Type() != StateReadiness.TYPE.CHANNELING) {
            //Debug.Log("Were notified of UpdateChannelTime, but we're not in a channeling state");
            txtChannelTime.text = "";
			goChannelDisplay.transform.localPosition = new Vector3(-100.0f, -100.0f, -100.0f);
			return;

        }

        //Otherwise, then the channeltime value
        if (((StateChanneling)mod.curStateReadiness).nChannelTime > 0) {
            txtChannelTime.text = ((StateChanneling)mod.curStateReadiness).nChannelTime.ToString();
			goChannelDisplay.transform.localPosition = v3ChannelPosition;
		} else {
            txtChannelTime.text = "";
			goChannelDisplay.transform.localPosition = new Vector3(-100.0f, -100.0f, -100.0f);
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
            txtPower.text = "+" + mod.pnPower.Get().ToString();
			goPowerDisplay.transform.localPosition = v3PowerPosition;
		} else if (mod.pnPower.Get() < 0) {
            txtPower.text = mod.pnPower.Get().ToString();
			goPowerDisplay.transform.localPosition = v3PowerPosition;
		} else {
            txtPower.text = "";
			goPowerDisplay.transform.localPosition = new Vector3(-100.0f, -100.0f, -100.0f);
		}
    }

    public void cbUpdateDefense(Object target, params object[] args) {
        if (mod.pnDefense.Get() > 0) {
            txtDefense.text = "+" + mod.pnDefense.Get().ToString();
			goDefenseDisplay.transform.localPosition = v3DefensePosition;
		} else if (mod.pnDefense.Get() < 0) {
            txtDefense.text = mod.pnDefense.Get().ToString();
			goDefenseDisplay.transform.localPosition = v3DefensePosition;
		} else {
            txtDefense.text = "";
			goDefenseDisplay.transform.localPosition = new Vector3(-100.0f, -100.0f, -100.0f);
        }
    }

    public void cbUpdateBlocker(Object target, params object[] args) {
        if (mod.bBlocker == true) {
            //If we haven't already, add the blocker indicator
            if (goBlockerIndicator == null) {
                goBlockerIndicator = Instantiate(pfBlockerIndicator, this.transform);

                if (mod.plyrOwner.id == 1 || mod.plyrOwner.id == 0) {
                    goBlockerIndicator.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    goBlockerIndicator.transform.localPosition =
                        new Vector3(goBlockerIndicator.transform.localPosition.x,
                                    goBlockerIndicator.transform.localPosition.y - 3.06f,
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

        DecideIfHighlighted(tarArg.chrOwner);

    }

    public void cbStopTargettingChr(Object target, params object[] args) {

        if (bSelectingChrTargettable) {
            bSelectingChrTargettable = false; 
        }

        DecideIfHighlighted(null);

    }

    public void cbStartTargettingTeam(Object target, params object[] args) {

        TargetArgTeam tarArg = (TargetArgTeam)args[0];


        if (tarArg.WouldBeLegal(mod.plyrOwner.id)) {
            bSelectingTeamTargettable = true;
        }

        DecideIfHighlighted(tarArg.chrOwner);

    }

    public void cbStopTargettingTeam(Object target, params object[] args) {

        bSelectingTeamTargettable = false;

        DecideIfHighlighted(null);

    }

    public void cbStartUsingAbility(Object target, params object[] args) {


        if (((Action)args[0]).bProperActive == true) {
            statePortrait = PortraitState.ACTING;

            SetPortrait();
        }

    }

    public void cbStopUsingAbility(Object target, params object[] args) {

        statePortrait = PortraitState.IDLE;

        SetPortrait();

    }

    public void cbOnInjured(Object target, params object[] args) {
        //First, double check that we're actually losing health
        if ((int)args[0] >= 0) return;

        statePortrait = PortraitState.INJURED;

        SetPortrait();

    }

    public void cbRecoil(Object target, params object[] args) {

        //Debug.Log(mod.sName + " is recoiling");

        bRecoiling = true;
        fCurRecoilTime = 0f;

    }

    public void cbSoulApplied(Object target, params object[] args) {

        if (((Soul)args[0]).bRecoilWhenApplied == false) return;

        //Debug.Log(mod.sName + " is recoiling");

        bRecoiling = true;
        fCurRecoilTime = 0f;

    }

    public void DecideIfHighlighted(Chr chrActing) {

        if (bSelectingChrTargettable || bSelectingTeamTargettable) {
            Debug.Log(mod.sName + " can be targetted");
            if (goCurSelectionGlow == null) {
                goCurSelectionGlow = Instantiate(pfSelectionGlow, maskPortrait.transform);
            }

            //By default, assume the character is an enemy
            string sSprPath = "Images/Chrs/imgGlow6";

            //But if they're a friend, then make it a green border
            if (chrActing.plyrOwner.id == mod.plyrOwner.id) {
                sSprPath = "Images/Chrs/imgGlow4";
            }

            Sprite sprGlow = Resources.Load(sSprPath, typeof(Sprite)) as Sprite;

            Debug.Assert(sprGlow != null, "Could not find specificed sprite: " + sSprPath);

            goCurSelectionGlow.GetComponent<SpriteRenderer>().sprite = sprGlow;
            
        } else if (mod == ContTurns.Get().GetNextActingChr()) {
            Debug.Log(mod.sName + " is next to act");

            if (goCurSelectionGlow == null) {
                goCurSelectionGlow = Instantiate(pfSelectionGlow, maskPortrait.transform);
            }

            //Set the colour of the glow to add to the character
            string sSprPath = "Images/Chrs/imgGlow";

            Sprite sprGlow = Resources.Load(sSprPath, typeof(Sprite)) as Sprite;

            Debug.Assert(sprGlow != null, "Could not find specificed sprite: " + sSprPath);

            goCurSelectionGlow.GetComponent<SpriteRenderer>().sprite = sprGlow;
            
        } else { 
           if(goCurSelectionGlow != null) {
                Destroy(goCurSelectionGlow);
                goCurSelectionGlow = null;
            }
        }

    }

    public void UpdateRecoil() {
        if (bRecoiling == false) return;

        fCurRecoilTime += ContTime.Get().fDeltaTime;
        goPortrait.transform.localPosition += fRecoilSpeed * v3RecoilDirection * ContTime.Get().fDeltaTime;

        //Debug.Log("x coord is " + goPortrait.transform.localPosition.x);

        //If we've moved past the left boundary
        if (v3RecoilDirection.x >= 0 && goPortrait.transform.localPosition.x >= v3BasePosition.x + fMaxRecoilDistance){
            //Make sure we don't move too far past the edge
            goPortrait.transform.localPosition = new Vector3
                (v3BasePosition.x + fMaxRecoilDistance,
                goPortrait.transform.localPosition.y,
                goPortrait.transform.localPosition.z);

            //Reverse the direction
            v3RecoilDirection *= -1;
            //Debug.Log("reversing");

        } else if(v3RecoilDirection.x < 0 && goPortrait.transform.localPosition.x <= v3BasePosition.x - fMaxRecoilDistance) {
            //Make sure we don't move too far past the edge
            goPortrait.transform.localPosition = new Vector3
                (v3BasePosition.x - fMaxRecoilDistance,
                goPortrait.transform.localPosition.y,
                goPortrait.transform.localPosition.z);

            //Reverse the direction
            v3RecoilDirection *= -1;
            //Debug.Log("reversing");
        }


        if(fCurRecoilTime > fMaxRecoilTime) {
            //Debug.Log(mod.sName + " is ending recoil");
            bRecoiling = false;
            fCurRecoilTime = 0f;

            //Ensure the position is now equal to the original base position
            goPortrait.transform.localPosition = v3BasePosition;

            //And reset our sate back to idle
            statePortrait = PortraitState.IDLE;
            SetPortrait();
        }

    }

    public void Update() {

        UpdateRecoil();

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
