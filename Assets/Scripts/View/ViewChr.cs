using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewChr : Observer {

	bool bStarted;                          //Confirms the Start() method has executed

    public Character mod;                   //Character model

	Character.STATESELECT lastStateSelect;  //Tracks previous character state (SELECTED, TARGETTING, UNSELECTED)
	Vector3 v3LastPos;                      //Tracks previous character position against the model position

	public GameObject pfActionWheel;        //ActionWheel prefab
	public GameObject objActionWheel;       //ActionWheel object
	public ViewActionWheel viewActionWheel; //ActionWheel view

	const int indexCharBorder = 0;          //The border surrounding the character's portrait
	const int indexCharPortrait = 1;        //The character's portait


    public void Start()
    {
        Unscale();
        bStarted = true;
        lastStateSelect = Character.STATESELECT.UNSELECTED;
    }

    //Undoes the image and border scaling set by the parent
    public void Unscale(){
		transform.localScale = new Vector3
			(transform.localScale.x / transform.parent.localScale.x,
				transform.localScale.y / transform.parent.localScale.y,
				transform.localScale.z / transform.parent.localScale.z);
	}

    //Sets the material used for the character's portrait
	void setPortrait(string _sName){
		string sMatPath = "Materials/Characters/mat" + _sName;
		Material matChr = Resources.Load(sMatPath, typeof(Material)) as Material;

		GetComponentsInChildren<Renderer> ()[indexCharPortrait].material = matChr;

	}

    //Sets the material used for the character's border
	void SetBorder(string _sName){
		string sMatPath = "Materials/mat" + _sName;
		Material matBorder = Resources.Load(sMatPath, typeof(Material)) as Material;

		GetComponentsInChildren<Renderer> ()[indexCharBorder].material = matBorder;
	}

    //Sets the character's model
	public void SetModel(Character _mod){
		mod = _mod;
		mod.Subscribe (this);
		setPortrait (mod.sName);

		if (mod.playOwner.id == 0) {
			this.transform.localScale = new Vector3 (-1, 1, 1);
		}
	}

    //Updates the character's position to match the model's position
	void UpdatePos(){
        //Checks if character position has changed
		if (v3LastPos != mod.v3Pos) {
			v3LastPos = mod.v3Pos;
			//TODO:: Will probably update this to be some animation
			transform.localPosition = v3LastPos;
		}
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
            
            //On switch to selecteds, spawns the ActionWheel and highlights character border
            case Character.STATESELECT.SELECTED:
				SetBorder ("ChrBorderSelected");
				AddActionWheel ();
				break;

            //On switch to targetting, despawns the ActionWheel
            case Character.STATESELECT.TARGGETING:
				RemoveActionWheel ();
				break;

            //On switch to unselected, make changes depending on previous state
			case Character.STATESELECT.UNSELECTED:
                //If previously SELECTED, unhighlights character border and despawns ActionWheel
                if (lastStateSelect == Character.STATESELECT.SELECTED) {
					SetBorder ("ChrBorder");
					RemoveActionWheel ();
                //If previously TARGETTING, unhilights character border
                } else if (lastStateSelect == Character.STATESELECT.TARGGETING) {
					SetBorder ("ChrBorder");
				}
				break;
            
            //Catches unrecognized character states
			default: 
				Debug.LogError ("UNRECOGNIZED VIEW CHAR SELECT STATE!");
				return;
			}
			lastStateSelect = mod.stateSelect;
		}
	}

    //Updates character view, detecting if changes are needed to the character position or state
	override public void UpdateObs(){
		UpdatePos();
		UpdateStatus();
	}

    //Notifies application when the character view is clicked on
	public void OnMouseDown(){
		app.Notify (Notification.ClickChr, this, app.view.viewArena.GetArenaPos(Input.mousePosition));
	}
}
