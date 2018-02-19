using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Will contain all of the status of the game

public class Model : Element {
	
	public Arena arena;

	public int nPlayers = 2;
	public Player [] arPlayers;

	public Character [][] arChrs;

	public Timeline timeline;


	void InitPlayers (int _nPlayers){
		nPlayers = _nPlayers;//in case this needs to be changed based on the match
		arPlayers = new Player[nPlayers];
		arChrs = new Character[nPlayers][];

		for (int i = 0; i < _nPlayers; i++) {
			arPlayers [i] = new Player (i);
			arPlayers [i].setChrs (); /// TODO:: replace with actual character selection
		}
	}

	// Will eventually need a clean solution to adding/removing characters
	// while managing ids - some sort of Buffer of unused views will probably help
	void InitChr (Character.CHARTYPE type, Player player, int id){

		Character newChr;

		switch (type) {
		case Character.CHARTYPE.KATARA: 
			newChr = new ChrKatara (player, id);
			break;
		case Character.CHARTYPE.LANCER:
			newChr = new ChrLancer (player, id);
			break;
		case Character.CHARTYPE.SKELCOWBOY:
			newChr = new ChrSkelCowboy (player, id);
			break;
		default: 
			Debug.LogError ("INVALID CHARACTER SELECTION");
			Application.Quit ();
			newChr = new Character (player, id); //so the editor will let us compile
			break;
		}
		arChrs [player.id] [id] = newChr;

		//Initially set a random position for the new character *** CHANGE THIS AT SOME POINT ***
		newChr.SetPosition(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));

		//Let the Arena View know that there's a new Character to represent
		app.view.viewArena.RegisterChar(newChr);

	}

	void InitAllChrs(){
		for (int i = 0; i < nPlayers; i++) {
			arChrs [i] = new Character[Player.MAXCHRS];

			for (int j = 0; j < arPlayers [i].nChrs; j++) {
				InitChr (arPlayers [i].arChrTypeSelection [j], arPlayers[i], j);
			}
		}
	}

	public void NextTimelineEvent(){
		timeline.EvaluateEvent ();
	}

	// Use this for initialization
	public Model(){
		
		arena = new Arena ();

		timeline = Timeline.Get ();

	}

	public void Start(){
		InitPlayers (nPlayers);

		InitAllChrs ();

		timeline.InitTimeline (this);

		/*FOR TESTING -> REMOVE THIS AT SOME POINT*/
		Invoke("NextTimelineEvent", 5);
		/**/

	}
}
