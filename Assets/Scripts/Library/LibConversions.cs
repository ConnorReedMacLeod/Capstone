using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LibConversions {


    public static object[] ArIntToArObj(int[] arInt) {

        object[] arObj = new object[arInt.Length];

        for(int i = 0; i < arInt.Length; i++) {
            arObj[i] = (object)arInt[i];
        }

        return arObj;
    }

    public static int[] ArObjToArInt(object[] arObj) {

        int[] arInt = new int[arObj.Length];

        for(int i = 0; i < arObj.Length; i++) {
            arInt[i] = (int)arObj[i];
        }

        return arInt;
    }

    //TODO:: Eventually figure out if this can be generalized
    public static int[] ArChrTypeToArInt(CharType.CHARTYPE[] _arChrTypes) {
        int[] arInt = new int[_arChrTypes.Length];

        for(int i = 0; i < _arChrTypes.Length; i++) {
            arInt[i] = (int)_arChrTypes[i];
        }

        return arInt;
    }

    public static int[][] ArArChrTypeToArArInt(CharType.CHARTYPE[][] _ararChrTypes) {
        int[][] ararInt = new int[_ararChrTypes.Length][];

        for (int i = 0; i < _ararChrTypes.Length; i++) {
            ararInt[i] = ArChrTypeToArInt(_ararChrTypes[i]);
        }

        return ararInt;
    }

    public static CharType.CHARTYPE[] ArIntToArChrType(int[] _arInt) {
        CharType.CHARTYPE[] arChrTypes = new CharType.CHARTYPE[_arInt.Length];

        for(int i = 0; i < _arInt.Length; i++) {
            arChrTypes[i] = (CharType.CHARTYPE)_arInt[i];
        }

        return arChrTypes;
    }

    public static CharType.CHARTYPE[][] ArARIntToArArChrType(int[][] _ararInt) {
        CharType.CHARTYPE[][] ararChrTypes = new CharType.CHARTYPE[_ararInt.Length][];

        for (int i = 0; i < _ararInt.Length; i++) {
            ararChrTypes[i] = ArIntToArChrType(_ararInt[i]);
        }

        return ararChrTypes;
    }

    public static object[] ArInputTypeToArObj(Player.InputType[] _arInputTypes) {
        object[] arObj = new object[_arInputTypes.Length];

        for(int i=0; i<_arInputTypes.Length; i++) {
            arObj[i] = (object)_arInputTypes[i];
        }

        return arObj;
    }

    public static Player.InputType[] ArObjToArInputType(object[] _arObj) {
        Player.InputType[] arInputTypes = new Player.InputType[_arObj.Length];

        for(int i = 0; i < _arObj.Length; i++) {
            arInputTypes[i] = (Player.InputType)_arObj[i];
        }

        return arInputTypes;
    }

}
