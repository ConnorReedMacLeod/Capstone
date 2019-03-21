using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Will generally contain everything in a match
// responsible for creating and managing the game

public class Match : MonoBehaviour {

    bool bStarted;                          //Confirms the Start() method has executed

    public Arena arena;

	public int nPlayers = 2;
	public Player [] arPlayers;

	public Chr [][] arChrs;

	public Controller controller;

	public GameObject pfPlayer;
	public GameObject pfChr;

	//TODO: Move cursor settings into their own script
	public Texture2D txCursor;							//Cursor Texture
	public CursorMode cursorMode = CursorMode.Auto;     //Cursor Mode
	public Vector2 v2HotSpot = Vector2.zero;			//Cursor Start Position

	public static Match instance;

	public static Match Get (){
		if (instance == null) {
			GameObject go = GameObject.FindGameObjectWithTag ("Match");
			if (go == null) {
				Debug.LogError ("ERROR! NO OBJECT HAS A MATCH TAG!");
			}
			instance = go.GetComponent<Match> ();
			if (instance == null) {
				Debug.LogError ("ERROR! MATCH TAGGED OBJECT DOES NOT HAVE A MATCH COMPONENT!");
			}
            instance.Start();
		}
		return instance;
	}

	public Player GetLocalPlayer(){
		return arPlayers [0];
	}

    public Player GetEnemyPlayer() {
        return arPlayers[1];
    }

	void InitPlayers (int _nPlayers){
		nPlayers = _nPlayers;//in case this needs to be changed based on the match
		arPlayers = new Player[nPlayers];
		arChrs = new Chr[nPlayers][];

		for (int i = 0; i < _nPlayers; i++) {
			GameObject goPlayer = Instantiate (pfPlayer, this.transform);
			Player newPlayer = goPlayer.GetComponent<Player> ();// TODO:: Replace this with Network spawning somehow
			if (newPlayer == null) {
				Debug.LogError ("ERROR! NO PLAYER COMPONENT ON PLAYER PREFAB!");
			}

			newPlayer.SetID (i);
			newPlayer.Start ();
			newPlayer.setChrs (); /// TODO:: replace with actual character selection
            newPlayer.SetInputType();
            arPlayers [i] = newPlayer;
		}
	}

	// Will eventually need a clean solution to adding/removing characters
	// while managing ids - some sort of Buffer of unused views will probably help
	void InitChr (Chr.CHARTYPE type, Player player, int id){

		GameObject goChr = Instantiate (pfChr, this.transform);
		Chr newChr = goChr.GetComponent<Chr>();
		if (newChr == null) {
			Debug.LogError ("ERROR! NO CHR COMPONENT ON CHR PREFAB!");
		}

		newChr.Start ();

		switch (type) {
		case Chr.CHARTYPE.KATARINA: 
			newChr.InitChr(player, id, new ChrKatarina(newChr));
			break;
		case Chr.CHARTYPE.FISCHER:
			newChr.InitChr(player, id, new ChrFischer(newChr));
			break;
		case Chr.CHARTYPE.SKELCOWBOY:
			newChr.InitChr(player, id, new ChrSkelCowboy(newChr));
			break;
        case Chr.CHARTYPE.SOHPIDIA:
            newChr.InitChr(player, id, new ChrSophidia(newChr));
            break;
        case Chr.CHARTYPE.PITBEAST:
            newChr.InitChr(player, id, new ChrPitBeast(newChr));
            break;
        case Chr.CHARTYPE.SAIKO:
            newChr.InitChr(player, id, new ChrSaiko(newChr));
            break;
        case Chr.CHARTYPE.RAYNE:
            newChr.InitChr(player, id, new ChrRayne(newChr));
            break;
            default: 
			Debug.LogError ("INVALID CHARACTER SELECTION");
			Application.Quit ();
			newChr.InitChr (player, id, new BaseChr(newChr)); //so the editor will let us compile
			break;
		}
		arChrs [player.id] [id] = newChr;
        player.arChr[id] = newChr;

	 
		//newChr.SetPosition(Random.Range(-10.0f, 10.0f), arena.arfStartingPosY[id]);
		arena.InitPlaceUnit(newChr);

	}

	void InitAllChrs(){
		for (int i = 0; i < nPlayers; i++) {
			arChrs [i] = new Chr[Player.MAXCHRS];

			for (int j = 0; j < arPlayers [i].nChrs; j++) {
				InitChr(arPlayers[i].arChrTypeSelection[j], arPlayers[i], j);
			}
		}
	}

    void InitAllBlockers() {
        for (int i=0; i<nPlayers; i++) {
            arPlayers[i].iBlocker = -1;//temporarily set the blocker to -1, so that we meaningfully change the blocker on the next line
            arPlayers[i].SetBlocker(1);//Initially set the blocker as the second character to go
            arPlayers[i].SetBlocker(0);//Initially set the blocker as the first character to go
        }
    }

	public Controller GetController(){
		if (controller == null) {
			controller = GetComponentInChildren<Controller> ();
			if (controller == null) {
				Debug.LogError ("ERROR! NO CONTROLLER FOUND IN CHILDREN OF MAIN MATCH OBJECT");
			}
		}
		return controller;
	}


	public void Start(){
        if (bStarted) {
            return;
        }
        bStarted = true;

        gameObject.tag = "Match"; // So that anything can find this very quickly

		arena = GetComponentInChildren<Arena> ();
		controller = GetComponentInChildren<Controller> ();

		arena.Start ();

		InitPlayers (nPlayers);

		InitAllChrs ();

        InitAllBlockers();

		Cursor.SetCursor(txCursor, v2HotSpot, cursorMode);

    }
}
