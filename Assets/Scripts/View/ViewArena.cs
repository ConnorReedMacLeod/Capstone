using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Arena))]
public class ViewArena : Observer {

	public Arena mod;                   //The Arena's model

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

		}
    }

	//TODO:: decide if preloading variables on Startup is better,
	//       or if using null-checking accessors everywhere is better
	//Find the model, and do any setup for reflect it
	public void InitModel(){
		mod = GetComponent<Arena>();
		mod.Subscribe (this);
	}
		
    //Gets the mouse position within the Arena
	//WARNING:: This requires the arena to be centered at 0,0 in worldspace
	public Vector3 GetArenaPos(Vector3 pos){

		Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);
		//return new Vector3 (worldPos.x / fWidth, worldPos.y / fHeight, worldPos.z);//TODO:: figure out a proper z coord

		Debug.Log("Click at " + new Vector3 (worldPos.x, worldPos.y, 0));
		return new Vector3 (worldPos.x, worldPos.y, worldPos.z);
	}

	//TODO:: Update this to reflect selecting specific coordinates in the grid
    //Notifies application when the Arena view is clicked on
    public void OnMouseDown(){
		Controller.Get().NotifyObs(Notification.ClickArena, this, GetArenaPos(Input.mousePosition));
	}
}
