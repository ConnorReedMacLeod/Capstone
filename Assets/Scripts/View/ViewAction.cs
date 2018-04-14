using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ViewAction : Observer {

	public int id;                              //The action's unique identifier
	public Chr mod;                      		//The action's model
	public ViewActionWheel viewActionWheel;     //The action's ActionWheel segment's view

    //Sets the ActionWheel segment's material
	public void SetActionMaterial (string _sName){
		string sMatPath = "Materials/Actions/matAction" + _sName;
		Material matChr = Resources.Load(sMatPath, typeof(Material)) as Material;
		GetComponent<Renderer> ().material = matChr;
	}

    //Let the Action button know which character and id it's representing
	public void SetModel (Chr _mod){
		mod = _mod;
		mod.Subscribe (this);
	}

    //Notifies application when the action's ActionWheel segment is clicked
    public void OnMouseDown(){
		Controller.Get().NotifyObs(Notification.ClickAct, this, id);
	}	
}
