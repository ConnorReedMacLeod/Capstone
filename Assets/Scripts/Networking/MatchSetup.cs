using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;


//Used to craft and manage the parameters with which a match is constructed (so character choices,
//  starting positions, loadouts, etc.)  
public class MatchSetup : SingletonPersistent<MatchSetup> {

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
        public Position.Coords[][] arPositionCoordsSelections;

        public override string ToString() {

            //If we haven't filled anything out yet, just return null
            if (arnPlayersOwners == null) return "(matchparams) null";

            string s = "";

            for (int i=0; i<Player.MAXPLAYERS; i++) {

                string sPlayer = string.Format("Player {0}:\nOwner = {1}, InputType = {2}\n", i, arnPlayersOwners[i], arInputTypes[i]);

                if (arChrSelections[i] == null) {

                    s += "null\n";

                } else {

                    for (int j = 0; j < arChrSelections[i].Length; j++) {
                        sPlayer += string.Format("{0} ({1}), {2}\n", arChrSelections[i][j], arPositionCoordsSelections[i][j], arLoadoutSelections[i][j]);
                    }
                    s += sPlayer;
                }
            }

            return s;
        }

        public void UpdateChrSelectionsForClient(int idClient, MatchParams matchparamsOther) {
            for (int i = 0; i < arnPlayersOwners.Length; i++) {
                //if the passed client owns this player, we'll copy over this entry from the passed matchparams
                if (arnPlayersOwners[i] == idClient) {
                    UpdateChrSelectionsForPlayer(i, matchparamsOther.arChrSelections[i]);
                }
            }
        }

        public void UpdateChrSelectionsForPlayer(int idPlayer, CharType.CHARTYPE[] arChrSelectionsForPlayer) {
            arChrSelections[idPlayer] = arChrSelectionsForPlayer;
        }

        public void UpdateLoadoutsForClient(int idClient, MatchParams matchparamsOther) {
            for (int i = 0; i < arnPlayersOwners.Length; i++) {
                //if the passed client owns this player, we'll copy over this entry from the passed matchparams
                if (arnPlayersOwners[i] == idClient) {
                    UpdateLoadoutsForPlayer(i, matchparamsOther.arLoadoutSelections[i]);
                }
            }
        }

        public void UpdateLoadoutsForPlayer(int idPlayer, LoadoutManager.Loadout[] arLoadoutsForPlayer) {
            arLoadoutSelections[idPlayer] = arLoadoutsForPlayer;
        }

        public void UpdatePositionCoordsForClient(int idClient, MatchParams matchparamsOther) {
            for(int i=0; i<arnPlayersOwners.Length; i++) {
                //if the passed client owns this player, we'll copy over this entry from the passed matchparams
                if(arnPlayersOwners[i] == idClient) {
                    UpdatePositionCoordsForPlayer(i, matchparamsOther.arPositionCoordsSelections[i]);
                }
            }
        }

        public void UpdatePositionCoordsForPlayer(int idPlayer, Position.Coords[] arPositionCoordsForPlayer) {
            arPositionCoordsSelections[idPlayer] = arPositionCoordsForPlayer;
        }

        public void UpdatePlayerOwners(int[] _arnPlayersOwners) {
            arnPlayersOwners = _arnPlayersOwners;
        }

        public void UpdateInputTypes(Player.InputType[] _arInputTypes) {
            arInputTypes = _arInputTypes;
        }

        //Copy in all relevent fields of the passed MatchParams that are relevent for starting a draft
        public void CopyForDraftStart(MatchParams other) {
            arnPlayersOwners = other.arnPlayersOwners;
            arInputTypes = other.arInputTypes;
        }

        //Copy in all relevent fields of the passed MatchParams that are relevent for moving to the loadout phase
        public void CopyForLoadoutStart(MatchParams other) {
            CopyForDraftStart(other);
            arChrSelections = other.arChrSelections;
        }

        //Copy in all relevent fields of the passed MatchParams that are relevent for moving to a match
        public void CopyForMatchStart(MatchParams other) {
            //Just copy the full matchParams
            CopyForLoadoutStart(other);
            arLoadoutSelections = other.arLoadoutSelections;
            arPositionCoordsSelections = other.arPositionCoordsSelections;
        }

        //Create as blank a match params as possible
        public MatchParams() {
            arChrSelections = new CharType.CHARTYPE[Player.MAXPLAYERS][];
            arLoadoutSelections = new LoadoutManager.Loadout[arChrSelections.Length][];
            arPositionCoordsSelections = new Position.Coords[arChrSelections.Length][];

            arnPlayersOwners = null;
            arInputTypes = null;
        }

        public MatchParams(CharType.CHARTYPE[][] _arChrSelections, LoadoutManager.Loadout[][] _arLoadoutSelections,
            int[] _arnPlayersOwners, Player.InputType[] _arInputTypes, Position.Coords[][] _arPositionCoordsSelections) {
            arChrSelections = _arChrSelections;
            arLoadoutSelections = _arLoadoutSelections;
            arnPlayersOwners = _arnPlayersOwners;
            arInputTypes = _arInputTypes;
            arPositionCoordsSelections = _arPositionCoordsSelections;
        }



    }


    //Create a basic filled-out match params 
    public static MatchParams CreateDefaultMatchParams() {


        CharType.CHARTYPE[][] arChrSelections = new CharType.CHARTYPE[Player.MAXPLAYERS][];
        LoadoutManager.Loadout[][] arLoadoutSelections = new LoadoutManager.Loadout[arChrSelections.Length][];
        Position.Coords[][] arPositionCoordsSelections = new Position.Coords[arChrSelections.Length][];
        for (int i = 0; i < arChrSelections.Length; i++) {
            arChrSelections[i] = new CharType.CHARTYPE[] { CharType.CHARTYPE.FISCHER, CharType.CHARTYPE.KATARINA, CharType.CHARTYPE.PITBEAST };
            arLoadoutSelections[i] = new LoadoutManager.Loadout[arChrSelections[i].Length];
            arPositionCoordsSelections[i] = new Position.Coords[arChrSelections[i].Length];

            for (int j = 0; j < arChrSelections[i].Length; j++) {
                arLoadoutSelections[i][j] = LoadoutManager.GetDefaultLoadoutForChar(arChrSelections[i][j]);
                arPositionCoordsSelections[i][j] = Position.GetDefaultPositionCoords(i, j);
            }
        }

        int nLocalClientID = ClientNetworkController.Get().nLocalClientID;

        //By default, assume we are locally controlling both players - can override as needed
        int[] arnPlayersOwners = new int[Player.MAXPLAYERS];
        Player.InputType[] arInputTypes = new Player.InputType[Player.MAXPLAYERS];
        for (int i = 0; i < Player.MAXPLAYERS; i++) {
            arnPlayersOwners[i] = nLocalClientID;
            arInputTypes[i] = Player.InputType.HUMAN;
        }

        return new MatchParams(arChrSelections, arLoadoutSelections, arnPlayersOwners, arInputTypes, arPositionCoordsSelections);
    }

    public static object[] SerializeMatchParams (MatchParams matchparams) {
        object[] arSerialized = new object[5];

        arSerialized[0] = LibConversions.ArArChrTypeToArArInt(matchparams.arChrSelections);
        arSerialized[1] = LoadoutManager.SerializeAllPlayersLoadouts(matchparams.arLoadoutSelections);
        arSerialized[2] = LibConversions.ArIntToArObj(matchparams.arnPlayersOwners);
        arSerialized[3] = LibConversions.ArInputTypeToArObj(matchparams.arInputTypes);
        arSerialized[4] = LibConversions.ArArPositionCoordToArArInt(matchparams.arPositionCoordsSelections);

        return arSerialized;
    }

    public static MatchParams UnserializeMatchParams (object[] arSerialized) {

        return new MatchParams(
            LibConversions.ArArIntToArArChrType((int[][])arSerialized[0]),
            LoadoutManager.UnserializeAllPlayersLoadouts((int[][][])arSerialized[1]),
            LibConversions.ArObjToArInt((object[])arSerialized[2]),
            LibConversions.ArObjToArInputType((object[])arSerialized[3]),
            LibConversions.ArArIntToArArPositionCoord((int[][])arSerialized[4])
            );
    }


    public override void Init() {

        curMatchParams = new MatchParams();

        int nLocalIDController = ClientNetworkController.Get().nLocalClientID;

        //Set the defaults for the curMatchParams to include player owners and input types
        curMatchParams.UpdatePlayerOwners(new int[] { nLocalIDController, nLocalIDController });
        curMatchParams.UpdateInputTypes(new Player.InputType[] { Player.InputType.HUMAN, Player.InputType.AI });

        Debug.Log("Finished CharacterSelection.Init");
    }


    // Send a complete matchparams to the master to move to the start of the draft
    public void SubmitLocalMatchParamsAndStartDraft() {

        Debug.Log("Client is submitting an initially filled (with owners/input types) match params (and requesting to start the draft): " + curMatchParams);

        NetworkConnectionManager.SendEventToMaster(MasterNetworkController.evtMStartDraft, SerializeMatchParams(curMatchParams));
    }

    // Send a complete matchparams to the master to jump directly to the loadout phase
    public void SubmitLocalMatchParamsAndDirectlyStartLoadout() {

        Debug.Log("Client is submitting a match params that is (supposedly) filled with chr selections (and requesting to directly start the loadout phase): " + curMatchParams);

        NetworkConnectionManager.SendEventToMaster(MasterNetworkController.evtMSubmitMatchParamsAndDirectlyStartLoadout, SerializeMatchParams(curMatchParams));
    }

    // Send a complete matchparams to the master to jump directly to the start of a match
    public void SubmitLocalMatchParamsAndDirectlyStartMatch() {

        Debug.Log("Client is submitting a supposedly completed match params (and requesting to directly start the match): " + curMatchParams);

        NetworkConnectionManager.SendEventToMaster(MasterNetworkController.evtMSubmitMatchParamsAndDirectlyStartMatch, SerializeMatchParams(curMatchParams));
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
            plyr.SetInputType((Player.InputType)curMatchParams.arInputTypes[plyr.id]);
        }
    }

}
