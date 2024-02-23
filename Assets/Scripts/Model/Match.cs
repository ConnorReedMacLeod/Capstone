using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

// Will generally contain everything in a match
// responsible for initializing and containing components a match

public class Match : MonoBehaviour {

    public const int NPLAYERS = 2;
    public const int NMINACTIVECHRSPERTEAM = 3;
    public const int NINITIALCHRSPERTEAM = 5;
    public const int NCHRSPERDRAFT = 7;

    public const int NCHARACTERLIVESPERTEAM = 3;
    public const int NSWITCHINGINDURATION = 3;
    public const int NSUMMONSTARTINGFATIGUE = 2;

    public const int NSOULBREAKDURATION = 3;
    public const int NSOULBREAKPOWERMODIFIER = 100;
    public const int NSOULBREAKDEFENSEMODIFIER = -100;

    bool bStarted;                          //Confirms the Start() method has executed

    public MatchResult matchresult;         //Stores the current status of who (if anyone) has won the match

    public Player[] arPlayers;

    public ManaPool manapool0;
    public ManaPool manapool1;

    public ManaCalendar manaCalendar0;
    public ManaCalendar manaCalendar1;

    public CameraControllerMatch cameraControllerMatch;

    public GameObject pfPlayer;
    public GameObject pfChr;
    public GameObject pfAdaptPanel;

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

    void InitPlayers() {
        arPlayers = new Player[NPLAYERS];

        for(int i = 0; i < arPlayers.Length; i++) {
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

    public Chr InitChr(CharType.CHARTYPE chartype, Player player, LoadoutManager.Loadout loadout, int nStartingFatigue, Position posStart) {

        GameObject goChr = Instantiate(pfChr, this.transform);
        Chr newChr = goChr.GetComponent<Chr>();

        if(newChr == null) {
            Debug.LogError("ERROR! NO CHR COMPONENT ON CHR PREFAB!");
        }

        newChr.Start();

        newChr.InitChr(chartype, player, loadout, nStartingFatigue, posStart);

        return newChr;
    }

    void InitAllChrs() {

        //Since we're initializing characters, we want to ensure the character collection container has been set up
        //  before we starting making characters
        ChrCollection.Get().Start();
        //Also have to ensure the priority system is set up since we'll be initializing fatigues here
        ContTurns.Get().Start();

        for(int j = 0; j < NINITIALCHRSPERTEAM; j++) {

            for(int i = 0; i < NPLAYERS; i++) {

                //Initially set this character's fatigue according to their position in the CharacterOrdering setup
                int nStartingFatigue = 0;

                if (j < NMINACTIVECHRSPERTEAM) {
                    nStartingFatigue = j * NPLAYERS + i + 1;
                }//For characters who aren't active, their starting fatigue will just be 0

                Position posStarting = ContPositions.Get().GetPosition(NetworkMatchSetup.GetPositionCoordsForChr(i, j));

                Chr chrNew = InitChr(NetworkMatchSetup.GetCharacterOrdering(i, j),
                    arPlayers[i],
                    NetworkMatchSetup.GetLoadout(i, j),
                    nStartingFatigue,
                    posStarting
                    );
            }

        }


    }

    public void AssignAllLocalInputControllers() {
        for(int i = 0; i < Match.NPLAYERS; i++) {
            AssignLocalInputController(Match.Get().arPlayers[i]);
        }
    }

    public void InitNetworking() {

        Debug.Log("Spawning networkcontroller");

        //Spawn the  client networking manager for our local player (and let the opponent spawn their own controller)
        GameObject goNetworkController = PhotonNetwork.Instantiate("Prefabs/Networking/pfNetworkController", Vector3.zero, Quaternion.identity);

        if(goNetworkController = null) {
            Debug.LogError("No prefab found for network controller");
        }
    }

    public void AssignLocalInputController(Player plyr) {

        //If the player isn't controlled locally, just set the plyr's controller to null since it's not our job to control them
        if(NetworkMatchSetup.IsLocallyOwned(plyr.id) == false) {
            plyr.SetInputType(LocalInputType.InputType.NONE);
        } else {
            //Otherwise, this character is controlled by this local client - figure out which input type they'll need and add it
            plyr.SetInputType(NetworkMatchSetup.GetInputType(plyr.id));
        }
    }

    public IEnumerator SetupMatch() {

        while(NetworkMatchSetup.HasAllMatchSetupInfo() == false) {
            //Spin until we have all the match setup info that we need to start the match
            yield return null;
        }

        while(NetworkMatchSender.Get() == null) {
            //Spin until the needed networking objects have been spawned and given a chance to initialize
            // - in particular, the NetworkMatchSender needs to be ready in case we need to immediately send inputs
            //    as part of initializing the match (like for loading a log file)
            yield return null;
        }

        Debug.Log("Starting match initializations since we have enough information");

        ContRandomization.Get().InitGenerator(NetworkMatchSetup.GetRandomizationSeed());

        Debug.Log("Finished initializing the randomizer");

        InitPlayers();

        Debug.Log("Finished initializing players");

        //Initialize characters 
        InitAllChrs();

        Debug.Log("After InitAllChrs");

        //Assign local input controllers for each player
        AssignAllLocalInputControllers();

        Debug.Log("After assigning local input controllers");

        ContManaDistributer.Get().InitializeRandomReserves();

        Debug.Log("After initializing mana reserves");

        matchresult = ContDeaths.Get().CheckMatchWinner();

        Debug.LogFormat("Initial match result set to {0}", matchresult);

        //Check if the LogManager wants to load in any starting inputs
        LogManager.Get().LoadStartingInputs();

        Debug.Log("After LoadStartingInputs");

        LogManager.Get().InitMatchLog();

        Debug.Log("Finished initializing the log file");
    }


    public void Start() {
        if(bStarted) {
            return;
        }
        bStarted = true;

        gameObject.tag = "Match"; // So that anything can find this very quickly

        Cursor.SetCursor(txCursor, v2HotSpot, cursorMode);

        //Do all the match setup stuff (once it is ready)
        StartCoroutine(SetupMatch());
    }
}
