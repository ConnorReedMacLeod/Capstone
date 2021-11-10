using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;


//Used to craft and manage the parameters with which a match is constructed (so character choices,
//  starting positions, loadouts, etc.)  
public class MatchSetup : SingletonPersistent<MatchSetup> {

    //So that these can be easily configured in the Unity inspector
    public CharType.CHARTYPE[] arChrVIEWABLE1 = new CharType.CHARTYPE[Player.MAXCHRS];
    public CharType.CHARTYPE[] arChrVIEWABLE2 = new CharType.CHARTYPE[Player.MAXCHRS];

    //Keep locally configured contributions for a match setup in these variables which we can submit to the master client
    public CharType.CHARTYPE[][] arLocalChrSelections = new CharType.CHARTYPE[Player.MAXPLAYERS][];
    public LoadoutManager.Loadout[][] arLocalLoadoutSelections = new LoadoutManager.Loadout[Player.MAXPLAYERS][];
    public int[] arnLocalPlayerOwners = new int[Player.MAXPLAYERS];
    public Player.InputType[] arLocalInputTypes = new Player.InputType[Player.MAXPLAYERS];
    public Position[][] arLocalStartingPosition = new Position[Player.MAXPLAYERS][];


    public MatchParams curMatchParams; //Holds the params forming the context of the match (if one is currently ongoing)
 

    //Hold all the information needed to start a match
    public class MatchParams {
        //Which characters have been picked for each player
        public CharType.CHARTYPE[][] arChrSelections;
        //Which loadouts those characters are using
        public LoadoutManager.Loadout[][] arLoadoutSelections;
        //Which client owns which player
        public int[] arnPlayersOwners;
        //Which type of input each player is using (locally controlled, AI, foreign controlled, etc.)
        public Player.InputType[] arInputTypes;
        //The starting positions of each character
        public Position[][] arPositionSelections;

        public override string ToString() {

            string s = "";

            for (int i=0; i<Player.MAXPLAYERS; i++) {

                string sPlayer = string.Format("Player {0}:\nOwner = {1}, InputType = {2}\n", i, arnPlayersOwners[i], arInputTypes[i]);
                
                for(int j=0; j<arChrSelections[i].Length; j++) {
                    sPlayer += string.Format("{0} ({1}), {2}\n", arChrSelections[i][j], arPositionSelections[i][j], arLoadoutSelections[i][j]);
                }
                s += sPlayer;
            }

            return s;
        }

        //For setting up a 'default' match params to be filled in later
        public MatchParams() {

            arChrSelections = new CharType.CHARTYPE[Player.MAXPLAYERS][];
            arLoadoutSelections = new LoadoutManager.Loadout[arChrSelections.Length][];
            arPositionSelections = new Position[arChrSelections.Length][];
            for (int i=0; i<arChrSelections.Length; i++) {
                arChrSelections[i] = new CharType.CHARTYPE[] { CharType.CHARTYPE.FISCHER, CharType.CHARTYPE.KATARINA , CharType.CHARTYPE.PITBEAST };
                arLoadoutSelections[i] = new LoadoutManager.Loadout[arChrSelections[i].Length];
                arPositionSelections[i] = new Position[arChrSelections[i].Length];

                for (int j = 0; j < arChrSelections[i].Length; j++) {
                    arLoadoutSelections[i][j] = LoadoutManager.GetDefaultLoadoutForChar(arChrSelections[i][j]);
                    arPositionSelections[i][j] = Position.GetDefaultPosition(i, j);
                }
            }

            int nLocalClientID = ClientNetworkController.Get().nLocalClientID;

            //By default, assume we are locally controlling both players - can override as needed
            arnPlayersOwners = new int[Player.MAXPLAYERS];
            arInputTypes = new Player.InputType[Player.MAXPLAYERS];
            for(int i=0; i<Player.MAXPLAYERS; i++) {
                arnPlayersOwners[i] = nLocalClientID;
                arInputTypes[i] = Player.InputType.HUMAN;
            }
            
        }

        public MatchParams(CharType.CHARTYPE[][] _arChrSelections, LoadoutManager.Loadout[][] _arLoadoutSelections,
            int[] _arnPlayersOwners, Player.InputType[] _arInputTypes, Position[][] _arPositionSelections) {
            arChrSelections = _arChrSelections;
            arLoadoutSelections = _arLoadoutSelections;
            arnPlayersOwners = _arnPlayersOwners;
            arInputTypes = _arInputTypes;
            arPositionSelections = _arPositionSelections;
        }

    }

    public static object[] SerializeMatchParams (MatchParams matchparams) {
        object[] arSerialized = new object[5];

        arSerialized[0] = LibConversions.ArArChrTypeToArArInt(matchparams.arChrSelections);
        arSerialized[1] = LoadoutManager.SerializeAllPlayersLoadouts(matchparams.arLoadoutSelections);
        arSerialized[2] = LibConversions.ArIntToArObj(matchparams.arnPlayersOwners);
        arSerialized[3] = LibConversions.ArInputTypeToArObj(matchparams.arInputTypes);
        arSerialized[4] = LibConversions.ArArPositionsToArArInt(matchparams.arPositionSelections);

        return arSerialized;
    }

    public static MatchParams UnserializeMatchParams (object[] arSerialized) {

        return new MatchParams(
            LibConversions.ArArIntToArArChrType((int[][])arSerialized[0]),
            LoadoutManager.UnserializeAllPlayersLoadouts((int[][][])arSerialized[1]),
            LibConversions.ArObjToArInt((object[])arSerialized[2]),
            LibConversions.ArObjToArInputType((object[])arSerialized[3]),
            LibConversions.ArArIntsToArArPositions((int[][])arSerialized[4])
            );
    }


    public override void Init() {

        arLocalChrSelections[0] = new CharType.CHARTYPE[Player.MAXCHRS];
        arLocalChrSelections[1] = new CharType.CHARTYPE[Player.MAXCHRS];
        arChrVIEWABLE1.CopyTo(arLocalChrSelections[0], 0);
        arChrVIEWABLE2.CopyTo(arLocalChrSelections[1], 0);
        
        for (int i = 0; i < arLocalLoadoutSelections.Length; i++) {
            arLocalLoadoutSelections[i] = new LoadoutManager.Loadout[arLocalChrSelections[i].Length];
            arLocalStartingPosition[i] = new Position[arLocalChrSelections[i].Length];
            for(int j=0; j<arLocalLoadoutSelections[i].Length; j++) {
                arLocalLoadoutSelections[i][j] = LoadoutManager.GetDefaultLoadoutForChar(arLocalChrSelections[i][j]);
                arLocalStartingPosition[i][j] = Position.GetDefaultPosition(i, j);
            }
        }

        Debug.Log("Finished CharacterSelection.Init");
    }

    // Convert our locally stored setup-params into a MatchParams and send it to the master to be used for a match
    public void SubmitLocalMatchParams() {

        MatchParams matchparamsToSend = new MatchParams(
            arLocalChrSelections,
            arLocalLoadoutSelections,
            arnLocalPlayerOwners,
            arLocalInputTypes,
            arLocalStartingPosition
            );

        Debug.Log("Client is submitting " + matchparamsToSend);

        NetworkConnectionManager.SendEventToMaster(MasterNetworkController.evtMSubmitMatchParams, SerializeMatchParams(matchparamsToSend));
    }

    public void SubmitLocalMatchParamsAndStartMatch() {
        MatchParams matchparamsToSend = new MatchParams(
            arLocalChrSelections,
            arLocalLoadoutSelections,
            arnLocalPlayerOwners,
            arLocalInputTypes,
            arLocalStartingPosition
            );

        Debug.Log("Client is submitting (and requesting to start): " + matchparamsToSend);

        NetworkConnectionManager.SendEventToMaster(MasterNetworkController.evtMSubmitMatchParamsAndStartMatch, SerializeMatchParams(matchparamsToSend));
    }


    // When the master has sent out out the information for forming a match, we'll save it locally
    //  and set up anything relevant for the start of the match
    public void SaveMatchParams(MatchParams matchparamsNew) {
        curMatchParams = matchparamsNew;
        Debug.Log("We've received a new MatchParams: " + curMatchParams);
    }


    public IEnumerator AssignAllLocalInputControllers() {
        //Sleep until we've recieved enough information to actually assign input controllers
        while(curMatchParams == null) {
            Debug.Log("Waiting to assign input controllers until matchparams have been received from the masted");
            yield return null;
        }

        for(int i = 0; i < Player.MAXPLAYERS; i++) {
            AssignLocalInputController(Match.Get().arPlayers[i]);
        }
    }

    public void AssignLocalInputController(Player plyr) {

        //If the player isn't controlled locally, just set the plyr's controller to null since it's not our job to control them
        if(ClientNetworkController.Get().IsPlayerLocallyControlled(plyr) == false) {
            plyr.SetInputType(Player.InputType.NONE);
        } else {
            //Otherwise, this character is controlled by this local client - figure out which input type they'll need and add it
            plyr.SetInputType((Player.InputType)arLocalInputTypes[plyr.id]);
        }
    }

}
