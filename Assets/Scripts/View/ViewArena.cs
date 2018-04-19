using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Arena))]
[RequireComponent (typeof(MouseHandler))]
public class ViewArena : Observer {

	public Arena mod;                   //The Arena's model
	public MouseHandler mousehandler;

	public GameObject pfDistance;
	public GameObject goDistance;		// The currently spawned distance measure
	public ViewDistance viewDistance;

	public float fWidth;                //The Arena's width
	public float fHeight;               //The Arena's height


	bool bStarted;

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

		mousehandler.SetNtfStartDrag (Notification.ArenaStartDrag, SpawnDistance);
		mousehandler.SetNtfStopDrag (Notification.ArenaStopDrag, DespawnDistance);
	}

	public void SpawnDistance (){

		Debug.Log("SpawnDistance");

		goDistance = Instantiate (pfDistance, Match.Get ().arena.transform);
		viewDistance = goDistance.GetComponent<ViewDistance> ();
		if (viewDistance == null) {
			Debug.LogError ("ERROR! NO VIEWDISTANCE COMPONENT ON GAMEOBJECT");
		}

		viewDistance.Start ();
		viewDistance.SetStart (mousehandler.v3Down);
		viewDistance.SetEnd (LibView.GetMouseLocation());

	}

	public void DespawnDistance (){
		
		Debug.Log ("DespawnDistance");

		Destroy (goDistance);
		viewDistance = null;
		goDistance = null;

	}
}
