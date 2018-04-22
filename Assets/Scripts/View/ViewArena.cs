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

	//TODO:: Consider if there's a better way to do this rather than just copying code
	public void SpawnDistance (Chr chrStart){

		goDistance = Instantiate (pfDistance, Match.Get ().arena.transform);
		viewDistance = goDistance.GetComponent<ViewDistance> ();
		if (viewDistance == null) {
			Debug.LogError ("ERROR! NO VIEWDISTANCE COMPONENT ON GAMEOBJECT");
		}

		viewDistance.Start ();
		viewDistance.SetStart (chrStart);
		viewDistance.SetEnd (LibView.GetMouseLocation());

	}

	// If no argument is provided, assume that we want to spawn it under the original mouse click
	public void SpawnDistance (){
		SpawnDistance (mousehandler.v3Down);
	}

	public void SpawnDistance (Vector3 v3Start){

		goDistance = Instantiate (pfDistance, Match.Get ().arena.transform);
		viewDistance = goDistance.GetComponent<ViewDistance> ();
		if (viewDistance == null) {
			Debug.LogError ("ERROR! NO VIEWDISTANCE COMPONENT ON GAMEOBJECT");
		}

		viewDistance.Start ();
		viewDistance.SetStart (v3Start);
		viewDistance.SetEnd (LibView.GetMouseLocation());

	}

	public void DespawnDistance (){

		Destroy (goDistance);
		viewDistance = null;
		goDistance = null;

	}
}
