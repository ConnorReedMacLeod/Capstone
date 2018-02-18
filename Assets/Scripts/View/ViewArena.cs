using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewArena : Observer {

	public Arena mod;

	public ViewChr [][] arviewChr;

	public GameObject pfChr;
	public GameObject pfActionWheel;

	public GameObject objChrContainer;

	public void SetModel(Arena _mod){
		mod = _mod;
		mod.Subscribe (this);

	}

	public void RegisterChar(Character chr){
		//create a new Character View for the character
		GameObject newObjChr = Instantiate (pfChr, this.transform);
		ViewChr newViewChr = newObjChr.AddComponent<ViewChr> () as ViewChr;

		newViewChr.pfActionWheel = pfActionWheel;

		//Let the view know which character it's representing
		newViewChr.SetModel (chr);

		//Add it to the list of Character views
		arviewChr [chr.idOwner] [chr.id] = newViewChr;

		//Have the new view make sure it's reflecting the model
		newViewChr.UpdateObs ();
	}

	public ViewArena(){

		arviewChr = new ViewChr[Player.MAXPLAYERS][];
		for (int i = 0; i < Player.MAXPLAYERS; i++) {
			arviewChr [i] = new ViewChr[Player.MAXCHRS];
		}
	}
		
	public void OnMouseDown(){
		//TODO:: Actually generate the correct mouse coordinates to pass along
		//       Note - these must be relative to the arena

		//float[] mousePos = {0f,0f,0f};
		//Debug.Log (Camera.main.ScreenToWorldPoint(Input.mousePosition));
		app.Notify (Notification.ClickArena, this, Vector3.zero);
	}



	public void Start(){

	}
}
