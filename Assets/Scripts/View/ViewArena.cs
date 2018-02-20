using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewArena : Observer {

	public Arena mod;                   //The Arena's model

	public ViewChr [][] arviewChr;      //Array of characters in the Arena

	public GameObject pfChr;            //Character prefab
	public GameObject pfActionWheel;    //ActionWheel prefab

	public float fWidth;                //The Arena's width
	public float fHeight;               //The Arena's height

    //Gets the Arena view's dimensions
    public void Start()
    {
        Vector3 size = GetComponent<Renderer>().bounds.size;
        fWidth = size.x;
        fHeight = size.y;
    }

    //Sets the Arena view's model
    public void SetModel(Arena _mod){
		mod = _mod;
		mod.Subscribe (this);

	}

    //Constructs the Arena view
    public ViewArena()
    {
        arviewChr = new ViewChr[Player.MAXPLAYERS][];
        for (int i = 0; i < Player.MAXPLAYERS; i++)
        {
            arviewChr[i] = new ViewChr[Player.MAXCHRS];
        }
    }

    //Instantiates a character and gives it a new character view and an ActionWheel
    public void RegisterChar(Character chr){
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

    //Gets the mouse posititon within the Arena
	public Vector3 GetArenaPos(Vector3 pos){
		Vector3 worldPos = Camera.main.ScreenToWorldPoint(pos);
		return new Vector3 (worldPos.x / fWidth, worldPos.y / fHeight, worldPos.z);//TODO:: figure out a proper z coord
	}

    //Notifies application when the Arena view is clicked on
    public void OnMouseDown(){
		///Debug.Log (GetArenaPos(Input.mousePosition));
		app.Notify (Notification.ClickArena, this, GetArenaPos(Input.mousePosition));
	}
}
