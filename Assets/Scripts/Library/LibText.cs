using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public static class LibText {

    public static Dictionary<string, char> dictTextConversions =
        new Dictionary<string, char>(){
        {"P", '1'},//(char)176},
		{"M", '2'},//(char)177},
		{"E", '3'},//(char)178},
		{"B", '4'},//(char)179},
		{"O", '5' } };//(char)180}};

    //Convert a single Symbol to it's ascii char representation
    public static char PrepSymbol(string sSym) {
        if(dictTextConversions.ContainsKey(sSym)) {
            return dictTextConversions[sSym];
        } else {
            Debug.LogError("ERROR!  " + sSym + " cannot be converted to a ascii character");
            return (char)0;
        }
    }

    //Convert any escaped words (using |xxx|) with their assigned ascii value
    public static string PrepText(string sText) {

        string[] arsComponents = sText.Split('|');

        //Skip over the first section of text which isn't escaped
        int i = 1;


        while(i < arsComponents.Length) {

            arsComponents[i] = PrepSymbol(arsComponents[i]).ToString();

            i += 2; //advance to the next escaped string
        }

        return string.Join(string.Empty, arsComponents);
    }

    public static string AddAllegianceColour(string sString, bool bAllied) {
        return AddRichColour(sString, bAllied ? "green" : "red");
    }

    public static string AddRichColour(string sString, string sColour) {
        return string.Format("<color={0}>{1}</color>", sColour, sString);
    }
}
