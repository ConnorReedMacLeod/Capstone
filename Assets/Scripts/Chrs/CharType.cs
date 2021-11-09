using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Discipline.DISCIPLINE;

public static class CharType {

    public enum CHARTYPE {
        FISCHER, KATARINA, RAYNE, SAIKO, PITBEAST, SOPHIDIA,

        DASHER, DANCER, PRANCER, VIXEN, COMET, CUPID, DONNER, BLITZEN, RUDOLPH, SANTA,

        LENGTH
    };

    public struct CharTypeInfo {
        public CHARTYPE type;
        public string sName;

        public List<Discipline.DISCIPLINE> lstDisciplines;

        public CharTypeInfo(CHARTYPE _type, string _sName, List<Discipline.DISCIPLINE> _lstDisciplines) {
            type = _type;
            sName = _sName;
            lstDisciplines = _lstDisciplines;
        }
    }


    public static Dictionary<CHARTYPE, CharTypeInfo> dictChrTypeInfos = new Dictionary<CHARTYPE, CharTypeInfo>() {
        { CHARTYPE.FISCHER, new CharTypeInfo(CHARTYPE.FISCHER, "Fischer", new List<Discipline.DISCIPLINE>() { FISCHER, TESTING } ) },
        { CHARTYPE.KATARINA, new CharTypeInfo(CHARTYPE.KATARINA, "Katarina", new List<Discipline.DISCIPLINE>() { KATARINA, TESTING } ) },
        { CHARTYPE.RAYNE, new CharTypeInfo(CHARTYPE.RAYNE, "Rayne", new List<Discipline.DISCIPLINE>() { RAYNE, TESTING } ) },
        { CHARTYPE.SAIKO, new CharTypeInfo(CHARTYPE.SAIKO, "Saiko", new List<Discipline.DISCIPLINE>() { SAIKO, TESTING } ) },
        { CHARTYPE.PITBEAST, new CharTypeInfo(CHARTYPE.PITBEAST, "PitBeast", new List<Discipline.DISCIPLINE>() { PITBEAST, TESTING } ) },
        { CHARTYPE.SOPHIDIA, new CharTypeInfo(CHARTYPE.SOPHIDIA, "Sophidia", new List<Discipline.DISCIPLINE>() { SOPHIDIA, TESTING } ) },

        { CHARTYPE.DASHER, new CharTypeInfo(CHARTYPE.DASHER, "Dasher", new List<Discipline.DISCIPLINE>() { TESTING } ) },
        { CHARTYPE.DANCER, new CharTypeInfo(CHARTYPE.DANCER, "Dancer", new List<Discipline.DISCIPLINE>() { TESTING } ) },
        { CHARTYPE.PRANCER, new CharTypeInfo(CHARTYPE.PRANCER, "Prancer", new List<Discipline.DISCIPLINE>() { TESTING } ) },
        { CHARTYPE.VIXEN, new CharTypeInfo(CHARTYPE.VIXEN, "Vixen", new List<Discipline.DISCIPLINE>() { TESTING } ) },
        { CHARTYPE.COMET, new CharTypeInfo(CHARTYPE.COMET, "Comet", new List<Discipline.DISCIPLINE>() { TESTING } ) },
        { CHARTYPE.CUPID, new CharTypeInfo(CHARTYPE.CUPID, "Cupid", new List<Discipline.DISCIPLINE>() { TESTING } ) },
        { CHARTYPE.DONNER, new CharTypeInfo(CHARTYPE.DONNER, "Donner", new List<Discipline.DISCIPLINE>() { TESTING } ) },
        { CHARTYPE.BLITZEN, new CharTypeInfo(CHARTYPE.BLITZEN, "Blitzen", new List<Discipline.DISCIPLINE>() { TESTING } ) },
        { CHARTYPE.RUDOLPH, new CharTypeInfo(CHARTYPE.RUDOLPH, "Rudolph", new List<Discipline.DISCIPLINE>() { TESTING } ) },
        { CHARTYPE.SANTA, new CharTypeInfo(CHARTYPE.SANTA, "Santa", new List<Discipline.DISCIPLINE>() { TESTING } ) },
    };

    public static string GetChrName(CHARTYPE type) {
        return dictChrTypeInfos[type].sName;
    }

    public static List<Discipline.DISCIPLINE> GetDisciplines(CHARTYPE type) {
        return dictChrTypeInfos[type].lstDisciplines;
    }

}
