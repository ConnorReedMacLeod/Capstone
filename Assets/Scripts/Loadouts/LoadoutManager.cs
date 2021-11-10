using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SkillType.SKILLTYPE;

public static class LoadoutManager {

    public const int nLOADOUTSLOTS = 3;
    public const string SKEYNAME = "loadout{0}-{1}-name";
    public const string SKEYEQUIPPED = "loadout{0}-{1}-equipped-{2}";
    public const string SKEYBENCHED = "loadout{0}-{1}-bench-{2}";

    public struct Loadout {
        public string sName;

        public List<SkillType.SKILLTYPE> lstEquippedSkills;
        public List<SkillType.SKILLTYPE> lstBenchSkills;

        public Loadout(string _sName, List<SkillType.SKILLTYPE> _lstEquippedSkills, List<SkillType.SKILLTYPE> _lstBenchSkills) {
            sName = _sName;
            lstEquippedSkills = _lstEquippedSkills;
            lstBenchSkills = _lstBenchSkills;
        }

        public override string ToString() {
            string sLoadout = string.Format("{0}:\nEquipped: {1}, {2}, {3}, {4}\nBench: {5}, {6}, {7}, {8}",
                sName,
                lstEquippedSkills[0], lstEquippedSkills[1], lstEquippedSkills[2], lstEquippedSkills[3],
                lstBenchSkills[0], lstBenchSkills[1], lstBenchSkills[2], lstBenchSkills[3]);

            return sLoadout;
        }
    }

    public static int[] SerializeLoadout(Loadout loadout) {
        int[] arSerialized = new int[Chr.nTotalCharacterSkills];

        int iSerialized = 0;
        for(int i = 0; i < Chr.nEquippedCharacterSkills; i++, iSerialized++) {
            arSerialized[iSerialized] = (int)loadout.lstEquippedSkills[i];
        }
        for(int i = 0; i < Chr.nBenchCharacterSkills; i++, iSerialized++) {
            arSerialized[iSerialized] = (int)loadout.lstBenchSkills[i];
        }

        return arSerialized;
    }

    public static Loadout UnserializeLoadout(int[] arSerialized) {

        List<SkillType.SKILLTYPE> lstEquippedSkills = new List<SkillType.SKILLTYPE>();
        List<SkillType.SKILLTYPE> lstBenchedSkills = new List<SkillType.SKILLTYPE>();

        int iDeserialized = 0;
        for(int i = 0; i < Chr.nEquippedCharacterSkills; i++, iDeserialized++) {
            lstEquippedSkills.Add((SkillType.SKILLTYPE)arSerialized[iDeserialized]);
        }
        for(int i = 0; i < Chr.nBenchCharacterSkills; i++, iDeserialized++) {
            lstBenchedSkills.Add((SkillType.SKILLTYPE)arSerialized[iDeserialized]);
        }

        return new Loadout("Deserialized", lstEquippedSkills, lstBenchedSkills);
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

    public static string GetKeyEquipped(CharType.CHARTYPE chartype, int iSlot, int iSkillType) {
        return string.Format(SKEYEQUIPPED, (int)chartype, iSlot, iSkillType);
    }

    public static string GetKeyBenched(CharType.CHARTYPE chartype, int iSlot, int iSkillType) {
        return string.Format(SKEYBENCHED, (int)chartype, iSlot, iSkillType);
    }

    public static List<Loadout> LoadSavedAllLoadoutsForChr(CharType.CHARTYPE chartype) {

        List<Loadout> lstLoadouts = new List<Loadout>();

        for(int i = 0; i < nLOADOUTSLOTS; i++) {
            if(PlayerPrefs.HasKey(GetKeyName(chartype, i)) == false) {
                //If we try to load the ith slot for this character, but there's no entry for the name of the loadout,
                //  we assume the loadout is blank so we can just save in the default loadout for the character then use it

                Loadout loadoutDefault = GetDefaultLoadoutForChar(chartype);
                SaveLoadout(chartype, i, loadoutDefault);

                lstLoadouts.Add(loadoutDefault);

            } else {
                //If the loadout should exist, then load it and add it to our return list
                lstLoadouts.Add(LoadSavedLoadoutForChr(chartype, i));
            }

        }

        return lstLoadouts;
    }

    public static Loadout LoadSavedLoadoutForChr(CharType.CHARTYPE chartype, int iSlot) {
        Debug.Assert(iSlot < nLOADOUTSLOTS, "Cannot load slot " + iSlot + " since we don't allow that many slots");

        // Fetch the name of the loadout
        string _sName = PlayerPrefs.GetString(GetKeyName(chartype, iSlot));

        // Fetch all the stored standard skill selections
        List<SkillType.SKILLTYPE> _lstEquippedSkills = new List<SkillType.SKILLTYPE>();

        for(int i = 0; i < Chr.nEquippedCharacterSkills; i++) {
            string sKey = GetKeyEquipped(chartype, iSlot, i);

            Debug.Assert(PlayerPrefs.HasKey(sKey) == false, "No stored entry for " + sKey + " found");

            _lstEquippedSkills.Add((SkillType.SKILLTYPE)PlayerPrefs.GetInt(sKey));
        }

        // Then fetch all the stored bench skill selections
        List<SkillType.SKILLTYPE> _lstBenchSkills = new List<SkillType.SKILLTYPE>();

        for(int i = 0; i < Chr.nBenchCharacterSkills; i++) {
            string sKey = GetKeyBenched(chartype, iSlot, i);

            Debug.Assert(PlayerPrefs.HasKey(sKey) == false, "No stored entry for " + sKey + " found");

            _lstBenchSkills.Add((SkillType.SKILLTYPE)PlayerPrefs.GetInt(sKey));
        }

        return new Loadout(_sName, _lstEquippedSkills, _lstBenchSkills);
    }


    public static void SaveLoadout(CharType.CHARTYPE chartype, int iSlot, Loadout loadout, string sName = "Default") {
        Debug.Assert(iSlot < nLOADOUTSLOTS, "Cannot save slot " + iSlot + " since we don't allow that many slots");

        // Save the name of the loadout
        PlayerPrefs.SetString(string.Format(SKEYNAME, (int)chartype, iSlot), sName);

        // Save all the standard skill selections
        for(int i = 0; i < Chr.nEquippedCharacterSkills; i++) {
            PlayerPrefs.SetInt(GetKeyEquipped(chartype, iSlot, i), (int)loadout.lstEquippedSkills[i]);
        }

        // Then save all the bench skill selections
        for(int i = 0; i < Chr.nBenchCharacterSkills; i++) {
            PlayerPrefs.SetInt(GetKeyBenched(chartype, iSlot, i), (int)loadout.lstEquippedSkills[i]);
        }
    }


    static Dictionary<CharType.CHARTYPE, Loadout> dictDefaultLoadouts = new Dictionary<CharType.CHARTYPE, Loadout>() {

        { CharType.CHARTYPE.FISCHER, new Loadout("Default", new List<SkillType.SKILLTYPE>() { ADVANCE, BUCKLERPARRY, IMPALE, HARPOONGUN},
            new List<SkillType.SKILLTYPE>() { FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.KATARINA, new Loadout("Default", new List<SkillType.SKILLTYPE>() { FORTISSIMO, REVERBERATE, CACOPHONY, SERENADE},
            new List<SkillType.SKILLTYPE>() { FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.RAYNE, new Loadout("Default", new List<SkillType.SKILLTYPE>() { CHEERLEADER, CLOUDCUSHION, SPIRITSLAP, THUNDERSTORM},
            new List<SkillType.SKILLTYPE>() { FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.SAIKO, new Loadout("Default", new List<SkillType.SKILLTYPE>() { AMBUSH, SMOKECOVER, STICKYBOMB, TRANQUILIZE},
            new List<SkillType.SKILLTYPE>() { FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.PITBEAST, new Loadout("Default", new List<SkillType.SKILLTYPE>() { FORCEDEVOLUTION, SADISM, TANTRUM, TENDRILSTAB},
            new List<SkillType.SKILLTYPE>() { FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.SOPHIDIA, new Loadout("Default", new List<SkillType.SKILLTYPE>() { HISS, HYDRASREGEN, TWINSNAKES, VENEMOUSBITE},
            new List<SkillType.SKILLTYPE>() { FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },



        { CharType.CHARTYPE.DASHER, new Loadout("Default", new List<SkillType.SKILLTYPE>() { BUNKER, LEECH, KNOCKBACK, EXPLOSION},
            new List<SkillType.SKILLTYPE>() { FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.DANCER, new Loadout("Default", new List<SkillType.SKILLTYPE>() { BUNKER, LEECH, KNOCKBACK, EXPLOSION},
            new List<SkillType.SKILLTYPE>() { FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.PRANCER, new Loadout("Default", new List<SkillType.SKILLTYPE>() { BUNKER, LEECH, KNOCKBACK, EXPLOSION},
            new List<SkillType.SKILLTYPE>() { FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.VIXEN, new Loadout("Default", new List<SkillType.SKILLTYPE>() { BUNKER, LEECH, KNOCKBACK, EXPLOSION},
            new List<SkillType.SKILLTYPE>() { FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.COMET, new Loadout("Default", new List<SkillType.SKILLTYPE>() { BUNKER, LEECH, KNOCKBACK, EXPLOSION},
            new List<SkillType.SKILLTYPE>() { FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.CUPID, new Loadout("Default", new List<SkillType.SKILLTYPE>() { BUNKER, LEECH, KNOCKBACK, EXPLOSION},
            new List<SkillType.SKILLTYPE>() { FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.DONNER, new Loadout("Default", new List<SkillType.SKILLTYPE>() { BUNKER, LEECH, KNOCKBACK, EXPLOSION},
            new List<SkillType.SKILLTYPE>() { FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.BLITZEN, new Loadout("Default", new List<SkillType.SKILLTYPE>() { BUNKER, LEECH, KNOCKBACK, EXPLOSION},
            new List<SkillType.SKILLTYPE>() { FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.RUDOLPH, new Loadout("Default", new List<SkillType.SKILLTYPE>() { BUNKER, LEECH, KNOCKBACK, EXPLOSION},
            new List<SkillType.SKILLTYPE>() { FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

        { CharType.CHARTYPE.SANTA, new Loadout("Default", new List<SkillType.SKILLTYPE>() { BUNKER, LEECH, KNOCKBACK, EXPLOSION},
            new List<SkillType.SKILLTYPE>() { FIREBALL, HEAL, ADVANCE, STRATEGIZE}) },

    };


    public static Loadout GetDefaultLoadoutForChar(CharType.CHARTYPE chartype) {

        return dictDefaultLoadouts[chartype];

    }


}
