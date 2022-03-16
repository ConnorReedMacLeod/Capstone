using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LibConversions {

    // AR<T> / Str
    public static string ArToStr<T>(T[] arT) {

        string s = "";

        //For each entry of our match input, add it to the string we'll be logging
        for (int i = 0; i < arT.Length; i++) {
            s += ":" + arT[i].ToString();
        }

        return s;
    }

    // INT / OBJ

    public static object[] ArIntToArObj(int[] arInt) {
        if (arInt == null) return null;

        object[] arObj = new object[arInt.Length];

        for(int i = 0; i < arInt.Length; i++) {
            arObj[i] = (object)arInt[i];
        }

        return arObj;
    }

    public static int[] ArObjToArInt(object[] arObj) {
        if (arObj == null) return null;

        int[] arInt = new int[arObj.Length];

        for(int i = 0; i < arObj.Length; i++) {
            arInt[i] = (int)arObj[i];
        }

        return arInt;
    }


    //TODO:: Eventually figure out if this can be generalized

    //  CHRTYPE / INT

    public static int[] ArChrTypeToArInt(CharType.CHARTYPE[] arChrTypes) {
        if (arChrTypes == null) return null;

        int[] arInt = new int[arChrTypes.Length];

        for(int i = 0; i < arChrTypes.Length; i++) {
            arInt[i] = (int)arChrTypes[i];
        }

        return arInt;
    }

    public static int[][] ArArChrTypeToArArInt(CharType.CHARTYPE[][] ararChrTypes) {
        if (ararChrTypes == null) return null;

        int[][] ararInt = new int[ararChrTypes.Length][];

        for (int i = 0; i < ararChrTypes.Length; i++) {
            ararInt[i] = ArChrTypeToArInt(ararChrTypes[i]);
        }

        return ararInt;
    }

    public static CharType.CHARTYPE[] ArIntToArChrType(int[] arInt) {
        if (arInt == null) return null;

        CharType.CHARTYPE[] arChrTypes = new CharType.CHARTYPE[arInt.Length];

        for(int i = 0; i < arInt.Length; i++) {
            arChrTypes[i] = (CharType.CHARTYPE)arInt[i];
        }

        return arChrTypes;
    }

    public static CharType.CHARTYPE[][] ArArIntToArArChrType(int[][] ararInt) {
        if (ararInt == null) return null;

        CharType.CHARTYPE[][] ararChrTypes = new CharType.CHARTYPE[ararInt.Length][];

        for (int i = 0; i < ararInt.Length; i++) {
            ararChrTypes[i] = ArIntToArChrType(ararInt[i]);
        }

        return ararChrTypes;
    }


    // COORDS / INT

    public static int[] ArPositionCoordToArInt(Position.Coords[] arPositionCoords) {
        if (arPositionCoords == null) return null;

        int[] arInt = new int[arPositionCoords.Length];

        for (int i = 0; i < arPositionCoords.Length; i++) {
            arInt[i] = Position.SerializeCoords(arPositionCoords[i]);
        }

        return arInt;
    }

    public static int[][] ArArPositionCoordToArArInt(Position.Coords[][] ararPositionCoords) {
        if (ararPositionCoords == null) return null;

        int[][] ararInt = new int[ararPositionCoords.Length][];

        for (int i = 0; i < ararPositionCoords.Length; i++) {
            ararInt[i] = ArPositionCoordToArInt(ararPositionCoords[i]);
        }

        return ararInt;
    }

    public static Position.Coords[] ArIntToArPositionCoord(int[] arInt) {
        if (arInt == null) return null;

        Position.Coords[] arPositionCoords = new Position.Coords[arInt.Length];

        for (int i = 0; i < arInt.Length; i++) {
            arPositionCoords[i] = Position.UnserializeCoords(arInt[i]);
        }

        return arPositionCoords;
    }

    public static Position.Coords[][] ArArIntToArArPositionCoord(int[][] ararInt) {
        if (ararInt == null) return null;

        Position.Coords[][] ararPositionCoords = new Position.Coords[ararInt.Length][];

        for (int i = 0; i < ararInt.Length; i++) {
            ararPositionCoords[i] = ArIntToArPositionCoord(ararInt[i]);
        }

        return ararPositionCoords;
    }
}
