using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class LibDebug {
    
    public enum Col { BLACK, RED, BLUE, GREEN, MAGENTA };
    public static readonly string[] ColNames = new string[] { "black", "red", "blue", "green", "magenta" };
    
    public static void LogWithColor(string sText, Col col, params object[] args) {
        Debug.LogFormat(string.Format("<color={0}>{1}</color>", ColNames[(int)col], sText), args);
    }

    //TODO - add in file logging
}
