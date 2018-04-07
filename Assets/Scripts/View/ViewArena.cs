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

    //Constructs the Arena view
    public ViewArena()
    {

    }

	/* *** TODO:: UNCOMMENT THIS TO FIGURE OUT WHERE TO KEEP TRACK OF CHARACTER POSITIONS IN THE ARENA *** 
    //Instantiates a character and gives it a new character view and an ActionWheel
	public void RegisterChar(Chr chr){
        //Instantiates the character
		GameObject newObjChr = Instantiate (pfChr, this.transform);
        //Creates a character view
		ViewChr newViewChr = newObjChr.AddComponent<ViewChr> () as ViewChr;
        //Sets the character view's model
        newViewChr.SetModel(chr);
        //Assigns the character view to the character
        arviewChr[chr.plyrOwner.id][chr.id] = newViewChr;
        //Sets the character view to match its model
        newViewChr.UpdateObs();
        //Gives the character view an ActionWheel prefab
        newViewChr.pfActionWheel = pfActionWheel;
	}  
	*** *******************************************************************************************************/

    //Gets the mouse posititon within the Arena
	public Vector3 GetArenaPos(Vector3 pos){
		Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);
		return new Vector3 (worldPos.x / fWidth, worldPos.y / fHeight, worldPos.z);//TODO:: figure out a proper z coord
	}

	//TODO:: Update this to reflect selecting specific coordinates in the grid
    //Notifies application when the Arena view is clicked on
    public void OnMouseDown(){
		///Debug.Log (GetArenaPos(Input.mousePosition));
		Controller.Get().NotifyObs(Notification.ClickArena, this, GetArenaPos(Input.mousePosition));
	}
}
