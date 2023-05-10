using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillType.SKILLTYPE;

public static class LoadoutManager {

    public const int nLOADOUTSLOTS = 3;
    public const string SKEYNAME = "loadout{0}-{1}-name";
    public const string SKEYSKILLS = "loadout{0}-{1}-skills-{2}";

    public struct Loadout {
        public string sName;

        public List<SkillType.SKILLTYPE> lstChosenSkills;

        public Loadout(string _sName, List<SkillType.SKILLTYPE> _lstChosenSkills) {
            sName = _sName;
            lstChosenSkills = _lstChosenSkills;
        }

        public int NumEquippedSkills() {
            int nSkills = 0;
            for(int i=0; i<Chr.nMaxEquippedChosenSkills; i++) {
                if (lstChosenSkills[i] == SkillType.SKILLTYPE.NULL) break;
                nSkills++;
            }
            return nSkills;
        }

        public int NumBenchedSkills() {
            int nSkills = 0;
            for (int i = Chr.nMaxEquippedChosenSkills; i < Chr.nMaxTotalChosenSkills; i++) {
                if (lstChosenSkills[i] == SkillType.SKILLTYPE.NULL) break;
                nSkills++;
            }
            return nSkills;
        }

        public override string ToString() {
            string sLoadout = string.Format("{0}:\nEquipped: {1}, {2}, {3}, {4}\nBench: {5}, {6}, {7}, {8}",
                sName,
                lstChosenSkills[0], lstChosenSkills[1], lstChosenSkills[2], lstChosenSkills[3],
                lstChosenSkills[4], lstChosenSkills[5], lstChosenSkills[6], lstChosenSkills[7]);

            return sLoadout;
        }
    }

    public static int[] SerializeLoadout(Loadout loadout) {
        int[] arSerialized = new int[Chr.nMaxTotalChosenSkills];

        int iSerialized = 0;
        for(int i = 0; i < arSerialized.Length; i++, iSerialized++) {
            arSerialized[iSerialized] = (int)loadout.lstChosenSkills[i];
        }

        return arSerialized;
    }

    public static Loadout UnserializeLoadout(int[] arSerialized, string sLoadoutName = "") {

        List<SkillType.SKILLTYPE> lstChosenSkills = new List<SkillType.SKILLTYPE>();

        int iDeserialized = 0;
        for(int i = 0; i < Chr.nMaxTotalChosenSkills; i++, iDeserialized++) {
            lstChosenSkills.Add((SkillType.SKILLTYPE)arSerialized[iDeserialized]);
        }

        return new Loadout(sLoadoutName, lstChosenSkills);
    }

    
    public static int[][] SerializePlayerLoadouts(Loadout[] arLoadouts) {
        int[][] ararnLoadoutSelection = new int[arLoadouts.Length][];

        for(int i = 0; i < arLoadouts.Length; i++) {
            ararnLoadoutSelection[i] = SerializeLoadout(arLoadouts[i]);
        }

        return ararnLoadoutSelection;
    }

    public static Loadout[] UnserializePlayerLoadouts(int[][] ararLoadoutSelection) {
        Loadout[] arLoadouts = new Loadout[ararLoadoutSelection.Length];

        for (int i=0; i<ararLoadoutSelection.Length; i++) {
            arLoadouts[i] = UnserializeLoadout(ararLoadoutSelection[i]);
        }

        return arLoadouts;
    }

    public static int[][][] SerializeAllPlayersLoadouts(Loadout[][] ararLoadouts) {
        int[][][] arararnLoadoutSelection = new int[ararLoadouts.Length][][];

        for (int i = 0; i < ararLoadouts.Length; i++) {
            arararnLoadoutSelection[i] = SerializePlayerLoadouts(ararLoadouts[i]);
        }

        return arararnLoadoutSelection;
    }

    public static Loadout[][] UnserializeAllPlayersLoadouts(int[][][] arararLoadoutSelection) {
        Loadout[][] ararLoadouts = new Loadout[arararLoadoutSelection.Length][];

        for (int i=0; i<arararLoadoutSelection.Length; i++) {
            ararLoadouts[i] = UnserializePlayerLoadouts(arararLoadoutSelection[i]);
        }

        return ararLoadouts;
    }
   

    public static string GetKeyName(CharType.CHARTYPE chartype, int iSlot) {
        return string.Format(SKEYNAME, (int)chartype, iSlot);
    }

    public static List<string> LoadAllLoadoutNamesForChr(CharType.CHARTYPE chartype) {
        List<string> lstLoadoutNames = new List<string>();

        for (int i = 0; i < nLOADOUTSLOTS; i++) {
            if (PlayerPrefs.HasKey(GetKeyName(chartype, i)) == false) {
                //If we try to load the ith slot for this character, but there's no entry for the name of the loadout,
                //  we assume the loadout is blank so we can just save in the default loadout for the character then use it
                Debug.LogFormat("No loadout names for {0} - setting defaults", chartype);

                Loadout loadoutDefault = GetDefaultLoadoutForChar(chartype);
                SaveLoadout(chartype, i, loadoutDefault);

                lstLoadoutNames.Add(loadoutDefault.sName);
            } else {
                //If the loadout name exists, then just load it
                lstLoadoutNames.Add(PlayerPrefs.GetString(GetKeyName(chartype, i)));
            }
        }

        return lstLoadoutNames;
    }

    public static string GetKeySkills(CharType.CHARTYPE chartype, int iSlot, int iSkillType) {
        return string.Format(SKEYSKILLS, (int)chartype, iSlot, iSkillType);
    }

    public static Loadout LoadSavedLoadoutForChr(CharType.CHARTYPE chartype, int iSlot) {
        Debug.Assert(iSlot < nLOADOUTSLOTS, "Cannot load slot " + iSlot + " since we don't allow that many slots");

        if(PlayerPrefs.HasKey(GetKeyName(chartype, iSlot)) == false) {
            //If we don't have a stored loadout for this slot, then save a default loadout in this slot and return it  

            Debug.LogFormat("No loadout for {0} - setting defaults", chartype);

            Loadout loadoutDefault = GetDefaultLoadoutForChar(chartype);
            SaveLoadout(chartype, iSlot, loadoutDefault);

            return loadoutDefault; 
        }

        //If we get this far, then we have a loadout stored in this slot, so we can load it

        // Fetch the name of the loadout
        string _sName = PlayerPrefs.GetString(GetKeyName(chartype, iSlot));

        // Fetch all the stored standard skill selections
        List<SkillType.SKILLTYPE> _lstChosenSkills = new List<SkillType.SKILLTYPE>();

        for(int i = 0; i < Chr.nMaxTotalChosenSkills; i++) {
            string sKey = GetKeySkills(chartype, iSlot, i);

            Debug.Assert(PlayerPrefs.HasKey(sKey), "No stored entry for " + sKey + " found");

            _lstChosenSkills.Add((SkillType.SKILLTYPE)PlayerPrefs.GetInt(sKey));
        }

        return new Loadout(_sName, _lstChosenSkills);
    }


    public static void SaveLoadout(CharType.CHARTYPE chartype, int iSlot, Loadout loadout) {
        Debug.Assert(iSlot < nLOADOUTSLOTS, "Cannot save slot " + iSlot + " since we don't allow that many slots");

        // Save the name of the loadout
        PlayerPrefs.SetString(GetKeyName(chartype, iSlot), loadout.sName);

        // Save all the standard skill selections
        for(int i = 0; i < Chr.nMaxTotalChosenSkills; i++) {
            PlayerPrefs.SetInt(GetKeySkills(chartype, iSlot, i), (int)loadout.lstChosenSkills[i]);
        }
    }


    static Dictionary<CharType.CHARTYPE, Loadout> dictDefaultLoadouts = new Dictionary<CharType.CHARTYPE, Loadout>() {

        { CharType.CHARTYPE.FISCHER, new Loadout("Default", new List<SkillType.SKILLTYPE>() { ADVANCE, BUCKLERPARRY, IMPALE, HARPOONGUN,
            FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.KATARINA, new Loadout("Default", new List<SkillType.SKILLTYPE>() { FORTISSIMO, REVERBERATE, CACOPHONY, SERENADE,
            FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.RAYNE, new Loadout("Default", new List<SkillType.SKILLTYPE>() { CHEERLEADER, CLOUDCUSHION, SPIRITSLAP, THUNDERSTORM,
            FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.SAIKO, new Loadout("Default", new List<SkillType.SKILLTYPE>() { AMBUSH, SMOKECOVER, STICKYBOMB, TRANQUILIZE,
            FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.PITBEAST, new Loadout("Default", new List<SkillType.SKILLTYPE>() { FORCEDEVOLUTION, SADISM, TANTRUM, TENDRILSTAB,
            FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.SOPHIDIA, new Loadout("Default", new List<SkillType.SKILLTYPE>() { HISS, HYDRASREGEN, TWINSNAKES, VENEMOUSBITE,
            FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },



        { CharType.CHARTYPE.DASHER, new Loadout("Default", new List<SkillType.SKILLTYPE>() { BUNKER, LEECH, KNOCKBACK, EXPLOSION,
            FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.DANCER, new Loadout("Default", new List<SkillType.SKILLTYPE>() { BUNKER, LEECH, KNOCKBACK, EXPLOSION,
            FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.PRANCER, new Loadout("Default", new List<SkillType.SKILLTYPE>() { BUNKER, LEECH, KNOCKBACK, EXPLOSION,
            FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.VIXEN, new Loadout("Default", new List<SkillType.SKILLTYPE>() { BUNKER, LEECH, KNOCKBACK, EXPLOSION,
            FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.COMET, new Loadout("Default", new List<SkillType.SKILLTYPE>() { BUNKER, LEECH, KNOCKBACK, EXPLOSION,
            FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.CUPID, new Loadout("Default", new List<SkillType.SKILLTYPE>() { BUNKER, LEECH, KNOCKBACK, EXPLOSION,
            FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.DONNER, new Loadout("Default", new List<SkillType.SKILLTYPE>() { BUNKER, LEECH, KNOCKBACK, EXPLOSION,
            FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.BLITZEN, new Loadout("Default", new List<SkillType.SKILLTYPE>() { BUNKER, LEECH, KNOCKBACK, EXPLOSION,
            FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.RUDOLPH, new Loadout("Default", new List<SkillType.SKILLTYPE>() { BUNKER, LEECH, KNOCKBACK, EXPLOSION,
            FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.SANTA, new Loadout("Default", new List<SkillType.SKILLTYPE>() { BUNKER, LEECH, KNOCKBACK, EXPLOSION,
            FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.SLIME, new Loadout("Default", new List<SkillType.SKILLTYPE>() { SUMMONSLIME, LEECH, NULL, NULL,
            NULL, NULL, NULL, NULL }) }

    };


    public static Loadout GetDefaultLoadoutForChar(CharType.CHARTYPE chartype) {

        return dictDefaultLoadouts[chartype];

    }


}
