using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Arena))]
[RequireComponent (typeof(MouseHandler))]
public class ViewArena : Observer {

	public Arena mod;                   //The Arena's model
	public MouseHandler mousehandler;

	public float fWidth;                //The Arena's width
	public float fHeight;               //The Arena's height


	bool bStarted;

    public void cbClickArena(Object target, params Object [] args) {

    }

    //Gets the Arena view's dimensions
    public void Start()
    {

		if (bStarted == false) {
			bStarted = true;

			// Find our model
			InitModel ();
			Vector3 size = GetComponent<Renderer> ().bounds.size;
			fWidth = size.x;
			fHeight = size.y;

			InitMouseHandler ();
		}
    }

	//TODO:: decide if preloading variables on Startup is better,
	//       or if using null-checking accessors everywhere is better
	//Find the model, and do any setup for reflect it
	public void InitModel(){
		mod = GetComponent<Arena>();
		mod.Subscribe (this);
	}

	public void InitMouseHandler(){
		mousehandler = GetComponent<MouseHandler> ();
		mousehandler.SetOwner (this);

		mousehandler.SetNtfClick (Notification.ClickArena);
		mousehandler.SetNtfDoubleClick (Notification.ClickArena);

	}
}
