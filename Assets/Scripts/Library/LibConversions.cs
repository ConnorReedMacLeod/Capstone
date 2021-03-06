﻿using System.Collections;
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
    public static int[] ArChrTypeToArInt(Chr.CHARTYPE[] _arChrTypes) {
        int[] arInt = new int[_arChrTypes.Length];

        for(int i = 0; i < _arChrTypes.Length; i++) {
            arInt[i] = (int)_arChrTypes[i];
        }

        return arInt;
    }

    public static Chr.CHARTYPE[] ArIntToArChrType(int[] _arInt) {
        Chr.CHARTYPE[] arChrTypes = new Chr.CHARTYPE[_arInt.Length];

        for(int i = 0; i < _arInt.Length; i++) {
            arChrTypes[i] = (Chr.CHARTYPE)_arInt[i];
        }

        return arChrTypes;
    }

    public static Player.InputType[] ArObjToArInputType(object[] _arObj) {
        Player.InputType[] arInputTypes = new Player.InputType[_arObj.Length];

        for(int i = 0; i < _arObj.Length; i++) {
            arInputTypes[i] = (Player.InputType)_arObj[i];
        }

        return arInputTypes;
    }
}
