using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Chr))]
[RequireComponent (typeof(MouseHandler))]
public class ViewChr : Observer {

	bool bStarted;                          //Confirms the Start() method has executed

	public Chr mod;                   //Character model
	public MouseHandler mousehandler;

	Chr.STATESELECT lastStateSelect;  //Tracks previous character state (SELECTED, TARGETTING, UNSELECTED)
	Vector3 v3LastPos;                      //Tracks previous character position against the model position

	public GameObject goBorder;        //Border reference
	public GameObject goPortrait;       //Portrait Reference

    /*
	public float fOrigRadius;
	public float fOrigWidth;
	public float fOrigHeight;
	public float fOrigScaleX;
	public float fOrigScaleY;
    */		

    public void Start()
    {
		if (bStarted == false) {
			bStarted = true;

			// Find our model
			InitModel ();
			//Unscale ();
			lastStateSelect = Chr.STATESELECT.IDLE;
			InitMouseHandler ();

            /*
			//Get the base size of the character prefab so we can scale it later
			fOrigWidth = GetComponent<Collider>().bounds.size.x;
			fOrigHeight = GetComponent<Collider>().bounds.size.y;
			fOrigRadius = GetComponent<CapsuleCollider> ().radius;
			fOrigScaleX = transform.localScale.x;
			fOrigScaleY = transform.localScale.y;
            */
		}
    }

	public void Init(){
		setPortrait (mod.sName);
		if (mod.plyrOwner.id == 1) {
			//Find the portrait and flip it for one of the players
			goPortrait.transform.localScale = new Vector3 (-1.33f, 1.33f, 1.33f);

            //Find the border and flip it for one of the players
            goBorder.transform.localScale = new Vector3(1.33f, -1.33f, 1.33f);
        }
	}

	public void InitMouseHandler(){
		mousehandler = GetComponent<MouseHandler> ();
		mousehandler.SetOwner (this);

		mousehandler.SetNtfClick (Notification.ClickChr);
		mousehandler.SetNtfDoubleClick (Notification.ClickChr);

		mousehandler.SetNtfStartHold (Notification.ChrStartHold);

		mousehandler.SetNtfStopHold (Notification.ChrStopHold);

		//mousehandler.SetReleaseOtherCallback (ReleaseOverOther);
	}

    

    //Sets the sprite used for the character's portrait
	void setPortrait(string _sName){
		string sSprPath = "Images/Chrs/img" + _sName;
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
		mod.Subscribe (this);

	}


	//TODO:: Make this a state machine
    //Updates the character's state (SELECTED, TARGETTING, UNSELECTED)
	void UpdateStatus(){
		//Refuses to accept udpates until after initialized
		if (!bStarted)
			return;

		//UpdateSize ();

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

    //Updates character view, detecting if changes are needed to the character position or state
	override public void UpdateObs(string eventType, Object target, params object[] args){
		//TODO:: Make this update more intelligent so that it updates based on the passed eventType
		UpdateStatus();
	}



    //UNUSED
    /*
    //Spawns the ActionWheel
    void AddActionWheel() {
        Debug.Assert(objActionWheel == null);
        Debug.Assert(viewActionWheel == null);
        objActionWheel = Instantiate(pfActionWheel, transform);
        ///viewActionWheel = objActionWheel.AddComponent<ViewActionWheel> ();
        viewActionWheel = objActionWheel.GetComponent<ViewActionWheel>();
        viewActionWheel.setModel(mod);
    }

    //Despawns the ActionWheel
    void RemoveActionWheel() {
        Debug.Assert(objActionWheel != null);
        Debug.Assert(viewActionWheel != null);
        Destroy(objActionWheel);
        objActionWheel = null;
        viewActionWheel = null;
    }
    */
    /*
	public void UpdateSize(){
		// Check the model's desired radius, so we can match that

		float fRelativeScale = fOrigRadius / mod.fRad;

		transform.localScale = new Vector3 (fOrigScaleX / fRelativeScale, fOrigScaleY / fRelativeScale, 1.0f);
		GetComponent<CapsuleCollider> ().radius = mod.fRad;
		//ERROR:: The collider doesn't rescale properly if the size increases past the original... idk why

	}
    */
    /* UNNEEDED WITH JUST ABILITY BUTTONS
	public void ReleaseOverOther(GameObject other){

		// Check if the other object has a ViewAction component
		ViewAction viewAction = other.GetComponent<ViewAction>();
		if (viewAction != null) {
			// Then use the action
			Controller.Get().NotifyObs(Notification.ReleaseChrOverAct, this, viewAction);
			return;
		}

		// If the object has none of the desired components
		Controller.Get().NotifyObs(Notification.ReleaseChrOverNone, this, null);
	}
    */

    /*//Undoes the image and border scaling set by the parent
    public void Unscale(){
		transform.localScale = new Vector3
			(transform.localScale.x / transform.parent.localScale.x,
				transform.localScale.y / transform.parent.localScale.y,
				transform.localScale.z / transform.parent.localScale.z);
	}*/
}
