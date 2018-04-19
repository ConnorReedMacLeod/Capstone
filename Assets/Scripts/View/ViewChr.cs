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
			lastStateSelect = Chr.STATESELECT.UNSELECTED;
			InitMouseHandler ();
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

		mousehandler.SetNtfStartHold (Notification.ClickChr);
		mousehandler.SetNtfStartDrag (Notification.ClickChr);

		mousehandler.SetReleaseOtherCallback (ReleaseOverOther);
	}

	public void ReleaseOverOther(GameObject other){

		// Check if the other object has a ViewAction component
		ViewAction viewAction = other.GetComponent<ViewAction>();
		if (viewAction != null) {
			// Then use the action
			// TODO:: Use the action
			return;
		}

		// If the object has none of the desired components
		// TODO:: Need to make us unselect the current character I guess
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

    //Updates the character's state (SELECTED, TARGETTING, UNSELECTED)
	void UpdateStatus(){
		//Refuses to accept udpates until after initialized
		if (!bStarted)
			return;
		//Checks if character status has changed
		if (lastStateSelect != mod.stateSelect) {
			switch (mod.stateSelect) {
            
            //On switch to selected, spawns the ActionWheel and highlights character border
			case Chr.STATESELECT.SELECTED:
				SetBorder ("ChrBorderSelected");
				AddActionWheel ();
				break;

            //On switch to targetting, despawns the ActionWheel
			case Chr.STATESELECT.TARGGETING:
				RemoveActionWheel ();
				break;

            //On switch to unselected, make changes depending on previous state
			case Chr.STATESELECT.UNSELECTED:
                //If previously SELECTED, unhighlights character border and despawns ActionWheel
				if (lastStateSelect == Chr.STATESELECT.SELECTED) {
					SetBorder ("ChrBorder");
					RemoveActionWheel ();
                //If previously TARGETTING, unhilights character border
				} else if (lastStateSelect == Chr.STATESELECT.TARGGETING) {
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
