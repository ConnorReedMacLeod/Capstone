using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Will contain all of the status of the game

public class Model : Element {
	
	Arena arena;

	static int nPlayers = 2;
	Player [] arPlayers;
	Character [][] arChrs;


	void initPlayers (int _nPlayers){
		nPlayers = _nPlayers;//in case this needs to be changed based on the match
		arPlayers = new Player[nPlayers];
		arChrs = new Character[nPlayers][];

		for (int i = 0; i < _nPlayers; i++) {
			arPlayers [i] = new Player (i);
			arPlayers [i].setChrs (); /// todo: replace with actual character selection
		}
	}

	// Will eventually need a clean solution to adding/removing characters
	// while managing ids - some sort of Buffer of unused views will probably help
	void initChr (Character.CHARTYPE type, int idOwner, int id){

		Character newChr;

		switch (type) {
		case Character.CHARTYPE.KATARA: 
			newChr = new ChrKatara (idOwner, id);
			break;
		case Character.CHARTYPE.LANCER:
			newChr = new ChrLancer (idOwner, id);
			break;
		case Character.CHARTYPE.SKELCOWBOY:
			newChr = new ChrSkelCowboy (idOwner, id);
			break;
		default: 
			Debug.LogError ("INVALID CHARACTER SELECTION");
			Application.Quit ();
			newChr = new Character (idOwner, id); //so the editor will let us compile
			break;
		}
		arChrs [idOwner] [id] = newChr;

		//Let the Arena View know that there's a new Character to represent
		app.view.viewArena.RegisterChar(newChr);
	}

	void initAllChrs(){
		for (int i = 0; i < nPlayers; i++) {
			arChrs [i] = new Character[Player.MAXCHRS];

			for (int j = 0; j < arPlayers [i].nChrs; j++) {
				initChr (arPlayers [i].arChrTypeSelection [j], i, j);
			}
		}
	}

	// Use this for initialization
	public Model(){
		
		arena = new Arena ();

	}

	public void Start(){
		initPlayers (nPlayers);

		initAllChrs ();
	}
}
