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
        if(PhotonNetwork.IsMasterClient == false) return; //Only allow the master to set the randomization key

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
        if(PhotonNetwork.IsMasterClient == false) return; //Only allow the master to do modifications for controller params

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

    public static void SetInputType(int idPlayer, LocalInputType.InputType inputtype) {
        if(PhotonNetwork.IsMasterClient == false) return; //Only allow the master to do modifications for controller params

        ExitGames.Client.Photon.Hashtable hashNewProperties = new ExitGames.Client.Photon.Hashtable() { { GetInputTypeKey(idPlayer), inputtype } };

        PhotonNetwork.CurrentRoom.SetCustomProperties(hashNewProperties);
    }

    public static LocalInputType.InputType GetInputType(int idPlayer) {
        return (LocalInputType.InputType)PhotonNetwork.CurrentRoom.CustomProperties[GetInputTypeKey(idPlayer)];
    }


    //Drafted Characters
    public static string GetDraftedCharactersKey(int idPlayer, int idChar) {
        return string.Format("dc{0}-{1}", idPlayer, idChar);
    }

    public static void SetDraftedCharacter(int idPlayer, int iChrSlot, CharType.CHARTYPE chartype) {

        Debug.LogFormat("Setting drafted character for player {0}'s {1}th character to {2}",
                idPlayer, iChrSlot, chartype);

        ExitGames.Client.Photon.Hashtable hashNewProperties = new ExitGames.Client.Photon.Hashtable() { { GetDraftedCharactersKey(idPlayer, iChrSlot), chartype } };

        PhotonNetwork.CurrentRoom.SetCustomProperties(hashNewProperties);
    }

    public static bool HasEntryDraftedCharacter(int idPlayer, int iChrSlot) {
        return PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(GetDraftedCharactersKey(idPlayer, iChrSlot));
    }

    public static CharType.CHARTYPE GetDraftedCharacter(int idPlayer, int iChrSlot) {
        return (CharType.CHARTYPE)PhotonNetwork.CurrentRoom.CustomProperties[GetDraftedCharactersKey(idPlayer, iChrSlot)];
    }


    // Character Ordering
    // - First N characters are active and in order of activation (starting fatigue)
    // - Next M Characters start on the bench (with their order dictating their bench position)
    // - Any remaining drafted characters should not be included in this list since they won't be used in the match
    public static string GetCharacterOrderingKey(int idPlayer, int idChar) {
        return string.Format("co{0}-{1}", idPlayer, idChar);
    }

    public static void SetCharacterOrdering(int idPlayer, int iChrSlot, CharType.CHARTYPE chartype) {

        Debug.LogFormat("Setting character ordering for player {0}'s {1}th character to {2}",
                idPlayer, iChrSlot, chartype);

        string sKey = GetCharacterOrderingKey(idPlayer, iChrSlot);

        ExitGames.Client.Photon.Hashtable hashNewProperties = new ExitGames.Client.Photon.Hashtable() { { sKey, chartype } };

        bool bSuccess = PhotonNetwork.CurrentRoom.SetCustomProperties(hashNewProperties);
        Debug.LogFormat("Were room properties set successfully?: {0}", bSuccess);
        Debug.LogFormat("hash's entry for {0} is {1}, while roomproperties' is {2}", sKey, hashNewProperties[sKey], PhotonNetwork.CurrentRoom.CustomProperties[sKey]);
    }

    public static bool HasEntryCharacterOrdering(int idPlayer, int iChrSlot) {
        return PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(GetCharacterOrderingKey(idPlayer, iChrSlot));
    }

    public static CharType.CHARTYPE GetCharacterOrdering(int idPlayer, int iChrSlot) {
        Debug.LogFormat("Requesting character ordering for player {0}'s {1}th",
                idPlayer, iChrSlot);
        Debug.LogFormat("Requesting roomproperty for {0}", GetCharacterOrderingKey(idPlayer, iChrSlot));
        Debug.LogFormat("Current Room is " + PhotonNetwork.CurrentRoom);
        return (CharType.CHARTYPE)PhotonNetwork.CurrentRoom.CustomProperties[GetCharacterOrderingKey(idPlayer, iChrSlot)];
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

    public static Position.Coords GetPositionCoords(int idPlayer, int iChrSlot) {
        return Position.UnserializeCoords((int)PhotonNetwork.CurrentRoom.CustomProperties[GetPositionCoordsKey(idPlayer, iChrSlot)]);
    }


    public static string MatchSetupToString() {

        string s = string.Format("Seed: {0}\n", GetRandomizationSeed());

        for(int i = 0; i < Match.NPLAYERS; i++) {

            string sPlayer = string.Format("Player {0}:\nOwner = {1}, InputType = {2}\n", i, GetPlayerOwner(i), GetInputType(i));


            s += "Character Selections:\n";

            for(int j = 0; j < Match.NINITIALCHRSPERTEAM; j++) {

                sPlayer += string.Format("{0} ({1}), {2}\n",
                    HasEntryCharacterOrdering(i, j) ? GetCharacterOrdering(i, j).ToString() : "Null",
                    HasEntryPositionCoords(i, j) ? GetPositionCoords(i, j).ToString() : "Null",
                    HasEntryLoadout(i, j) ? GetLoadout(i, j).ToString() : "Null");
            }
            s += sPlayer;

        }

        return s;
    }

    public static bool HasAllMatchSetupInfo() {
        //Note that we always assume that there will be a default entry for player owners and input types for all players
        // We'll check if every character has a character ordering, a loadout, and a starting position

        for(int i = 0; i < Match.NPLAYERS; i++) {

            for(int j = 0; j < Match.NINITIALCHRSPERTEAM; j++) {
                if(HasEntryCharacterOrdering(i, j) == false) {
                    Debug.LogFormat("Still waiting on char selection {1} for player {0}", i, j);
                    return false;
                }
                if(HasEntryLoadout(i, j) == false) {
                    Debug.LogFormat("Still waiting on loadout {1} for player {0}", i, j);
                    return false;
                }
                if(j < Match.NMINACTIVECHRSPERTEAM && HasEntryPositionCoords(i, j) == false) {
                    Debug.LogFormat("Still waiting on starting position {1} for player {0}", i, j);
                    return false;
                }
            }

        }
        return true;

    }

}
