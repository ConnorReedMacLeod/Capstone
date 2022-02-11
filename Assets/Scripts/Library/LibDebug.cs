using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class LibDebug {
    
    public enum Col { BLACK, RED, BLUE, GREEN, MAGENTA };
    public static readonly string[] ColNames = new string[] { "black", "red", "blue", "green", "magenta" };
    
    public static string AddColor(string sText, Col col) {
        return string.Format("<color={0}>{1}</color>", ColNames[(int)col], sText);
    }


    //TODO - add in file logging
}
