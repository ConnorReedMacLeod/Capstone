using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

// Will generally contain everything in a match
// responsible for creating and managing the game

public class Match : MonoBehaviour {

    bool bStarted;                          //Confirms the Start() method has executed

    public int nPlayers = 2;
    public Player[] arPlayers;

    public Chr[][] arChrs;

    public GameObject pfPlayer;
    public GameObject pfChr;

    //TODO: Move cursor settings into their own script
    public Texture2D txCursor;                          //Cursor Texture
    public CursorMode cursorMode = CursorMode.Auto;     //Cursor Mode
    public Vector2 v2HotSpot = Vector2.zero;            //Cursor Start Position

    public static Match instance;

    public static Match Get() {
        if(instance == null) {
            GameObject go = GameObject.FindGameObjectWithTag("Match");
            if(go == null) {
                Debug.LogError("ERROR! NO OBJECT HAS A MATCH TAG!");
            }
            instance = go.GetComponent<Match>();
            if(instance == null) {
                Debug.LogError("ERROR! MATCH TAGGED OBJECT DOES NOT HAVE A MATCH COMPONENT!");
            }
            instance.Start();
        }
        return instance;
    }

    public Player GetLocalPlayer() {
        return arPlayers[0];
    }

    public Player GetEnemyPlayer() {
        return arPlayers[1];
    }

    void InitPlayers(int _nPlayers) {
        nPlayers = _nPlayers;//in case this needs to be changed based on the match
        arPlayers = new Player[nPlayers];
        arChrs = new Chr[nPlayers][];

        for(int i = 0; i < _nPlayers; i++) {
            GameObject goPlayer = Instantiate(pfPlayer, this.transform);
            Player newPlayer = goPlayer.GetComponent<Player>();
            if(newPlayer == null) {
                Debug.LogError("ERROR! NO PLAYER COMPONENT ON PLAYER PREFAB!");
            }

            newPlayer.SetID(i);
            newPlayer.Start();

            arPlayers[i] = newPlayer;
        }
    }

    // Will eventually need a clean solution to adding/removing characters
    // while managing ids - some sort of Buffer of unused views will probably help
    void InitChr(Chr.CHARTYPE type, Player player, int id) {

        GameObject goChr = Instantiate(pfChr, this.transform);
        Chr newChr = goChr.GetComponent<Chr>();
        if(newChr == null) {
            Debug.LogError("ERROR! NO CHR COMPONENT ON CHR PREFAB!");
        }

        newChr.Start();

        switch(type) {
        case Chr.CHARTYPE.KATARINA:
            newChr.InitChr(player, id, new ChrKatarina(newChr));
            break;
        case Chr.CHARTYPE.FISCHER:
            newChr.InitChr(player, id, new ChrFischer(newChr));
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
            Debug.LogError("INVALID CHARACTER SELECTION");
            Application.Quit();
            newChr.InitChr(player, id, new ChrKatarina(newChr)); //so the editor will let us compile
            break;
        }
        arChrs[player.id][id] = newChr;
        player.arChr[id] = newChr;


    }

    IEnumerator InitAllChrs() {

        //Keep looping until we've properly setup our character selections
        while(CharacterSelection.Get().bSavedSelections == false) {
            Debug.Log("Waiting for character selections to be registered and distributed");
            yield return null;
        }

        for(int i = 0; i < nPlayers; i++) {
            arChrs[i] = new Chr[Player.MAXCHRS];
            arPlayers[i].nChrs = Player.MAXCHRS;

            for(int j = 0; j < arPlayers[i].nChrs; j++) {
                InitChr(CharacterSelection.Get().arChrSelections[i][j], arPlayers[i], j);
            }
        }

        Debug.Log("Ending Character Initializations");
    }

    IEnumerator InitAllChrPositions() {

        //TODO - have this set up with the character loadout phase - for now, just give default positions
        while(false) {
            //Wait for position input from the character loadout phase
            yield return null;
        }

        //Set up each team in a 'triangle' - two sides in the back, center in the front
        ContPositions.Get().MoveChrToPosition(arChrs[0][0], ContPositions.Get().GetAlliedBacklinePositions(arPlayers[0])[0]);
        ContPositions.Get().MoveChrToPosition(arChrs[0][1], ContPositions.Get().GetAlliedFrontlinePositions(arPlayers[0])[1]);
        ContPositions.Get().MoveChrToPosition(arChrs[0][2], ContPositions.Get().GetAlliedBacklinePositions(arPlayers[0])[2]);

        ContPositions.Get().MoveChrToPosition(arChrs[1][0], ContPositions.Get().GetAlliedBacklinePositions(arPlayers[1])[0]);
        ContPositions.Get().MoveChrToPosition(arChrs[1][1], ContPositions.Get().GetAlliedFrontlinePositions(arPlayers[1])[1]);
        ContPositions.Get().MoveChrToPosition(arChrs[1][2], ContPositions.Get().GetAlliedBacklinePositions(arPlayers[1])[2]);

    }

    public void InitNetworking() {

        Debug.Log("Spawning networkcontroller");

        //Spawn the  client networking manager for our local player (and let the opponent spawn their own controller)
        GameObject goNetworkController = PhotonNetwork.Instantiate("pfNetworkController", Vector3.zero, Quaternion.identity);

        if(goNetworkController = null) {
            Debug.LogError("No prefab found for network controller");
        }
    }


    public IEnumerator Start() {
        if(bStarted) {
            yield return null;
        }
        bStarted = true;

        gameObject.tag = "Match"; // So that anything can find this very quickly

        InitPlayers(nPlayers);

        Debug.Log("Finished initializing players");

        InitNetworking();

        Debug.Log("Finished Initializing Networking");

        //Initialize characters (and spin until we get their selections)
        yield return StartCoroutine(InitAllChrs());

        Debug.Log("After InitAllChrs");

        //Assign local input controllers for each player
        yield return StartCoroutine(CharacterSelection.Get().AssignAllLocalInputControllers());

        Debug.Log("After assigning local input controllers");

        yield return StartCoroutine(InitAllChrPositions());

        Debug.Log("After initializing positions");

        //ContPositions.Get().PrintAllPositions();

        ContTurns.Get().InitializePriorities();

        Debug.Log("After InitializePriorities");

        Cursor.SetCursor(txCursor, v2HotSpot, cursorMode);

    }
}
