using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewAction : Observer {

	public int id;                              //The action's unique identifier
	public Character mod;                       //The action's model
	public ViewActionWheel viewActionWheel;     //The action's ActionWheel segment's view

    //Sets the ActionWheel segment's material
	public void SetActionMaterial (string _sName){
		string sMatPath = "Materials/Actions/matAction" + _sName;
		Material matChr = Resources.Load(sMatPath, typeof(Material)) as Material;
		GetComponent<Renderer> ().material = matChr;
	}

    //Sets the action's model
	public void SetModel (Character _mod){
		mod = _mod;
		mod.Subscribe (this);
	}

    //Sets the ActionWheel segment's view
    public ViewAction(ViewActionWheel _viewActionWheel, int _id)
    {
        viewActionWheel = _viewActionWheel;
        id = _id;
    }

    //Notifies application when the action's ActionWheel segment is clicked
    public void OnMouseDown(){
		app.Notify (Notification.ClickAct, this, id);
	}	
}
