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

	public GameObject pfActionWheel;        //ActionWheel prefab
	public GameObject objActionWheel;       //ActionWheel object
	public ViewActionWheel viewActionWheel; //ActionWheel view

	public float fOrigRadius;
	public float fOrigWidth;
	public float fOrigHeight;
	public float fOrigScaleX;
	public float fOrigScaleY;

	const int indexTransformParent = 0;			//The parent object's transform index
	const int indexRendererBorder = 0;         	//The border surrounding the character's portrait
	const int indexTransformBorder = 1;
	const int indexRendererPortrait = 1;		//The character's portait
	const int indexTransformPortrait = 2;        		

    public void Start()
    {
		if (bStarted == false) {
			bStarted = true;

			// Find our model
			InitModel ();
			//Unscale ();
			lastStateSelect = Chr.STATESELECT.IDLE;
			InitMouseHandler ();

			//Get the base size of the character prefab so we can scale it later
			fOrigWidth = GetComponent<Collider>().bounds.size.x;
			fOrigHeight = GetComponent<Collider>().bounds.size.y;
			fOrigRadius = GetComponent<CapsuleCollider> ().radius;
			fOrigScaleX = transform.localScale.x;
			fOrigScaleY = transform.localScale.y;
		}
    }

	public void Init(){
		setPortrait (mod.sName);
		if (mod.plyrOwner.id == 0) {
			//Find the portrait and flip it for one of the players
			GetComponentsInChildren<Transform>()[indexTransformPortrait].localScale = new Vector3 (-1, 1, 1);


		}
	}

	public void InitMouseHandler(){
		mousehandler = GetComponent<MouseHandler> ();
		mousehandler.SetOwner (this);

		mousehandler.SetNtfClick (Notification.ClickChr);
		mousehandler.SetNtfDoubleClick (Notification.ClickChr);

		mousehandler.SetNtfStartHold (Notification.ChrStartHold);

		mousehandler.SetNtfStopHold (Notification.ChrStopHold);

		mousehandler.SetReleaseOtherCallback (ReleaseOverOther);
	}

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

    /*//Undoes the image and border scaling set by the parent
    public void Unscale(){
		transform.localScale = new Vector3
			(transform.localScale.x / transform.parent.localScale.x,
				transform.localScale.y / transform.parent.localScale.y,
				transform.localScale.z / transform.parent.localScale.z);
	}*/

    //Sets the material used for the character's portrait
	void setPortrait(string _sName){
		string sMatPath = "Materials/Chrs/mat" + _sName;
		Material matChr = Resources.Load(sMatPath, typeof(Material)) as Material;

		GetComponentsInChildren<Renderer> ()[indexRendererPortrait].material = matChr;

	}

    //Sets the material used for the character's border
	void SetBorder(string _sName){
		string sMatPath = "Materials/mat" + _sName;
		Material matBorder = Resources.Load(sMatPath, typeof(Material)) as Material;

		GetComponentsInChildren<Renderer> ()[indexRendererBorder].material = matBorder;
	}

    //Find the model, and do any setup for reflect it
	public void InitModel(){
		mod = GetComponent<Chr>();
		mod.Subscribe (this);

	}

    //Updates the character's position to match the model's position
	void UpdatePos(){
		transform.position = mod.v3Pos;
	}

    //Spawns the ActionWheel
	void AddActionWheel(){
		Debug.Assert (objActionWheel == null);
		Debug.Assert (viewActionWheel == null);
		objActionWheel = Instantiate (pfActionWheel, transform);
		///viewActionWheel = objActionWheel.AddComponent<ViewActionWheel> ();
		viewActionWheel = objActionWheel.GetComponent<ViewActionWheel>();
		viewActionWheel.setModel (mod);
	}

    //Despawns the ActionWheel
	void RemoveActionWheel(){
		Debug.Assert (objActionWheel != null);
		Debug.Assert (viewActionWheel != null);
		Destroy (objActionWheel);
		objActionWheel = null;
		viewActionWheel = null;
	}

	public void UpdateSize(){
		// Check the model's desired radius, so we can match that

		float fRelativeScale = fOrigRadius / mod.fRad;

		transform.localScale = new Vector3 (fOrigScaleX / fRelativeScale, fOrigScaleY / fRelativeScale, 1.0f);
		GetComponent<CapsuleCollider> ().radius = mod.fRad;
		//ERROR:: The collider doesn't rescale properly if the size increases past the original... idk why

	}

	//TODO:: Make this a state machine
    //Updates the character's state (SELECTED, TARGETTING, UNSELECTED)
	void UpdateStatus(){
		//Refuses to accept udpates until after initialized
		if (!bStarted)
			return;

		UpdateSize ();

		//Checks if character status has changed
		if (lastStateSelect != mod.stateSelect) {
			switch (mod.stateSelect) {

			//On switch to selection, highlight the border
			case Chr.STATESELECT.SELECTED:
				if (lastStateSelect == Chr.STATESELECT.CHOOSINGACT) {
					RemoveActionWheel ();
				}
				SetBorder ("ChrBorderSelected");

				break;
            
            //On switch to choosing action, spawns the ActionWheel
			case Chr.STATESELECT.CHOOSINGACT:
				SetBorder ("ChrBorder");
				AddActionWheel ();
				break;

            //On switch to targetting, despawns the ActionWheel
			case Chr.STATESELECT.TARGGETING:
				RemoveActionWheel ();
				break;

            //On switch to unselected, make changes depending on previous state
			case Chr.STATESELECT.IDLE:
                //If previously choosing an action, despawn the ActionWheel
				if (lastStateSelect == Chr.STATESELECT.CHOOSINGACT) {
					RemoveActionWheel ();

				} else if (lastStateSelect == Chr.STATESELECT.TARGGETING) {
					//Nothing needs to be done (currently, this may change)

				} else if (lastStateSelect == Chr.STATESELECT.SELECTED) {
					//Then unhighlight the border
					SetBorder ("ChrBorder");
				}
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
		UpdatePos();
		UpdateStatus();
	}
		
}
