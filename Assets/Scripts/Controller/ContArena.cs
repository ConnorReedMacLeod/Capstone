using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContArena : Observer {

	public float fTimeMouseHeld;

	public Vector3 v3HeldPos;
	public bool bHeldArena;
	public float fHeldDelay; //Time until it counts as holding
	public float fDistDelay; //Minimum distance before Distance is immediately displayed

	public GameObject pfDistance;
	public bool bDistanceShown;
	public GameObject goDistance;
	public ViewDistance viewDistance;


	// Use this for initialization
	void Start () {
		fHeldDelay = 0.2f;
		fDistDelay = 1.0f;
	}

	public override void UpdateObs(string eventType, Object target, params object[] args){


		switch (eventType) {
		case Notification.ClickArena:
			bHeldArena = true;
			v3HeldPos = (Vector3)args [0];

			break;
		case Notification.MouseHeldStop:
			bHeldArena = false;
			fTimeMouseHeld = 0.0f;
			v3HeldPos = Vector3.zero;
			DespawnDistance ();

			break;

		default:

			break;
		}
	}

	public void SpawnDistance (Vector3 v3Start){
		bDistanceShown = true;
		Debug.Log("SpawnDistance");

		goDistance = Instantiate (pfDistance, Match.Get ().arena.transform);
		viewDistance = goDistance.GetComponent<ViewDistance> ();
		if (viewDistance == null) {
			Debug.LogError ("ERROR! NO VIEWDISTANCE COMPONENT ON GAMEOBJECT");
		}

		viewDistance.Start ();
		viewDistance.SetStart (v3Start);
		viewDistance.SetEnd (ViewArena.GetArenaPos(Input.mousePosition));

	}

	public void DespawnDistance (){
		if (bDistanceShown == false)
			return;
		Debug.Log ("DespawnDistance");

		Destroy (goDistance);
		viewDistance = null;
		goDistance = null;

		bDistanceShown = false;

	}

	void Update () {
		if (bHeldArena) {
			fTimeMouseHeld += Time.deltaTime;
			if (bDistanceShown == false && 
				((fTimeMouseHeld >= fHeldDelay) || 
					(ViewDistance.Dist(v3HeldPos, ViewArena.GetArenaPos(Input.mousePosition)) >= fDistDelay))) {
				//Check if either we've been holding the mouse long enough, or we've moved the mouse far enough
				SpawnDistance (v3HeldPos);
			}
		}

		//TODO:: Consider putting this in a general controller that notifies everyone else
		if (Input.GetMouseButtonUp(0)) {
			Controller.Get ().NotifyObs (Notification.MouseHeldStop, null);
		}
	}
}
