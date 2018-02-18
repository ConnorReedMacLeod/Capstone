using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewChr : Observer {

	bool bStarted;// so that updates aren't looked at until after this has been initialized

	public Character mod;

	//keep some local variables to keep track of things that have changed
	Character.STATESELECT lastStateSelect;
	Vector3 lastPos;

	public GameObject pfActionWheel;
	public GameObject objActionWheel;
	public ViewActionWheel viewActionWheel;

	const int indexCharBorder = 0;
	const int indexCharPortrait = 1;

	//undoes the scaling of the parent
	public void Unscale(){
		transform.localScale = new Vector3
			(transform.localScale.x / transform.parent.localScale.x,
				transform.localScale.y / transform.parent.localScale.y,
				transform.localScale.z / transform.parent.localScale.z);
	}

	void setPortrait(string _sName){
		string sMatPath = "Materials/Characters/mat" + _sName;
		Material matChr = Resources.Load(sMatPath, typeof(Material)) as Material;

		GetComponentsInChildren<Renderer> ()[indexCharPortrait].material = matChr;
	}

	void SetBorder(string _sName){
		string sMatPath = "Materials/mat" + _sName;
		Material matBorder = Resources.Load(sMatPath, typeof(Material)) as Material;

		GetComponentsInChildren<Renderer> ()[indexCharBorder].material = matBorder;
	}

	public void SetModel(Character _mod){
		mod = _mod;
		mod.Subscribe (this);

		// Set the portrait to be this new character
		setPortrait (mod.sName);
	}

	void UpdatePos(){
		if (lastPos != mod.pos) {
			lastPos = mod.pos;

			//TODO:: Will probably update this to be some animation
			transform.localPosition = lastPos;
		}
	}

	void AddActionWheel(){
		Debug.Assert (objActionWheel == null);
		Debug.Assert (viewActionWheel == null);
		objActionWheel = Instantiate (pfActionWheel, transform);
		//viewActionWheel = objActionWheel.AddComponent<ViewActionWheel> ();
		viewActionWheel = objActionWheel.GetComponent<ViewActionWheel>();
		viewActionWheel.setModel (mod);
	}

	void RemoveActionWheel(){
		Debug.Assert (objActionWheel != null);
		Debug.Assert (viewActionWheel != null);
		Destroy (objActionWheel);
		objActionWheel = null;
		viewActionWheel = null;
	}

	void UpdateStatus(){

		// Don't accept udpates until after initialized
		if (!bStarted)
			return;

		//If our selected status has changed
		if (lastStateSelect != mod.stateSelect) {
			 
			switch (mod.stateSelect) {
			case Character.STATESELECT.SELECTED:
				//Need to add the action wheel
				SetBorder ("ChrBorderSelected");
				AddActionWheel ();
				break;

			case Character.STATESELECT.TARGGETING:
				//Need to remove action wheel
				RemoveActionWheel ();
				break;

			case Character.STATESELECT.UNSELECTED:
				//Determine which state we came from
				if (lastStateSelect == Character.STATESELECT.SELECTED) {
					// then we just deselected
					SetBorder ("ChrBorder");
					RemoveActionWheel ();
				} else if (lastStateSelect == Character.STATESELECT.TARGGETING) {
					// then we finished selecting targets
					SetBorder ("ChrBorder");
				}
				break;
			default: 
				Debug.LogError ("UNRECOGNIZED VIEW CHAR SELECT STATE!");
				return;
			}

			lastStateSelect = mod.stateSelect;
		}

	}

	override public void UpdateObs(){
		// Detect if the position has been changed
		UpdatePos();

		// Detect if selected status has changed
		UpdateStatus();
	}

	public void OnMouseDown(){
		app.Notify (Notification.ClickChr, this);
	}

	public void Start(){
		Unscale ();
		bStarted = true;

		lastStateSelect = Character.STATESELECT.UNSELECTED;
	}
}
