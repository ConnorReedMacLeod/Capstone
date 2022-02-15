using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

// Used for storing and receiving match setup parameters (primarily through
//   stored room properties to ensure consistency)
public static class NetworkMatchSetup {


    // Randomization Seed
    public static string GetRandomizationSeedKey() {
        return "rnd";
    }

    public static void SetRandomizationSeed(int nSeed) {
        if (PhotonNetwork.IsMasterClient == false) return; //Only allow the master to set the randomization key

        ExitGames.Client.Photon.Hashtable hashNewProperties = new ExitGames.Client.Photon.Hashtable() { { GetRandomizationSeedKey(), nSeed } };

        PhotonNetwork.CurrentRoom.SetCustomProperties(hashNewProperties);
    }

    public static int GetRandomizationSeed() {
        return (int)PhotonNetwork.CurrentRoom.CustomProperties[GetRandomizationSeedKey()];
    }

    // Player Owner
    public static string GetPlayerOwnerKey(int idPlayer) {
        return string.Format("po{0}", idPlayer);
    }

    public static void SetPlayerOwner(int idPlayer, int idClient) {
        if (PhotonNetwork.IsMasterClient == false) return; //Only allow the master to do modifications for controller params

        ExitGames.Client.Photon.Hashtable hashNewProperties = new ExitGames.Client.Photon.Hashtable() { { GetPlayerOwnerKey(idPlayer), idClient } };

        PhotonNetwork.CurrentRoom.SetCustomProperties(hashNewProperties);
    }

    public static int GetPlayerOwner(int idPlayer) {
        return (int)PhotonNetwork.CurrentRoom.CustomProperties[GetPlayerOwnerKey(idPlayer)];
    }

    public static bool IsLocallyOwned(int idPlayer) {
        return PhotonNetwork.LocalPlayer.ActorNumber == GetPlayerOwner(idPlayer);
    }


    // Input Type
    public static string GetInputTypeKey(int idPlayer) {
        return string.Format("it{0}", idPlayer);
    }

    public static void SetInputType(int idPlayer, Player.InputType inputtype) {
        if (PhotonNetwork.IsMasterClient == false) return; //Only allow the master to do modifications for controller params

        ExitGames.Client.Photon.Hashtable hashNewProperties = new ExitGames.Client.Photon.Hashtable() { { GetInputTypeKey(idPlayer), inputtype } };

        PhotonNetwork.CurrentRoom.SetCustomProperties(hashNewProperties);
    }

    public static Player.InputType GetInputType(int idPlayer) {
        return (Player.InputType)PhotonNetwork.CurrentRoom.CustomProperties[GetInputTypeKey(idPlayer)];
    }


    // Character Selections
    public static string GetCharSelectionsKey(int idPlayer, int idChar) {
        return string.Format("cs{0}-{1}", idPlayer, idChar);
    }

    public static void SetCharacterSelection(int idPlayer, int iChrSlot, CharType.CHARTYPE chartype) {

        Debug.LogFormat("Setting character selection for player {0}'s {1}th character to {2}",
                idPlayer, iChrSlot, chartype);

        ExitGames.Client.Photon.Hashtable hashNewProperties = new ExitGames.Client.Photon.Hashtable() { { GetCharSelectionsKey(idPlayer, iChrSlot), chartype } };

        PhotonNetwork.CurrentRoom.SetCustomProperties(hashNewProperties);
    }

    public static bool HasEntryCharacterSelection(int idPlayer, int iChrSlot) {
        return PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(GetCharSelectionsKey(idPlayer, iChrSlot));
    }

    public static CharType.CHARTYPE GetCharacterSelection(int idPlayer, int iChrSlot) {
        return (CharType.CHARTYPE)PhotonNetwork.CurrentRoom.CustomProperties[GetCharSelectionsKey(idPlayer, iChrSlot)];
    }


    // Loadouts
    public static string GetLoadoutKey(int idPlayer, int idChar) {
        return string.Format("lo{0}-{1}", idPlayer, idChar);
    }

    public static void SetLoadout(int idPlayer, int iChrSlot, LoadoutManager.Loadout loadout) {

        ExitGames.Client.Photon.Hashtable hashNewProperties = new ExitGames.Client.Photon.Hashtable() { { GetLoadoutKey(idPlayer, iChrSlot), LoadoutManager.SerializeLoadout(loadout) } };

        PhotonNetwork.CurrentRoom.SetCustomProperties(hashNewProperties);
    }

    public static bool HasEntryLoadout(int idPlayer, int iChrSlot) {
        return PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(GetLoadoutKey(idPlayer, iChrSlot));
    }

    public static LoadoutManager.Loadout GetLoadout(int idPlayer, int iChrSlot) {
        return LoadoutManager.UnserializeLoadout((int[])PhotonNetwork.CurrentRoom.CustomProperties[GetLoadoutKey(idPlayer, iChrSlot)]);
    }


    // Starting Position Coords
    public static string GetPositionCoordsKey(int idPlayer, int idChar) {
        return string.Format("pc{0}-{1}", idPlayer, idChar);
    }

    public static void SetPositionCoords(int idPlayer, int iChrSlot, Position.Coords positionCoords) {
        ExitGames.Client.Photon.Hashtable hashNewProperties = new ExitGames.Client.Photon.Hashtable() { { GetPositionCoordsKey(idPlayer, iChrSlot), Position.SerializeCoords(positionCoords) } };

        PhotonNetwork.CurrentRoom.SetCustomProperties(hashNewProperties);
    }

    public static bool HasEntryPositionCoords(int idPlayer, int iChrSlot) {
        return PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(GetLoadoutKey(idPlayer, iChrSlot));
    }

    public static  Position.Coords GetPositionCoords(int idPlayer, int iChrSlot) {
        return Position.UnserializeCoords((int)PhotonNetwork.CurrentRoom.CustomProperties[GetPositionCoordsKey(idPlayer, iChrSlot)]);
    }


    public static string MatchSetupToString() {
        
        string s = string.Format("Seed: {0}\n", GetRandomizationSeed());

        for (int i = 0; i < Player.MAXPLAYERS; i++) {

            string sPlayer = string.Format("Player {0}:\nOwner = {1}, InputType = {2}\n", i, GetPlayerOwner(i), GetInputType(i));

            
            s += "Character Selections:\n";

            for (int j = 0; j < Player.MAXCHRS; j++) {

                sPlayer += string.Format("{0} ({1}), {2}\n",
                    HasEntryCharacterSelection(i, j) ? GetCharacterSelection(i, j).ToString() : "Null",
                    HasEntryPositionCoords(i, j) ? GetPositionCoords(i, j).ToString() : "Null",
                    HasEntryLoadout(i, j) ? GetLoadout(i, j).ToString() : "Null");
            }
            s += sPlayer;
            
        }

        return s;
    }

    public static bool HasAllMatchSetupInfo() {
        //Note that we always assume that there will be a default entry for the randomization seed, player owners and input types for all players
        //  We'll also only be checking that there are enough character selections to start a match with (you can potentially draft
        //  more than this though)

        for(int i=0; i<Player.MAXPLAYERS; i++) {

            for(int j=0; j<Player.MAXCHRS; j++) {
                if(HasEntryCharacterSelection(i, j) == false) {
                    Debug.LogFormat("Still waiting on char selection {1} for player {0}", i, j);
                    return false;
                }
                if (HasEntryCharacterSelection(i, j) == false) {
                    Debug.LogFormat("Still waiting on char selection {1} for player {0}", i, j);
                    return false;
                }
            }

        }
        return true;

    }
    
}
