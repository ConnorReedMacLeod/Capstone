using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewChr : Observer {

	public Character mod;

	//keep some local variables to keep track of things that have changed
	bool lastbSelected;
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
		//If our selected status has changed
		if (lastbSelected != mod.bSelected) {
			lastbSelected = mod.bSelected;
			if (lastbSelected) {
				SetBorder ("ChrBorderSelected");

				AddActionWheel ();

			} else {
				SetBorder ("ChrBorder");

				RemoveActionWheel ();

			}

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
	}
}
